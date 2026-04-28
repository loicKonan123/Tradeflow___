# TradeFlow — Suivi de développement

## Légende
- ✅ Terminé
- 🚧 En cours
- ⏳ À faire
- ❌ Bloqué

---

## Phase 0 — Fondations ✅

### PR-001 — Socle technique backend & frontend
**Date :** 2026-04-28
**Statut :** ✅ Terminé

**Ce qui a été fait :**
- Initialisation du monorepo (`/api`, `/web`)
- Solution .NET 9 Clean Architecture (4 couches + 4 projets de tests)
- Domain : `Product`, `Order`, `License`, `Customer`, Value Objects (`Money`, `Currency`, `Slug`), Domain Events, `Result<T>`
- Application : CQRS MediatR, `GetProductsQuery`, `GetProductBySlugQuery`, `SyncCustomerCommand`, behaviors Validation + Logging
- Infrastructure : `ApplicationDbContext` (Npgsql/EF Core 9), `CurrentUserService`, DI
- Api : Firebase JWT, policies `AdminOnly`, controllers Catalog / Account / Admin, healthcheck `/health`, Scalar UI
- `docker-compose.yml` — PostgreSQL 16 + Redis
- Next.js 14 + TypeScript + Tailwind + Shadcn/ui + Firebase + Zustand + TanStack Query
- `.gitignore`

**Dépendances à configurer avant PR-002 :**
- [ ] Créer le projet Firebase → renseigner `Firebase:ProjectId` dans `appsettings.json`
- [ ] Démarrer Docker Desktop → `docker compose up -d`
- [ ] Créer le compte Stripe

---

## Phase 1 — MVP

### Sprint 1 — Auth & Customer

#### PR-002 — Firebase Auth frontend + middleware sync Customer ⏳
**Objectif :** L'utilisateur peut se connecter avec Google, le backend crée/sync son profil en BDD.

**Tâches :**
- [ ] `lib/auth/firebase.ts` — init Firebase SDK
- [ ] `lib/auth/AuthProvider.tsx` — contexte React
- [ ] `hooks/useAuth.ts` — hook login/logout
- [ ] Page `/connexion` — bouton "Se connecter avec Google"
- [ ] Middleware Next.js — protection des routes `/compte/*`
- [ ] Test du flow complet : login → token → `POST /api/account/sync` → Customer en BDD

---

### Sprint 2 — Catalogue

#### PR-003 — CRUD produits admin + listing public ⏳
**Objectif :** Loïc peut créer des produits, les visiteurs peuvent les voir.

**Tâches :**
- [ ] EF Core Migration initiale
- [ ] `CreateProductCommand` + validator
- [ ] `UpdateProductCommand`, `PublishProductCommand`, `ArchiveProductCommand`
- [ ] Controller admin `POST /api/admin/catalog`
- [ ] Page catalogue `/catalogue` (SSR)
- [ ] Page produit `/catalogue/[slug]` (ISR)
- [ ] `ProductCard` component
- [ ] Upload image vers Cloudflare R2

---

### Sprint 3 — Panier & Checkout

#### PR-004 — Stripe + création commande ⏳
**Objectif :** Un client peut acheter un produit.

**Tâches :**
- [ ] Zustand cart store
- [ ] `CreateOrderCommand` + calcul taxes Québec/Canada
- [ ] Stripe PaymentIntent
- [ ] Stripe Elements côté frontend
- [ ] Webhook `payment_intent.succeeded` → `MarkOrderAsPaid`
- [ ] Page checkout `/checkout`
- [ ] Page confirmation `/checkout/confirmation/[orderId]`

---

### Sprint 4 — Licensing & Livraison

#### PR-005 — Licences + back-office admin ⏳
**Objectif :** Loïc peut accorder l'accès TradingView manuellement.

**Tâches :**
- [ ] Création automatique des `License` au paiement
- [ ] Page "Mes achats" client `/compte/achats`
- [ ] Action admin "Marquer accès accordé"
- [ ] Email `LicenseAccessGranted` via Brevo
- [ ] Génération facture PDF (QuestPDF)

---

### Sprint 5 — Polish & Lancement

#### PR-006 — SEO, légal, lancement ⏳
**Objectif :** Site prêt pour la mise en production.

**Tâches :**
- [ ] Pages légales (CGV, mentions, RGPD)
- [ ] Disclaimer financier sur pages produit + checkout
- [ ] SEO metadata, sitemap.xml, robots.txt
- [ ] Newsletter Brevo double opt-in
- [ ] Tests E2E Playwright (7 scénarios critiques)
- [ ] Déploiement Azure App Service + Vercel
- [ ] Soft launch 5 produits

---

## Phase 2 — V2 (M4–M7)

### PR-007 — Avis clients ⏳
### PR-008 — Abonnement SaaS Stripe ⏳
### PR-009 — Demandes sur-mesure ⏳
### PR-010 — Marketing automation ⏳

---

## Phase 3 — V3 (M9–M18)

- Programme d'affiliation
- Paiement crypto + mobile money
- Comptes équipes / fonds
- App mobile

---

## Notes & décisions techniques

| Date | Décision | Raison |
|------|----------|--------|
| 2026-04-28 | .NET 9 au lieu de .NET 8 | Seul SDK installé sur la machine |
| 2026-04-28 | Scalar UI au lieu de Swagger | Meilleure UX, natif .NET 9 |
