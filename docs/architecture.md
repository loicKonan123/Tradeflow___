# Architecture — TradeFlow

## Vue d'ensemble

```
┌─────────────────────────────────────────────────────────┐
│                     CLIENT (Next.js)                     │
│  Firebase Auth  │  Catalogue  │  Dashboard  │  Admin     │
└──────────────────────────┬──────────────────────────────┘
                           │ HTTP + Firebase JWT
┌──────────────────────────▼──────────────────────────────┐
│                  TradeFlow API (.NET 9)                   │
│  Controllers → MediatR → Commands/Queries                │
└──────┬──────────────┬──────────────┬────────────────────┘
       │              │              │
  ┌────▼────┐   ┌─────▼─────┐  ┌───▼──────┐
  │ PostgreSQL│   │ Cloudflare│  │  Stripe  │
  │  (Neon)  │   │    R2     │  │          │
  └──────────┘   └───────────┘  └──────────┘
```

---

## Structure du backend

```
backend/src/
├── TradeFlow.Domain/          ← Entités, règles métier, events
│   ├── Catalog/               ← Product, Category, ProductMedia, BacktestReport
│   ├── Customers/             ← Customer
│   ├── Orders/                ← Order, OrderItem
│   ├── Licensing/             ← License
│   ├── Projects/              ← CustomProjectRequest
│   ├── Events/                ← Domain events
│   └── Common/                ← BaseEntity, AggregateRoot, Result<T>
│
├── TradeFlow.Application/     ← Use cases (CQRS)
│   ├── Catalog/
│   │   ├── Commands/          ← Create, Update, Publish, Archive, SetFile
│   │   └── Queries/           ← GetProducts, GetBySlug, GetAdmin, GetCategories
│   ├── Customers/Commands/    ← Sync, UpdateProfile
│   ├── Orders/
│   │   ├── Commands/          ← CreateOrder, MarkOrderPaid
│   │   └── Queries/           ← GetMyOrders, GetAdminOrders
│   ├── Licensing/
│   │   ├── Commands/          ← Download, Revoke
│   │   └── Queries/           ← GetMyLicenses
│   ├── Projects/
│   │   ├── Commands/          ← Submit, SendQuote, StartWork, Deliver, Cancel
│   │   └── Queries/           ← GetMyProjects, GetAdminProjects
│   └── Common/
│       ├── Interfaces/        ← IApplicationDbContext, ICurrentUserService, IStorageService...
│       ├── Behaviors/         ← ValidationBehavior, LoggingBehavior
│       └── Models/            ← PagedResult<T>
│
├── TradeFlow.Infrastructure/  ← Implémentations techniques
│   ├── Persistence/
│   │   ├── ApplicationDbContext.cs
│   │   ├── ApplicationDbContextFactory.cs
│   │   ├── Configurations/    ← EF Core (snake_case, conversions)
│   │   └── Migrations/
│   └── Services/
│       ├── CurrentUserService.cs   ← Lit claims Firebase
│       ├── OrderNumberService.cs   ← TF-{year}-{count}
│       ├── StripeService.cs        ← Checkout Sessions
│       └── R2StorageService.cs     ← Upload + liens signés
│
└── TradeFlow.Api/             ← Couche HTTP
    ├── Controllers/
    │   ├── Public/            ← Catalog, Categories
    │   ├── Customer/          ← Account, Orders, Licenses, Projects
    │   ├── Admin/             ← AdminCatalog, AdminOrders, AdminLicenses, AdminProjects
    │   └── Webhooks/          ← StripeWebhook
    └── Program.cs             ← Firebase JWT, Swagger, CORS, DI
```

---

## Flux métier principaux

### Achat d'un script

```
Client                    API                      Stripe         R2
  │                        │                          │            │
  ├── POST /api/orders ───►│                          │            │
  │                        ├── vérifie doublons        │            │
  │                        ├── crée Order (Pending)    │            │
  │                        ├── CreateCheckoutSession ─►│            │
  │◄── { checkoutUrl } ────┤                          │            │
  │                        │                          │            │
  ├── redirect Stripe ─────────────────────────────►│            │
  ├── paiement ────────────────────────────────────►│            │
  │                        │◄── webhook (session.completed)        │
  │                        ├── MarkOrderPaid          │            │
  │                        ├── Order → Paid           │            │
  │                        └── License créée (token)  │            │
  │                                                   │            │
  ├── POST /api/licenses/{id}/download ─────────────────────────►│
  │◄── { url: "https://r2...?expires=1h" } ────────────────────────┤
  ├── télécharge le fichier ──────────────────────────────────────►│
```

### Projet sur-mesure

```
Statut:  Submitted → Quoted → DepositPaid → InProgress → Delivered
                 ↓         ↓            ↓            ↓
              (admin)  (client)       (admin)      (admin)
            SendQuote  AcceptQuote  StartWork     Deliver
```

---

## Sécurité

### Authentification
- Firebase JWT (tokens vérifiés par `securetoken.google.com`)
- Claim `user_id` = Firebase UID
- Claim `role` = `"admin"` pour les routes admin

### Authorisation
```
[AllowAnonymous]     → Catalogue public, health
[Authorize]          → Toutes les routes client
[Authorize("AdminOnly")] → Toutes les routes /api/admin/*
```

### Fichiers
- Les fichiers (`.pine`, PDF) sont stockés sur Cloudflare R2
- Jamais d'URL directe exposée : seuls des **liens signés expirables** (1h) sont générés
- Le compteur de téléchargements est incrémenté côté serveur

---

## Base de données

### Tables principales

| Table | Description |
|-------|-------------|
| `categories` | Catégories de produits |
| `products` | Scripts Pine Script (avec slug unique) |
| `product_medias` | Images/vidéos d'un produit |
| `backtest_reports` | Rapports de performance |
| `customers` | Utilisateurs (soft delete) |
| `orders` | Commandes (order_number unique) |
| `order_items` | Lignes de commandes |
| `licenses` | Licences de téléchargement (download_token unique) |
| `custom_project_requests` | Projets sur-mesure |

### Conventions
- Nommage : `snake_case`
- IDs : `UUID` (Guid)
- Dates : `UTC` uniquement
- Soft delete sur `customers` (`deleted_at`)
