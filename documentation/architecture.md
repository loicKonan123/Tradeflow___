# Architecture — TradeFlow

## Vue d'ensemble

```
┌──────────────────────────────────────────────────────────────┐
│                    CLIENT (Next.js 16)                        │
│  Firebase Auth  │  Catalogue  │  Dashboard  │  Admin          │
└──────────────────────────┬───────────────────────────────────┘
                           │ HTTPS + Firebase JWT
┌──────────────────────────▼───────────────────────────────────┐
│                   TradeFlow API (.NET 9)                       │
│  Controllers → MediatR → Commands / Queries                   │
└──────┬──────────────┬──────────────┬─────────────────────────┘
       │              │              │
  ┌────▼────┐   ┌─────▼─────┐  ┌───▼──────┐
  │PostgreSQL│   │Cloudflare │  │  Stripe  │
  │  (Neon)  │   │    R2     │  │          │
  └──────────┘   └───────────┘  └──────────┘
```

---

## Structure Backend

```
backend/src/
├── TradeFlow.Domain/              ← Entités, règles métier, events
│   ├── Catalog/                   ← Product, Category, ProductMedia, BacktestReport
│   ├── Customers/                 ← Customer
│   ├── Orders/                    ← Order, OrderItem
│   ├── Licensing/                 ← License (download_token, 5 DL max)
│   ├── Projects/                  ← CustomProjectRequest
│   ├── Events/                    ← Domain events (MediatR INotification)
│   └── Common/                    ← BaseEntity, AggregateRoot, Result<T>, strongly-typed IDs
│
├── TradeFlow.Application/         ← Use cases CQRS
│   ├── Catalog/Commands+Queries/
│   ├── Customers/Commands/
│   ├── Orders/Commands+Queries/
│   ├── Licensing/Commands+Queries/
│   ├── Projects/Commands+Queries/
│   └── Common/
│       ├── Interfaces/            ← IApplicationDbContext, ICurrentUserService,
│       │                             IStorageService, IStripeService, IOrderNumberService
│       ├── Behaviors/             ← ValidationBehavior, LoggingBehavior
│       └── Models/                ← PagedResult<T>
│
├── TradeFlow.Infrastructure/      ← Implémentations techniques
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── Configurations/        ← EF Core (snake_case, HasConversion typed IDs)
│   │   └── Migrations/            ← InitialCreate
│   └── Services/
│       ├── CurrentUserService.cs  ← Claims Firebase (user_id, email, role)
│       ├── OrderNumberService.cs  ← Format TF-{année}-{count:D4}
│       ├── StripeService.cs       ← Checkout Sessions
│       └── R2StorageService.cs    ← Upload + PreSigned URLs (1h)
│
└── TradeFlow.Api/                 ← Couche HTTP
    ├── Controllers/
    │   ├── Public/                ← CatalogController, CategoriesController
    │   ├── Customer/              ← AccountController, OrdersController,
    │   │                             LicensesController, ProjectsController
    │   ├── Admin/                 ← AdminCatalogController, AdminOrdersController,
    │   │                             AdminLicensesController, AdminProjectsController
    │   └── Webhooks/              ← StripeWebhookController
    └── Program.cs                 ← Firebase JWT, Swagger, CORS, DI
```

---

## Structure Frontend

```
frontend/src/
├── app/
│   ├── (public)/                  ← Layout avec Navbar + Footer (route group)
│   │   ├── page.tsx               ← Accueil (hero, services, produits vedettes, CTA)
│   │   ├── catalogue/page.tsx     ← Grille produits + filtres + recherche
│   │   └── sur-mesure/page.tsx    ← Landing + formulaire brief (non-connecté)
│   │
│   ├── (dashboard)/               ← Layout sidebar, auth guard (route group)
│   │   └── dashboard/
│   │       ├── page.tsx           ← Vue d'ensemble (stats, résumé)
│   │       ├── licenses/page.tsx  ← Licences + téléchargement signé R2
│   │       ├── orders/page.tsx    ← Historique commandes
│   │       └── projects/
│   │           ├── page.tsx       ← Liste projets + stepper de progression
│   │           └── new/page.tsx   ← Formulaire brief sur-mesure
│   │
│   ├── auth/                      ← Layout minimaliste
│   │   ├── login/page.tsx         ← Connexion Firebase
│   │   └── register/page.tsx      ← Inscription Firebase
│   │
│   ├── layout.tsx                 ← Root layout (Space Grotesk + Inter, AuthProvider)
│   └── globals.css                ← @theme Tailwind v4 (Kinetic Precision palette)
│
├── components/
│   ├── Navbar.tsx                 ← Sticky nav, lien actif, boutons auth
│   └── Footer.tsx                 ← Liens solutions / légal
│
├── contexts/
│   └── AuthContext.tsx            ← Firebase onAuthStateChanged, login/register/logout
│
└── lib/
    ├── firebase.ts                ← Initialisation lazy (évite crash SSR)
    └── api.ts                     ← Fetch helper + tous les appels API typés
```

---

## Flux métier principaux

### Achat d'un script (Boutique)

```
Client                    API                    Stripe          R2
  │                        │                       │              │
  ├─ POST /api/orders ────►│                       │              │
  │                        ├─ vérifie doublons      │              │
  │                        ├─ Order(Pending)        │              │
  │                        ├─ CreateCheckoutSession►│              │
  │◄─ { checkoutUrl } ─────┤                       │              │
  │                        │                       │              │
  ├─ redirect Stripe ──────────────────────────►  │              │
  ├─ paiement ────────────────────────────────►   │              │
  │                        │◄─ webhook (session.completed)        │
  │                        ├─ Order → Paid          │              │
  │                        └─ License créée / token │              │
  │                                                               │
  ├─ POST /api/licenses/{id}/download ──────────────────────────►│
  │◄─ { url: signed URL (1h) } ───────────────────────────────────┤
  ├─ télécharge fichier .pine ────────────────────────────────────►
```

### Projet sur-mesure (Atelier)

```
Statut machine :
  Submitted ──(admin)──► Quoted ──(client, dépôt Stripe)──► DepositPaid
       ──(admin)──► InProgress ──(admin + upload fichier)──► Delivered
  (n'importe quand avant InProgress) ──► Cancelled
```

---

## Sécurité

### Authentification
- Firebase JWT — tokens vérifiés par `securetoken.google.com/{projectId}`
- Claim `user_id` = Firebase UID
- Claim `role = "admin"` pour les routes admin (à poser manuellement en Firebase Custom Claims)

### Autorisation
| Préfixe | Niveau |
|---------|--------|
| `GET /api/catalog`, `/api/categories`, `/health` | Public |
| `POST /api/account`, `/api/orders`, `/api/licenses`, `/api/projects` | `[Authorize]` |
| `/api/admin/*` | `[Authorize(Policy = "AdminOnly")]` |
| `POST /api/webhooks/stripe` | Signature Stripe uniquement |

### Fichiers (R2)
- Les clés de fichiers ne sont jamais exposées directement
- Chaque téléchargement génère un **lien signé expirable (1h)**
- Max **5 téléchargements** par licence (compteur côté serveur)

---

## Base de données (PostgreSQL)

### Tables

| Table | Description |
|-------|-------------|
| `categories` | Catégories de produits |
| `products` | Scripts Pine Script (slug unique) |
| `product_medias` | Images/vidéos d'un produit |
| `backtest_reports` | Rapports (WinRate, Drawdown, PF) |
| `customers` | Utilisateurs Firebase (soft delete) |
| `orders` | Commandes (`order_number` unique `TF-YYYY-NNNN`) |
| `order_items` | Lignes de commande |
| `licenses` | Licences (`download_token` unique, 5 DL max) |
| `custom_project_requests` | Projets sur-mesure |

### Conventions
- Nommage colonnes : `snake_case` (via Npgsql)
- IDs : `UUID` (Guid)
- Dates : `UTC` uniquement
- Soft delete sur `customers` (`HasQueryFilter(c => c.DeletedAt == null)`)

---

## Design System — Kinetic Precision

Thème dark trading terminal, configuré via `@theme` dans `globals.css` (Tailwind v4).

| Rôle | Couleur |
|------|---------|
| Background | `#031427` (Midnight Slate) |
| On-surface (texte) | `#d3e4fe` |
| Secondary (accent bleu) | `#adc6ff` |
| Secondary-container (boutons) | `#0566d9` |
| Tertiary (neon vert, métriques) | `#4ae176` |
| Error | `#ffb4ab` |
| Outline-variant (bordures) | `#45464d` |

**Classes utilitaires globales** : `.glass-panel`, `.glass-card`, `.neon-border`, `.trading-bg`

**Polices** : Space Grotesk (headings, labels, data) + Inter (body)
