# TradeFlow — Plan technique complet

## Vision
TradeFlow = deux services en un :
- **Boutique** : scripts Pine Script validés manuellement (livraison par fichier .pine / PDF)
- **Atelier sur-mesure** : le client décrit sa stratégie → Loïc code le script → livraison de fichiers

---

## Architecture

```
TradeFlow/
├── api/                          ← .NET 9 Clean Architecture
│   └── src/
│       ├── TradeFlow.Domain/
│       ├── TradeFlow.Application/
│       ├── TradeFlow.Infrastructure/
│       └── TradeFlow.Api/
├── web/                          ← Next.js 14
├── docker-compose.yml
└── .gitignore
```

---

## Entités domaine

### Catalogue
| Entité | Champs clés |
|--------|-------------|
| **Product** | Title, Slug, ShortDescription, LongDescription, Type (Indicator/Strategy/Bundle), Price, Status (Draft/Published/Archived), CategoryId |
| **Category** | Name, Slug |
| **ProductMedia** | ProductId, Url, Type (Image/Video), SortOrder |
| **BacktestReport** | ProductId, Title, WinRate, MaxDrawdown, ProfitFactor, PeriodStart, PeriodEnd, Markets |

### Clients & Commandes
| Entité | Champs clés |
|--------|-------------|
| **Customer** | FirebaseUid, Email, DisplayName, TradingViewUsername |
| **Order** | CustomerId, Total, Status (Pending/Paid/Fulfilled/Refunded), StripePaymentIntentId |
| **OrderItem** | OrderId, ProductId, UnitPrice |

### Licences (accès au téléchargement)
| Entité | Champs clés |
|--------|-------------|
| **License** | CustomerId, ProductId, DownloadToken (unique), DownloadCount, MaxDownloads (3), ExpiresAt, Status (Active/Expired/Revoked) |

### Projets sur-mesure
| Entité | Champs clés |
|--------|-------------|
| **CustomProjectRequest** | CustomerId, Market, Timeframe, EntryConditions, ExitConditions, RiskManagement, Status, QuotedPrice, DepositAmount, DeliveryFileUrl, AdminNotes |

**Statuts CustomProjectRequest :**
```
Submitted → Quoted → DepositPaid → InProgress → Delivered → Cancelled
```

---

## Endpoints API

### Public
```
GET  /api/catalog              → liste produits publiés (filtres: catégorie, type)
GET  /api/catalog/{slug}       → détail produit + médias + backtest
GET  /api/categories           → liste catégories
GET  /health
```

### Client (authentifié Firebase)
```
POST /api/account/sync         → sync Firebase → Customer en base
PUT  /api/account/profile      → modifier profil (displayName, tradingViewUsername)

POST /api/orders               → créer commande (checkout Stripe)
GET  /api/orders               → mes commandes

GET  /api/licenses             → mes licences (achats)
GET  /api/licenses/{id}/download → générer lien téléchargement signé (R2)

POST /api/projects             → soumettre projet sur-mesure
GET  /api/projects             → mes projets
GET  /api/projects/{id}        → détail projet
POST /api/projects/{id}/accept-quote → accepter devis + payer acompte
```

### Admin (role: admin)
```
GET/POST     /api/admin/products
PUT          /api/admin/products/{id}
POST         /api/admin/products/{id}/publish
POST         /api/admin/products/{id}/archive
POST         /api/admin/products/{id}/media      → uploader média (R2)
DELETE       /api/admin/products/{id}/media/{mediaId}
POST         /api/admin/products/{id}/backtest   → attacher rapport backtest

GET          /api/admin/orders
GET          /api/admin/licenses
POST         /api/admin/licenses/{id}/revoke

GET          /api/admin/projects
GET          /api/admin/projects/{id}
POST         /api/admin/projects/{id}/quote      → envoyer devis
POST         /api/admin/projects/{id}/start      → démarrer le travail
POST         /api/admin/projects/{id}/deliver    → livrer les fichiers (upload R2)
POST         /api/admin/projects/{id}/cancel
```

### Webhooks
```
POST /api/webhooks/stripe      → payment_intent.succeeded → crée License
```

---

## Pages Frontend (Next.js 14)

### Publiques
```
/                        → Accueil (hero, produits phares, CTA sur-mesure)
/catalogue               → Grille produits + filtres (catégorie, type)
/catalogue/[slug]        → Fiche produit (description, captures, backtest, acheter)
/login                   → Connexion Firebase
/register                → Inscription Firebase
```

### Espace client (authentifié)
```
/dashboard               → Résumé (achats récents, projets actifs)
/dashboard/licenses      → Mes achats + bouton télécharger
/dashboard/orders        → Historique commandes
/dashboard/projects      → Mes projets sur-mesure
/dashboard/projects/new  → Formulaire soumission stratégie
/dashboard/projects/[id] → Détail projet (statut, devis, téléchargements)
```

### Admin
```
/admin                       → Dashboard (CA, commandes en attente, projets actifs)
/admin/products              → Liste produits
/admin/products/new          → Créer produit
/admin/products/[id]/edit    → Modifier produit (+ upload médias, backtest)
/admin/orders                → Toutes les commandes
/admin/licenses              → Toutes les licences
/admin/projects              → File projets sur-mesure
/admin/projects/[id]         → Gérer un projet (devis, livraison fichiers)
```

---

## Phases de développement

| PR | Contenu | Statut |
|----|---------|--------|
| **PR-001** | Socle : solution .NET, Docker, .gitignore | ✅ 2026-04-29 |
| **PR-002** | Domain layer : entités, value objects, domain events | ✅ 2026-04-29 |
| **PR-003** | Application layer : commands, queries, interfaces | ✅ 2026-04-29 |
| **PR-004** | Infrastructure : EF Core, migrations, services (Stripe, R2, Firebase) | ✅ 2026-04-29 |
| **PR-005** | API layer : controllers, Program.cs, auth Firebase | ✅ 2026-04-29 |
| **PR-006** | Frontend : Next.js init, layout, pages publiques (accueil, catalogue) | ⏳ À faire |
| **PR-007** | Frontend : espace client (licences, commandes, projets) | ⏳ À faire |
| **PR-008** | Frontend : admin dashboard | ⏳ À faire |
| **PR-009** | Intégration Stripe checkout + R2 upload/download | ⏳ À faire |
| **PR-010** | Tests + déploiement Railway (API) + Vercel (web) | ⏳ À faire |

---

## Stack technique

| Couche | Techno |
|--------|--------|
| Backend | .NET 9 — Clean Architecture, CQRS, MediatR, FluentValidation |
| Frontend | Next.js 14 — App Router, Tailwind, Shadcn/ui, TanStack Query |
| Auth | Firebase (JWT) |
| Paiements | Stripe |
| Base de données | PostgreSQL 16 |
| Fichiers | Cloudflare R2 (liens signés, expiration 72h) |
| ORM | EF Core 9 + Npgsql |
| Déploiement | Railway (API) + Vercel (web) |
