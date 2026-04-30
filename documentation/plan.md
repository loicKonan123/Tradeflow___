# TradeFlow — Plan technique

## Vision

TradeFlow = deux services en un :
- **Boutique** : scripts Pine Script v5 validés manuellement (livraison fichier `.pine` + PDF via lien R2 signé)
- **Atelier sur-mesure** : le client décrit sa stratégie → Loïc code → livraison de fichiers

---

## Architecture cible

```
TradeFlow/
├── backend/                      ← .NET 9 Clean Architecture
│   └── src/
│       ├── TradeFlow.Domain/
│       ├── TradeFlow.Application/
│       ├── TradeFlow.Infrastructure/
│       └── TradeFlow.Api/
├── frontend/                     ← Next.js 16 (Tailwind v4)
├── documentation/                ← Toute la documentation
├── docker-compose.yml
└── .gitignore
```

---

## Entités domaine

### Catalogue
| Entité | Champs clés |
|--------|-------------|
| `Category` | Id, Name, Slug |
| `Product` | Id, Title, Slug, Type (Indicator/Strategy), Price, Currency, FileUrl, Status (Draft/Published/Archived) |
| `ProductMedia` | Id, ProductId, Url, Type, SortOrder |
| `BacktestReport` | Id, ProductId, Title, WinRate, MaxDrawdown, ProfitFactor, PeriodStart, PeriodEnd, Markets |

### Commerce
| Entité | Champs clés |
|--------|-------------|
| `Customer` | Id, FirebaseUid, Email, DeletedAt |
| `Order` | Id, OrderNumber, CustomerId, StripeSessionId, PaymentIntentId, Status, TotalAmount |
| `OrderItem` | Id, OrderId, ProductId, UnitPrice |
| `License` | Id, OrderItemId, CustomerId, ProductId, DownloadToken (Guid N), MaxDownloads=5, DownloadsUsed |
| `CustomProjectRequest` | Id, CustomerId, Market, Timeframe, EntryConditions, ExitConditions, RiskManagement, Status, QuotedPrice, DepositAmount |

---

## Routes frontend

### Publiques
| Route | Page |
|-------|------|
| `/` | Accueil — hero, services, produits vedettes, CTA |
| `/catalogue` | Grille produits + filtres + recherche |
| `/catalogue/[slug]` | Fiche produit (médias, backtests, acheter) |
| `/sur-mesure` | Landing + formulaire brief |
| `/auth/login` | Connexion Firebase |
| `/auth/register` | Inscription Firebase |

### Espace client (authentifié)
| Route | Page |
|-------|------|
| `/dashboard` | Vue d'ensemble (stats, résumé) |
| `/dashboard/licenses` | Licences + téléchargement signed URL |
| `/dashboard/orders` | Historique commandes |
| `/dashboard/projects` | Projets sur-mesure + stepper |
| `/dashboard/projects/new` | Soumettre un brief |

### Admin
| Route | Page |
|-------|------|
| `/admin` | Dashboard KPIs |
| `/admin/products` | Catalogue (liste, créer, modifier, upload) |
| `/admin/orders` | Toutes les commandes |
| `/admin/projects` | File projets (devis, démarrer, livrer) |

---

## Phases de développement

| PR | Contenu | Statut |
|----|---------|--------|
| **PR-001** | Socle : solution .NET 9, Docker, .gitignore | ✅ 2026-04-29 |
| **PR-002** | Domain layer : entités, value objects, domain events | ✅ 2026-04-29 |
| **PR-003** | Application layer : CQRS, MediatR, FluentValidation | ✅ 2026-04-29 |
| **PR-004** | Infrastructure : EF Core 9, migrations, Stripe, R2, Firebase | ✅ 2026-04-29 |
| **PR-005** | API layer : controllers, Program.cs | ✅ 2026-04-29 |
| **PR-006** | Frontend : Next.js 16, Tailwind v4, Kinetic Precision, pages publiques | ✅ 2026-04-29 |
| **PR-007** | Frontend : Firebase auth + dashboard client | ✅ 2026-04-29 |
| **PR-008** | Frontend : admin dashboard | ✅ 2026-04-29 |
| **PR-009** | Stripe checkout redirect + acompte + R2 upload admin | ⏳ À faire |
| **PR-010** | Déploiement Railway + Vercel + Neon | ⏳ À faire |

---

## Stack technique

| Couche | Techno | Version |
|--------|--------|---------|
| Backend | .NET Clean Architecture, CQRS, MediatR, FluentValidation | .NET 9 |
| Frontend | Next.js, App Router, Tailwind v4 | Next.js 16 |
| Auth | Firebase JWT | — |
| Paiements | Stripe Checkout Sessions | Stripe.net 47 |
| Base de données | PostgreSQL + EF Core + Npgsql | EF Core 9 |
| Fichiers | Cloudflare R2, liens signés 1h | AWSSDK.S3 3.7 |
| Déploiement | Railway (API) + Vercel (web) + Neon (DB) | — |
