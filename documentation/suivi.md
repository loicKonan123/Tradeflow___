# TradeFlow — Suivi du projet

## Statut global

| PR | Contenu | Statut | Date |
|----|---------|--------|------|
| PR-001 | Socle technique (.NET 9, Docker, .gitignore) | ✅ Terminé | 2026-04-29 |
| PR-002 | Domain layer (entités, value objects, domain events) | ✅ Terminé | 2026-04-29 |
| PR-003 | Application layer (CQRS, MediatR, FluentValidation) | ✅ Terminé | 2026-04-29 |
| PR-004 | Infrastructure (EF Core 9, migrations, Stripe, R2) | ✅ Terminé | 2026-04-29 |
| PR-005 | API layer (controllers, Program.cs, Firebase JWT) | ✅ Terminé | 2026-04-29 |
| PR-006 | Frontend — Next.js 16, Tailwind v4, pages publiques | ✅ Terminé | 2026-04-29 |
| PR-007 | Frontend — Firebase auth + espace client (dashboard) | ✅ Terminé | 2026-04-29 |
| PR-008 | Frontend — admin dashboard | ✅ Terminé | 2026-04-29 |
| PR-009 | Stripe checkout redirect + R2 upload admin | ✅ Terminé | 2026-04-29 |
| PR-010 | Déploiement Railway + Vercel + Neon | ⏳ À faire | — |

---

## PR-001 — Socle technique ✅

### Ce qui a été fait
- Solution .NET 9 avec 4 projets : Domain, Application, Infrastructure, Api
- Références entre projets configurées
- Packages NuGet : MediatR 12, FluentValidation, EF Core 9, Npgsql 9, Stripe.net 47, AWSSDK.S3 3.7, JwtBearer, Swashbuckle 6.9
- `docker-compose.yml` : PostgreSQL 16-alpine
- `.gitignore` complet (bin/, obj/, .env, secrets, node_modules)
- `appsettings.json` avec toutes les clés de configuration

---

## PR-002 — Domain layer ✅

### Entités

| Entité | Namespace | Description |
|--------|-----------|-------------|
| `Category` | `Domain.Catalog` | Catégorie de produits |
| `Product` | `Domain.Catalog` | Script Pine Script — Publish() valide FileUrl |
| `ProductMedia` | `Domain.Catalog` | Images/vidéos d'un produit |
| `BacktestReport` | `Domain.Catalog` | WinRate, MaxDrawdown, ProfitFactor |
| `Customer` | `Domain.Customers` | Utilisateur Firebase (soft delete) |
| `Order` | `Domain.Orders` | Commande — MarkAsPaid() → OrderPaidEvent |
| `OrderItem` | `Domain.Orders` | Ligne de commande |
| `License` | `Domain.Licensing` | Token unique, MaxDownloads=5, RecordDownload() |
| `CustomProjectRequest` | `Domain.Projects` | SendQuote() calcule dépôt 50%, Deliver() → event |

### Value objects & patterns
- `Slug` — `FromTitle()` slugification automatique, retourne `Result<Slug>`
- IDs fortement typés (`ProductId`, `OrderId`, etc.) avec `New()` et `From(Guid)`
- `Result<T>` — gestion d'erreurs sans exceptions
- `AggregateRoot<TId>` + domain events via `RaiseDomainEvent()`

### Domain events
- `ProductPublishedEvent`, `OrderPaidEvent`, `ProjectQuotedEvent`, `ProjectDeliveredEvent`

---

## PR-003 — Application layer ✅

### Interfaces
- `IApplicationDbContext` — DbSet + SaveChangesAsync
- `ICurrentUserService` — FirebaseUid, Email, IsAdmin
- `IStorageService` — UploadFileAsync, GenerateSignedDownloadUrlAsync, DeleteFileAsync
- `IStripeService` — CreateCheckoutSessionAsync
- `IOrderNumberService` — GenerateAsync

### Pipeline MediatR
- `ValidationBehavior` — FluentValidation automatique sur toutes les commandes
- `LoggingBehavior` — log durée de chaque requête

### Commands & Queries (résumé)

**Catalogue** : Create, Update, Publish, Archive, SetFile / GetProducts, GetBySlug, GetAdmin, GetCategories

**Clients** : SyncCustomer (GetOrCreate), UpdateProfile

**Commandes** : CreateOrder (vérifie doublons), MarkOrderPaid (crée licences auto) / GetMyOrders, GetAdminOrders

**Licences** : Download (génère signed URL, incrémente compteur), Revoke / GetMyLicenses

**Projets** : Submit, SendQuote, StartWork, Deliver, Cancel / GetMyProjects, GetAdminProjects

---

## PR-004 — Infrastructure layer ✅

### EF Core
- `ApplicationDbContext` — toutes les DbSet avec configurations
- Snake_case naming convention (Npgsql)
- HasConversion pour tous les IDs fortement typés
- Index uniques : `slug`, `order_number`, `download_token`
- Soft delete Customer via `HasQueryFilter`
- Migration `20260429151154_InitialCreate` générée et appliquée

### Services
- `CurrentUserService` — lit claims Firebase (`user_id`, `email`, `role`)
- `OrderNumberService` — format `TF-{année}-{count:D4}`
- `StripeService` — Checkout Sessions avec metadata `order_id` et `order_number`
- `R2StorageService` — ServiceURL `https://{accountId}.r2.cloudflarestorage.com`, ForcePathStyle=true, PreSigned URL (non-async, Task.FromResult)

---

## PR-005 — API layer ✅

### Endpoints (voir `documentation/api.md` pour le détail complet)

| Groupe | Routes |
|--------|--------|
| Public | `GET /api/catalog`, `GET /api/catalog/{slug}`, `GET /api/categories`, `GET /health` |
| Client | `POST /api/account/sync`, `PUT /api/account/profile`, CRUD orders/licenses/projects |
| Admin | CRUD produits + upload fichier, gestion commandes/licences/projets |
| Webhook | `POST /api/webhooks/stripe` |

### `Program.cs`
- Firebase JWT Bearer (Authority = `securetoken.google.com/{projectId}`)
- Policy `AdminOnly` : claim `role == "admin"`
- Swagger avec Bearer security definition (Swashbuckle 6.9)
- CORS depuis config (`Cors:AllowedOrigins`)

---

## PR-006 — Frontend pages publiques ✅

### Stack
- Next.js 16.2.4, TypeScript, Tailwind v4 (configuration via `@theme` CSS, pas de `tailwind.config.ts`)
- Polices : Space Grotesk + Inter via `next/font/google`
- Design system : Kinetic Precision (dark navy, glassmorphism, neon vert)

### Fichiers créés
- `globals.css` — palette complète via `@theme`, `.glass-panel`, `.glass-card`, `.neon-border`, `.trading-bg`
- `layout.tsx` — root layout avec `AuthProvider`, fonts, métadonnées FR
- `components/Navbar.tsx` — sticky, lien actif souligné, boutons Login / Get Started
- `components/Footer.tsx` — 3 colonnes (Solutions, Compagnie, Légal)

### Pages
| Route | Description |
|-------|-------------|
| `/` | Hero (chart SVG + profit card), trust banner, bento services, produits vedettes, quality protocol, CTA |
| `/catalogue` | Grille 4 colonnes, filtre par catégorie, recherche live, product cards avec stats |
| `/sur-mesure` | 3 étapes + formulaire brief (non-connecté) |
| `/auth/login` | Formulaire (non connecté à Firebase à ce stade) |
| `/auth/register` | Formulaire + validation mot de passe |

---

## PR-007 — Frontend espace client ✅

### Firebase auth
- `lib/firebase.ts` — initialisation **lazy** (pas de crash SSR car pas d'appel au module level)
- `lib/api.ts` — fetch helper avec `Authorization: Bearer <token>` automatique, tous les types API
- `contexts/AuthContext.tsx` — `login`, `register`, `logout`, `user`, `loading`

### Pages login/register
- Branchées à Firebase (`signInWithEmailAndPassword`, `createUserWithEmailAndPassword`)
- Redirect vers `/dashboard` après succès
- Messages d'erreur Firebase traduits (email existant, mot de passe faible)

### Dashboard (route group `(dashboard)`, auth guard via `useEffect`)
| Route | Description |
|-------|-------------|
| `/dashboard` | Stats (licences actives, commandes, projets en cours), aperçu récent |
| `/dashboard/licenses` | Liste + barre progression téléchargements + bouton ↓ Télécharger (signed URL R2) |
| `/dashboard/orders` | Historique commandes avec statuts colorés et détail articles |
| `/dashboard/projects` | Liste avec stepper 5 étapes, notes admin, bouton annuler |
| `/dashboard/projects/new` | Formulaire complet (marché, timeframe, entrée, sortie, risk, notes) |

---

## PR-008 — Admin dashboard ✅

### Pages
| Route | Description |
|-------|-------------|
| `/admin` | KPIs (CA total, commandes payées, projets actifs, licences actives) |
| `/admin/products` | Liste paginée, filtres statut, créer produit (modal), publier/archiver, upload fichier |
| `/admin/projects` | File de projets avec modals devis (prix + dépôt auto 50%) et livraison (fichier + notes) |
| `/admin/orders` | Toutes les commandes avec statuts et montants |

### Fonctionnalités
- Auth guard (rôle `admin` vérifié côté frontend via Firebase claims)
- Upload multipart vers R2 (`FormData`, bypass du helper JSON)
- Preview prix dépôt en temps réel dans le modal devis
- Actions : `sendQuote`, `startProjectWork`, `deliverProject`, `publishProduct`, `archiveProduct`, `revokeLicense`

---

## PR-009 — Stripe Checkout + R2 Download ✅

### Fichiers modifiés
- `lib/api.ts` — `publicGet()`, `ProductDetail`, `getProduct(slug)`, `syncAccount()`
- `(public)/catalogue/[slug]/page.tsx` — page détail produit avec buy flow complet
- `components/Navbar.tsx` — auth-aware (Dashboard/Déconnexion vs Login/Get Started)
- `auth/login/page.tsx` — support `?redirect=` param, Suspense boundary
- `(dashboard)/dashboard/orders/page.tsx` — bannières success/cancel Stripe, Suspense boundary

### Flow d'achat
1. `/catalogue/[slug]` : bouton "Acheter maintenant" → si non connecté, redirect `/auth/login?redirect=/catalogue/{slug}`
2. Après login : `POST /api/account/sync` → `POST /api/orders` → `window.location.href = checkoutUrl`
3. Stripe redirige vers `/dashboard/orders?success=true` ou `?cancelled=true`
4. Bannière dismissible affichée, lien vers licences en cas de succès

---

## Prochaines étapes

### PR-010 — Déploiement
- Railway (backend .NET 9)
- Vercel (frontend Next.js)
- Neon (PostgreSQL)
- Variables d'environnement de production
