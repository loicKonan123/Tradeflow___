# TradeFlow — Suivi du projet

## Statut global

| Phase | Contenu | Statut | Date |
|-------|---------|--------|------|
| PR-001 | Socle technique (.NET, Docker, .gitignore) | ✅ Terminé | 2026-04-29 |
| PR-002 | Domain layer | ✅ Terminé | 2026-04-29 |
| PR-003 | Application layer (CQRS) | ✅ Terminé | 2026-04-29 |
| PR-004 | Infrastructure (EF Core, services) | ✅ Terminé | 2026-04-29 |
| PR-005 | API layer (controllers) | ✅ Terminé | 2026-04-29 |
| PR-006 | Frontend — layout, pages publiques | ⏳ À faire | — |
| PR-007 | Frontend — espace client | ⏳ À faire | — |
| PR-008 | Frontend — admin dashboard | ⏳ À faire | — |
| PR-009 | Stripe checkout + R2 upload/download | ⏳ À faire | — |
| PR-010 | Tests + déploiement Railway + Vercel | ⏳ À faire | — |

---

## PR-001 — Socle technique ✅

**Branch** : `feat/pr-002-backend-complet`

### Ce qui a été fait
- Solution .NET 9 avec 4 projets (Domain, Application, Infrastructure, Api)
- Références entre projets correctement configurées
- Packages NuGet installés : MediatR, FluentValidation, EF Core 9, Npgsql, Stripe.net, AWSSDK.S3, JwtBearer, Swashbuckle
- `docker-compose.yml` : PostgreSQL 16
- `.gitignore` complet (bin/, obj/, .env, secrets, node_modules)
- `appsettings.json` avec toutes les clés de configuration (Firebase, Stripe, R2, CORS)

---

## PR-002 — Domain layer ✅

### Entités créées

| Entité | Namespace | Description |
|--------|-----------|-------------|
| `Category` | `Domain.Catalog` | Catégorie de produits |
| `Product` | `Domain.Catalog` | Script Pine Script à vendre |
| `ProductMedia` | `Domain.Catalog` | Images/vidéos d'un produit |
| `BacktestReport` | `Domain.Catalog` | Rapport de performance (WinRate, Drawdown, etc.) |
| `Customer` | `Domain.Customers` | Utilisateur authentifié via Firebase |
| `Order` | `Domain.Orders` | Commande d'achat |
| `OrderItem` | `Domain.Orders` | Ligne d'une commande |
| `License` | `Domain.Licensing` | Droit de téléchargement (token unique, 5 DL max) |
| `CustomProjectRequest` | `Domain.Projects` | Demande de script sur-mesure |

### Value objects
- `Slug` — slugification automatique depuis le titre
- IDs fortement typés pour toutes les entités

### Domain events
- `ProductPublishedEvent`
- `OrderPaidEvent`
- `ProjectQuotedEvent`
- `ProjectDeliveredEvent`

### Patterns
- `Result<T>` pour la gestion d'erreurs sans exceptions
- `AggregateRoot<TId>` + `IAggregateRoot` pour les domain events
- `BaseEntity<TId>` avec `CreatedAt` / `UpdatedAt`

---

## PR-003 — Application layer ✅

### Interfaces
- `IApplicationDbContext` — accès à la base de données
- `ICurrentUserService` — utilisateur courant (FirebaseUid, Email, IsAdmin)
- `IStorageService` — upload/download R2
- `IStripeService` — création de sessions Checkout
- `IOrderNumberService` — génération des numéros de commande

### Pipeline MediatR
- `ValidationBehavior` — FluentValidation automatique
- `LoggingBehavior` — log de chaque requête

### Commands & Queries

**Catalogue**
- `CreateProductCommand`, `UpdateProductCommand`, `PublishProductCommand`, `ArchiveProductCommand`, `SetProductFileCommand`
- `GetProductsQuery`, `GetProductBySlugQuery`, `GetAdminProductsQuery`, `GetCategoriesQuery`

**Clients**
- `SyncCustomerCommand` (GetOrCreate), `UpdateCustomerProfileCommand`

**Commandes**
- `CreateOrderCommand` (vérifie les doublons de licence), `MarkOrderPaidCommand` (crée les licences auto)
- `GetMyOrdersQuery`, `GetAdminOrdersQuery`

**Licences**
- `DownloadLicenseCommand` (génère lien signé R2, incrémente compteur), `RevokeLicenseCommand`
- `GetMyLicensesQuery`

**Projets sur-mesure**
- `SubmitProjectCommand`, `SendQuoteCommand`, `StartProjectWorkCommand`, `DeliverProjectCommand`, `CancelProjectCommand`
- `GetMyProjectsQuery`, `GetAdminProjectsQuery`

---

## PR-004 — Infrastructure layer ✅

### EF Core
- `ApplicationDbContext` — toutes les DbSet
- `ApplicationDbContextFactory` — pour les migrations design-time
- Configurations : snake_case, IDs fortement typés, index uniques (slug, order_number, download_token)
- Soft delete sur Customer (`HasQueryFilter`)
- Migration `InitialCreate` générée

### Services
- `CurrentUserService` — lit les claims Firebase (`user_id`, `email`, `role`)
- `OrderNumberService` — format `TF-{année}-{count:D4}`
- `StripeService` — Stripe Checkout Sessions
- `R2StorageService` — upload + liens signés expirables (AWS S3-compatible)

---

## PR-005 — API layer ✅

### Endpoints publics
```
GET  /api/catalog                → liste produits publiés (filtres: category, type)
GET  /api/catalog/{slug}         → détail produit + médias + backtests
GET  /api/categories             → liste catégories
GET  /health                     → healthcheck
```

### Endpoints client (Firebase JWT requis)
```
POST /api/account/sync           → sync Firebase → Customer
PUT  /api/account/profile        → mise à jour profil

GET  /api/orders                 → mes commandes
POST /api/orders                 → créer commande + lien Stripe

GET  /api/licenses               → mes licences (achats)
POST /api/licenses/{id}/download → lien de téléchargement signé (1h)

GET  /api/projects               → mes projets sur-mesure
POST /api/projects               → soumettre un nouveau projet
POST /api/projects/{id}/cancel   → annuler
```

### Endpoints admin (role: admin)
```
GET/POST     /api/admin/products
PUT          /api/admin/products/{id}
POST         /api/admin/products/{id}/publish
POST         /api/admin/products/{id}/archive
POST         /api/admin/products/{id}/file      → upload fichier .pine / PDF

GET          /api/admin/orders
POST         /api/admin/licenses/{id}/revoke

GET          /api/admin/projects
POST         /api/admin/projects/{id}/quote     → envoyer devis
POST         /api/admin/projects/{id}/start     → démarrer travail
POST         /api/admin/projects/{id}/deliver   → livrer fichier
POST         /api/admin/projects/{id}/cancel
```

### Webhook
```
POST /api/webhooks/stripe        → checkout.session.completed → MarkOrderPaid → crée Licenses
```

---

## Prochaines étapes

### PR-006 — Frontend (pages publiques)
- Init Next.js 14 (TypeScript, Tailwind, App Router, Shadcn/ui)
- Layout global (navbar, footer)
- Page d'accueil `/`
- Catalogue `/catalogue` avec filtres
- Fiche produit `/catalogue/[slug]`
- Pages auth `/login` et `/register`

### PR-007 — Frontend (espace client)
- `/dashboard` — résumé
- `/dashboard/licenses` — mes achats + télécharger
- `/dashboard/orders` — historique
- `/dashboard/projects` — mes projets sur-mesure
- `/dashboard/projects/new` — formulaire soumission

### PR-008 — Frontend (admin)
- `/admin` — dashboard KPIs
- `/admin/products` — gestion catalogue
- `/admin/projects` — file projets sur-mesure

### PR-009 — Intégrations
- Stripe Checkout redirect depuis le frontend
- Upload fichiers via l'admin

### PR-010 — Déploiement
- Railway (backend .NET)
- Vercel (frontend Next.js)
- Neon (PostgreSQL)
- Variables d'environnement de production
