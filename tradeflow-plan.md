# TradeFlow — Plan de projet exhaustif

> **Studio de développement Pine Script & marketplace de stratégies de trading validées manuellement.**
> Stack : .NET 8 (Clean Architecture) · Firebase Auth · Next.js 14 · PostgreSQL · Stripe.

---

## Table des matières

1. [Vision & positionnement](#1-vision--positionnement)
2. [Architecture globale du système](#2-architecture-globale-du-système)
3. [Architecture backend — Clean Architecture .NET](#3-architecture-backend--clean-architecture-net)
4. [Modèle de domaine](#4-modèle-de-domaine)
5. [Schéma de base de données](#5-schéma-de-base-de-données)
6. [Authentification Firebase ↔ .NET](#6-authentification-firebase--net)
7. [Architecture frontend Next.js](#7-architecture-frontend-nextjs)
8. [Intégrations externes](#8-intégrations-externes)
9. [Sécurité & conformité](#9-sécurité--conformité)
10. [DevOps, hébergement & CI/CD](#10-devops-hébergement--cicd)
11. [Tests & qualité](#11-tests--qualité)
12. [Roadmap de développement par phase](#12-roadmap-de-développement-par-phase)
13. [Annexes & checklists](#13-annexes--checklists)

---

## 1. Vision & positionnement

### 1.1 Pitch

TradeFlow est un studio de développement spécialisé en Pine Script v5 qui combine une **bibliothèque de stratégies validées manuellement** (revenus passifs) et un **service de développement sur-mesure** (revenus actifs premium). Le produit s'oppose volontairement aux générateurs IA de Pine Script en mettant en avant la **rigueur de validation humaine** comme garantie de qualité.

### 1.2 Proposition de valeur

| Pour le client | TradeFlow apporte |
|----------------|-------------------|
| Trader débutant | Indicateurs prêts à l'emploi avec documentation et vidéos |
| Trader intermédiaire | Stratégies backtestées sur plusieurs marchés et timeframes |
| Trader avancé | Développement sur-mesure encadré par un protocole de validation à 4 étapes |
| Petit fonds / trading group | Audits, optimisations et licences à durée définie |

### 1.3 Modèle de revenus

- **Couche 1 — Produits scalables** : scripts/indicateurs en achat unique (29–149 €)
- **Couche 2 — Sur-mesure encadré** : forfaits à scope fixe (199–499 €)
- **Couche 3 — Conseil/audit** : facturation horaire (80–120 €/h)
- **Couche SaaS (V2)** : abonnement bibliothèque (19 €/mois)

### 1.4 Cibles & métriques de succès

- **MVP (M3)** : 5 produits en catalogue, 10 ventes, 1 000 € de CA cumulé
- **V2 (M9)** : 20 produits, 50 abonnés actifs, 5 000 €/mois récurrents
- **V3 (M18)** : 50 produits, programme d'affiliation actif, 15 000 €/mois

---

## 2. Architecture globale du système

### 2.1 Vue macro

```
┌────────────────────────────────────────────────────────────────────┐
│                       UTILISATEURS                                 │
│   Visiteurs publics · Clients authentifiés · Admin (Loïc)         │
└────────────────────┬───────────────────────────────────────────────┘
                     │
        ┌────────────┴────────────┐
        │                         │
┌───────▼─────────┐      ┌────────▼──────────┐
│  Next.js 14     │      │  Next.js Admin    │
│  (App Router)   │      │  (Back-office)    │
│  Vercel         │      │  Vercel (sous-    │
│                 │      │  domaine privé)   │
└───────┬─────────┘      └────────┬──────────┘
        │                         │
        │   HTTPS + JWT Firebase  │
        └────────────┬────────────┘
                     │
              ┌──────▼──────┐
              │  API .NET 8 │
              │  Clean Arch │
              │  Azure App  │
              │  Service    │
              └──┬───────┬──┘
                 │       │
       ┌─────────▼─┐   ┌─▼──────────────────┐
       │PostgreSQL │   │  Services externes  │
       │ Azure     │   │  - Firebase Auth    │
       │ Flexible  │   │  - Stripe           │
       │           │   │  - SendGrid/Brevo   │
       └───────────┘   │  - Cloudflare R2    │
                       │  - TradingView      │
                       └────────────────────┘
```

### 2.2 Choix technologiques justifiés

| Couche | Technologie | Justification |
|--------|-------------|---------------|
| Frontend | Next.js 14 App Router | SEO critique pour acquisition organique, SSR pour les pages produits, écosystème React mature |
| Backend | .NET 8 + ASP.NET Core | Maîtrise existante, performance, écosystème Clean Architecture solide |
| Base de données | PostgreSQL 16 | Open-source, JSONB pour les métadonnées flexibles, full-text search natif, support multi-devise via NUMERIC |
| Auth | Firebase Auth | OAuth Google natif, gratuit jusqu'à 50k MAU, validation JWT côté .NET |
| Paiement | Stripe | Standard du marché, factures auto, multi-devises, conformité PCI |
| Stockage fichiers | Cloudflare R2 | Coût bas (vs S3), pas d'egress fees, idéal pour PDF/vidéos |
| Email | Brevo (ex-Sendinblue) | Gratuit jusqu'à 300/jour, API simple, basé en UE (RGPD) |
| Cache | Redis (Azure Cache) | Sessions, rate limiting, cache catalogue produits |
| Hébergement API | Azure App Service | Intégration .NET native, scaling vertical simple |
| CI/CD | GitHub Actions | Gratuit pour repos publics/petits projets, déjà maîtrisé |
| Monitoring | Sentry + Application Insights | Erreurs frontend + backend, traces distribuées |

### 2.3 Communications inter-services

- **Frontend ↔ API** : REST/JSON sur HTTPS, authentification par JWT Firebase dans header `Authorization: Bearer`
- **API ↔ Stripe** : webhooks signés (vérification HMAC) pour événements de paiement
- **API ↔ Email** : appels API Brevo asynchrones via `IBackgroundJobService` (Hangfire)
- **API ↔ Stockage** : SAS tokens signés à durée limitée pour télécharger les PDF/vidéos protégés

---

## 3. Architecture backend — Clean Architecture .NET

### 3.1 Arborescence des projets

```
TradeFlow.sln
│
├── src/
│   ├── TradeFlow.Domain/              # Cœur métier — aucune dépendance
│   │   ├── Common/                    # BaseEntity, AggregateRoot, ValueObject
│   │   ├── Catalog/                   # Product, Category, Tag, ProductVariant
│   │   ├── Orders/                    # Order, OrderItem, OrderStatus
│   │   ├── Customers/                 # Customer, CustomerProfile
│   │   ├── Licensing/                 # License, AccessGrant, TradingViewUsername
│   │   ├── Pricing/                   # Money, Currency, TaxRate, Discount
│   │   ├── Reviews/                   # Review, Rating
│   │   ├── Subscriptions/             # Subscription, Plan, BillingCycle (V2)
│   │   ├── CustomProjects/            # ProjectRequest, Quote, Milestone (V2)
│   │   ├── Events/                    # Domain events (OrderPlaced, etc.)
│   │   └── Exceptions/                # Domain-specific exceptions
│   │
│   ├── TradeFlow.Application/         # Use cases, orchestration
│   │   ├── Common/
│   │   │   ├── Behaviors/             # MediatR pipeline (validation, logging, txn)
│   │   │   ├── Interfaces/            # Ports vers l'infrastructure
│   │   │   ├── Mappings/              # AutoMapper profiles
│   │   │   └── Models/                # DTOs partagés, Result<T>
│   │   ├── Catalog/
│   │   │   ├── Commands/              # CreateProduct, UpdatePrice, etc.
│   │   │   ├── Queries/               # GetProductBySlug, ListProducts
│   │   │   └── Validators/            # FluentValidation
│   │   ├── Orders/
│   │   ├── Customers/
│   │   ├── Licensing/
│   │   ├── Payments/                  # Use cases de paiement
│   │   ├── Notifications/             # Use cases d'envoi d'email
│   │   ├── Subscriptions/             # (V2)
│   │   └── CustomProjects/            # (V2)
│   │
│   ├── TradeFlow.Infrastructure/      # Implémentations techniques
│   │   ├── Persistence/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   ├── Configurations/        # IEntityTypeConfiguration<T>
│   │   │   ├── Repositories/          # Concrete repos
│   │   │   ├── Migrations/
│   │   │   └── Seed/
│   │   ├── Identity/                  # FirebaseAuthService, JWT validation
│   │   ├── Payments/                  # StripeService, webhook handlers
│   │   ├── Email/                     # BrevoEmailService, templates
│   │   ├── Storage/                   # CloudflareR2Service
│   │   ├── BackgroundJobs/            # Hangfire setup
│   │   ├── Caching/                   # Redis cache decorators
│   │   └── DependencyInjection.cs
│   │
│   ├── TradeFlow.Api/                 # ASP.NET Core Web API
│   │   ├── Controllers/               # Endpoints REST
│   │   │   ├── Public/                # Catalogue public (sans auth)
│   │   │   ├── Customer/              # Espace client (auth requise)
│   │   │   ├── Admin/                 # Back-office (auth + rôle Admin)
│   │   │   └── Webhooks/              # Stripe, Firebase
│   │   ├── Middlewares/               # ErrorHandling, RequestLogging
│   │   ├── Filters/                   # FirebaseAuthFilter, AdminOnlyFilter
│   │   ├── Extensions/                # ServiceCollection, WebApp
│   │   ├── appsettings.json
│   │   └── Program.cs
│   │
│   └── TradeFlow.Worker/              # Service de jobs background (V2)
│       └── Program.cs                 # Hangfire server pour traitements lourds
│
└── tests/
    ├── TradeFlow.Domain.UnitTests/
    ├── TradeFlow.Application.UnitTests/
    ├── TradeFlow.Infrastructure.IntegrationTests/
    └── TradeFlow.Api.FunctionalTests/
```

### 3.2 Règles de dépendance Clean Architecture

```
   Api ──► Application ──► Domain
    │           ▲             ▲
    │           │             │
    └────► Infrastructure ────┘
```

- **Domain** ne dépend de rien (POCO uniquement)
- **Application** dépend uniquement du Domain (et définit les interfaces qu'Infrastructure implémente)
- **Infrastructure** dépend de Application et Domain
- **Api** dépend de Application et Infrastructure (pour DI)

### 3.3 Patterns appliqués

- **CQRS via MediatR** : séparation Commands (mutations) et Queries (lectures)
- **Repository pattern + Unit of Work** : abstraction de la persistance
- **Specification pattern** : pour les requêtes complexes du catalogue
- **Domain Events** : `OrderPlaced`, `LicenseGranted`, `PaymentFailed`, dispatchés via MediatR
- **Result pattern** : retour `Result<T>` ou `Result` plutôt qu'exceptions pour les erreurs métier prévisibles
- **Pipeline behaviors** : validation FluentValidation, logging, gestion transactionnelle automatique

### 3.4 Pipeline MediatR

Chaque commande passe par les comportements suivants dans l'ordre :

1. `LoggingBehavior` — log de la requête et du résultat
2. `ValidationBehavior` — validation FluentValidation, retourne `Result.Failure` si invalide
3. `AuthorizationBehavior` — vérification des permissions (rôles, ownership)
4. `TransactionBehavior` — wrap en transaction DB pour les commandes
5. `PerformanceBehavior` — alerte si requête > 500ms

---

## 4. Modèle de domaine

### 4.1 Aggregate : Product (Catalog)

```csharp
public class Product : AggregateRoot<ProductId>
{
    public Slug Slug { get; private set; }
    public string Title { get; private set; }
    public string ShortDescription { get; private set; }
    public string LongDescriptionMarkdown { get; private set; }
    public ProductType Type { get; private set; }        // Indicator, Strategy, Bundle
    public Money BasePrice { get; private set; }
    public ProductStatus Status { get; private set; }    // Draft, Published, Archived
    public CategoryId CategoryId { get; private set; }
    public List<Tag> Tags { get; private set; }
    public List<ProductMedia> Media { get; private set; }
    public ProductTechnicalSpec TechnicalSpec { get; private set; }
    public BacktestReport BacktestReport { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PublishedAt { get; private set; }

    // Méthodes métier
    public Result Publish() { /* ... */ }
    public Result Archive() { /* ... */ }
    public Result UpdatePrice(Money newPrice) { /* ... */ }
    public Result AddBacktestReport(BacktestReport report) { /* ... */ }
}
```

### 4.2 Aggregate : Order

```csharp
public class Order : AggregateRoot<OrderId>
{
    public OrderNumber Number { get; private set; }       // ex: TF-2026-0042
    public CustomerId CustomerId { get; private set; }
    public List<OrderItem> Items { get; private set; }
    public Money Subtotal { get; private set; }
    public Money TaxAmount { get; private set; }
    public Money DiscountAmount { get; private set; }
    public Money Total { get; private set; }
    public Currency Currency { get; private set; }
    public OrderStatus Status { get; private set; }
    public PaymentStatus PaymentStatus { get; private set; }
    public TradingViewUsername TradingViewUsername { get; private set; }
    public CustomerSnapshot BillingSnapshot { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? FulfilledAt { get; private set; }

    public Result MarkAsPaid(string stripePaymentIntentId) { /* déclenche OrderPaidEvent */ }
    public Result MarkAsFulfilled() { /* déclenche OrderFulfilledEvent */ }
    public Result Refund(Money amount, string reason) { /* ... */ }
}
```

### 4.3 Aggregate : License

Représente le droit d'accès d'un client à un produit. Distinct de Order pour permettre les transferts, révocations, et licences à durée.

```csharp
public class License : AggregateRoot<LicenseId>
{
    public CustomerId CustomerId { get; private set; }
    public ProductId ProductId { get; private set; }
    public OrderId SourceOrderId { get; private set; }
    public TradingViewUsername GrantedTo { get; private set; }
    public LicenseType Type { get; private set; }        // Lifetime, TimeLimited
    public DateTime IssuedAt { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public DateTime? AccessGrantedAt { get; private set; }
    public LicenseStatus Status { get; private set; }    // Pending, Active, Revoked, Expired

    public Result MarkAccessGranted() { /* déclenche AccessGrantedEvent → email */ }
    public Result Revoke(string reason) { /* ... */ }
    public Result Renew(DateTime newExpiry) { /* ... */ }
}
```

### 4.4 Value Objects critiques

```csharp
public sealed record Money(decimal Amount, Currency Currency)
{
    public Money Add(Money other) { /* check même currency */ }
    public Money ApplyTaxRate(decimal rate) { /* ... */ }
    public Money ApplyDiscount(decimal percentage) { /* ... */ }
}

public sealed record Currency(string Code)  // EUR, USD, XOF, CAD
{
    public static readonly Currency EUR = new("EUR");
    public static readonly Currency USD = new("USD");
    // ...
}

public sealed record TradingViewUsername(string Value)
{
    public static Result<TradingViewUsername> Create(string input)
    {
        // Validation : 3-30 caractères, alphanumérique + underscore
    }
}

public sealed record Slug(string Value)
{
    public static Result<Slug> FromTitle(string title) { /* slugify */ }
}
```

### 4.5 Domain Events

| Événement | Déclencheur | Conséquences |
|-----------|-------------|--------------|
| `OrderPlacedEvent` | Création commande | Création licences en statut Pending, email confirmation |
| `OrderPaidEvent` | Webhook Stripe `payment_intent.succeeded` | Mise à jour statut, email facture |
| `LicenseAccessGrantedEvent` | Action admin | Email au client avec instructions TradingView |
| `RefundIssuedEvent` | Action admin | Révocation des licences associées, email |
| `SubscriptionRenewedEvent` (V2) | Webhook Stripe `invoice.paid` | Extension de l'accès à la bibliothèque |
| `CustomProjectQuoteAcceptedEvent` (V2) | Action client | Création projet, demande d'acompte |

---

## 5. Schéma de base de données

### 5.1 Diagramme conceptuel (tables principales)

```
customers ──────┐
    │           │
    │           │  (1,N)
    │           ▼
    │       orders ──────► order_items ──────► products
    │           │                                 ▲
    │           │ (1,N)                           │
    │           ▼                                 │
    │       payments                              │
    │                                             │
    └──► licenses ────────────────────────────────┘
            │
            │ (1,1)
            ▼
        license_access_logs

products ──► product_media (1,N)
        ──► product_categories (N,1)
        ──► product_tags (N,N via junction)
        ──► product_backtest_reports (1,1)
        ──► reviews (1,N)
```

### 5.2 Tables principales (DDL simplifié)

```sql
-- Customers (lien avec Firebase Auth via firebase_uid)
CREATE TABLE customers (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    firebase_uid VARCHAR(128) UNIQUE NOT NULL,
    email VARCHAR(255) UNIQUE NOT NULL,
    display_name VARCHAR(100),
    country_code CHAR(2),
    preferred_currency CHAR(3) DEFAULT 'EUR',
    tradingview_username VARCHAR(30),
    role VARCHAR(20) DEFAULT 'customer',  -- customer, admin
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    deleted_at TIMESTAMPTZ
);
CREATE INDEX idx_customers_firebase_uid ON customers(firebase_uid);
CREATE INDEX idx_customers_email ON customers(email);

-- Categories
CREATE TABLE categories (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    slug VARCHAR(100) UNIQUE NOT NULL,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    display_order INT DEFAULT 0
);

-- Products
CREATE TABLE products (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    slug VARCHAR(150) UNIQUE NOT NULL,
    title VARCHAR(200) NOT NULL,
    short_description VARCHAR(500),
    long_description_md TEXT,
    type VARCHAR(30) NOT NULL,  -- indicator, strategy, bundle
    category_id UUID REFERENCES categories(id),
    base_price_amount NUMERIC(12,2) NOT NULL,
    base_price_currency CHAR(3) NOT NULL,
    status VARCHAR(20) DEFAULT 'draft',  -- draft, published, archived
    technical_spec JSONB,                 -- timeframes, markets, version Pine
    sales_count INT DEFAULT 0,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    updated_at TIMESTAMPTZ DEFAULT NOW(),
    published_at TIMESTAMPTZ
);
CREATE INDEX idx_products_status ON products(status);
CREATE INDEX idx_products_category ON products(category_id);
CREATE INDEX idx_products_search ON products USING GIN (to_tsvector('french', title || ' ' || short_description));

-- Tags (N,N)
CREATE TABLE tags (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    slug VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(50) NOT NULL
);
CREATE TABLE product_tags (
    product_id UUID REFERENCES products(id) ON DELETE CASCADE,
    tag_id UUID REFERENCES tags(id) ON DELETE CASCADE,
    PRIMARY KEY (product_id, tag_id)
);

-- Médias produits
CREATE TABLE product_media (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID REFERENCES products(id) ON DELETE CASCADE,
    type VARCHAR(20),  -- screenshot, video, pdf
    url VARCHAR(500),
    display_order INT DEFAULT 0,
    is_protected BOOLEAN DEFAULT FALSE  -- nécessite licence pour télécharger
);

-- Backtest reports
CREATE TABLE product_backtest_reports (
    product_id UUID PRIMARY KEY REFERENCES products(id) ON DELETE CASCADE,
    report_data JSONB NOT NULL,  -- { markets: [], timeframes: [], metrics: {} }
    pdf_url VARCHAR(500),
    generated_at TIMESTAMPTZ
);

-- Orders
CREATE TABLE orders (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_number VARCHAR(20) UNIQUE NOT NULL,  -- TF-2026-0042
    customer_id UUID REFERENCES customers(id),
    status VARCHAR(30) NOT NULL,        -- pending, paid, fulfilled, refunded, cancelled
    payment_status VARCHAR(30) NOT NULL,
    subtotal_amount NUMERIC(12,2) NOT NULL,
    tax_amount NUMERIC(12,2) NOT NULL,
    discount_amount NUMERIC(12,2) DEFAULT 0,
    total_amount NUMERIC(12,2) NOT NULL,
    currency CHAR(3) NOT NULL,
    tradingview_username VARCHAR(30) NOT NULL,
    billing_snapshot JSONB,             -- snapshot des infos facturation
    stripe_payment_intent_id VARCHAR(100),
    discount_code VARCHAR(50),
    notes TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW(),
    paid_at TIMESTAMPTZ,
    fulfilled_at TIMESTAMPTZ
);
CREATE INDEX idx_orders_customer ON orders(customer_id);
CREATE INDEX idx_orders_status ON orders(status);
CREATE INDEX idx_orders_created ON orders(created_at DESC);

-- Order items
CREATE TABLE order_items (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id UUID REFERENCES orders(id) ON DELETE CASCADE,
    product_id UUID REFERENCES products(id),
    product_snapshot JSONB,             -- snapshot pour archivage
    unit_price NUMERIC(12,2) NOT NULL,
    quantity INT DEFAULT 1,
    line_total NUMERIC(12,2) NOT NULL
);

-- Licenses
CREATE TABLE licenses (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID REFERENCES customers(id),
    product_id UUID REFERENCES products(id),
    source_order_id UUID REFERENCES orders(id),
    granted_to_tv_username VARCHAR(30) NOT NULL,
    type VARCHAR(20) NOT NULL,           -- lifetime, time_limited
    status VARCHAR(20) NOT NULL,         -- pending, active, revoked, expired
    issued_at TIMESTAMPTZ DEFAULT NOW(),
    expires_at TIMESTAMPTZ,
    access_granted_at TIMESTAMPTZ,
    revoked_at TIMESTAMPTZ,
    revocation_reason TEXT
);
CREATE INDEX idx_licenses_customer ON licenses(customer_id);
CREATE INDEX idx_licenses_status ON licenses(status);

-- Discount codes
CREATE TABLE discount_codes (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(50) UNIQUE NOT NULL,
    type VARCHAR(20),                    -- percentage, fixed
    value NUMERIC(10,2),
    currency CHAR(3),
    max_uses INT,
    used_count INT DEFAULT 0,
    valid_from TIMESTAMPTZ,
    valid_until TIMESTAMPTZ,
    applies_to_product_ids UUID[],
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Reviews (V2)
CREATE TABLE reviews (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    product_id UUID REFERENCES products(id),
    customer_id UUID REFERENCES customers(id),
    rating SMALLINT CHECK (rating BETWEEN 1 AND 5),
    title VARCHAR(200),
    body TEXT,
    is_verified_purchase BOOLEAN DEFAULT FALSE,
    is_approved BOOLEAN DEFAULT FALSE,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Subscriptions (V2)
CREATE TABLE subscription_plans (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    code VARCHAR(50) UNIQUE NOT NULL,
    name VARCHAR(100) NOT NULL,
    price_monthly NUMERIC(10,2),
    price_yearly NUMERIC(10,2),
    currency CHAR(3),
    features JSONB,
    stripe_price_id_monthly VARCHAR(100),
    stripe_price_id_yearly VARCHAR(100),
    is_active BOOLEAN DEFAULT TRUE
);

CREATE TABLE subscriptions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID REFERENCES customers(id),
    plan_id UUID REFERENCES subscription_plans(id),
    status VARCHAR(30),                  -- active, past_due, cancelled
    billing_cycle VARCHAR(10),           -- monthly, yearly
    stripe_subscription_id VARCHAR(100),
    current_period_start TIMESTAMPTZ,
    current_period_end TIMESTAMPTZ,
    cancelled_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Custom projects (V2)
CREATE TABLE custom_project_requests (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id UUID REFERENCES customers(id),
    title VARCHAR(200) NOT NULL,
    description TEXT,
    requirements JSONB,                  -- réponses au formulaire structuré
    status VARCHAR(30),                  -- submitted, quoted, accepted, in_progress, delivered
    quoted_amount NUMERIC(12,2),
    quoted_currency CHAR(3),
    quoted_deadline_days INT,
    deposit_paid_at TIMESTAMPTZ,
    final_paid_at TIMESTAMPTZ,
    delivered_at TIMESTAMPTZ,
    created_at TIMESTAMPTZ DEFAULT NOW()
);

-- Audit log (sécurité)
CREATE TABLE audit_logs (
    id BIGSERIAL PRIMARY KEY,
    actor_customer_id UUID,
    actor_role VARCHAR(20),
    action VARCHAR(100) NOT NULL,        -- license.granted, order.refunded
    target_entity VARCHAR(50),
    target_id UUID,
    payload JSONB,
    ip_address INET,
    user_agent TEXT,
    created_at TIMESTAMPTZ DEFAULT NOW()
);
CREATE INDEX idx_audit_logs_actor ON audit_logs(actor_customer_id);
CREATE INDEX idx_audit_logs_action ON audit_logs(action);
```

### 5.3 Stratégie de migration

- **EF Core Migrations** pour le schéma versionné
- Migration initiale créée à partir du modèle Domain via `dotnet ef migrations add Initial`
- Données de seed (catégories, tags de base, produit de démo) dans `ApplicationDbContextSeed`
- Backup automatique quotidien Azure PostgreSQL Flexible Server

---

## 6. Authentification Firebase ↔ .NET

### 6.1 Flux d'authentification

```
1. Utilisateur clique "Se connecter avec Google"
   └─► Frontend Next.js appelle Firebase Auth SDK
       └─► Redirection Google OAuth → callback Firebase
           └─► Firebase retourne un ID Token JWT (durée 1h)

2. Frontend stocke le token (mémoire + refresh via Firebase SDK)

3. Pour chaque appel API :
   └─► Frontend ajoute header: Authorization: Bearer <ID_TOKEN>

4. Backend .NET reçoit la requête
   └─► Middleware FirebaseAuthMiddleware
       ├─► Valide le JWT avec les clés publiques Firebase (cache 1h)
       ├─► Vérifie issuer = https://securetoken.google.com/{project-id}
       ├─► Vérifie audience = {project-id}
       ├─► Vérifie expiration
       └─► Extrait firebase_uid

5. Récupération/création du Customer en BDD
   └─► CustomerSyncService.GetOrCreateAsync(firebaseUid, email, name)
       ├─► Si Customer existe → enrichit HttpContext.User
       └─► Sinon → crée Customer en BDD + enrichit HttpContext.User

6. Endpoint exécuté avec [Authorize] et claims disponibles
```

### 6.2 Implémentation .NET (extrait clé)

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var firebaseProjectId = builder.Configuration["Firebase:ProjectId"];
        options.Authority = $"https://securetoken.google.com/{firebaseProjectId}";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = $"https://securetoken.google.com/{firebaseProjectId}",
            ValidateAudience = true,
            ValidAudience = firebaseProjectId,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireAssertion(ctx =>
            ctx.User.HasClaim(c => c.Type == "role" && c.Value == "admin")));
});

// Middleware de synchronisation Customer
app.UseMiddleware<CustomerSyncMiddleware>();
```

### 6.3 Gestion des rôles

- Le rôle `admin` est défini en BDD (table `customers.role`)
- Au moment de la synchronisation, un **custom claim** Firebase est ajouté via Firebase Admin SDK
- Les claims sont rafraîchis automatiquement à chaque renouvellement de token
- Initialement, seul Loïc a le rôle admin (assigné manuellement en BDD)

### 6.4 Sécurité du token

- Token de courte durée (1h) — le SDK Firebase frontend gère le refresh automatique
- Pas de stockage en localStorage côté client (vulnérable XSS) — utilisation de cookies httpOnly secure pour les apps natives, mémoire React pour le web
- Rate limiting sur les endpoints d'auth pour éviter le brute force
- Logs de toutes les actions sensibles dans `audit_logs`

---

## 7. Architecture frontend Next.js

### 7.1 Arborescence Next.js 14 App Router

```
tradeflow-web/
├── app/
│   ├── (public)/                       # Layout public
│   │   ├── page.tsx                    # Landing page
│   │   ├── catalogue/
│   │   │   ├── page.tsx                # Liste produits avec filtres
│   │   │   └── [slug]/
│   │   │       └── page.tsx            # Page produit (SSR + ISR)
│   │   ├── categories/[slug]/page.tsx
│   │   ├── stratégies-gratuites/page.tsx
│   │   ├── à-propos/page.tsx
│   │   ├── blog/                       # MDX articles SEO
│   │   └── contact/page.tsx
│   │
│   ├── (auth)/
│   │   ├── connexion/page.tsx
│   │   └── inscription/page.tsx
│   │
│   ├── (account)/                      # Layout client authentifié
│   │   ├── compte/
│   │   │   ├── page.tsx                # Dashboard client
│   │   │   ├── achats/page.tsx         # Mes achats + statut licences
│   │   │   ├── factures/page.tsx
│   │   │   ├── profil/page.tsx
│   │   │   └── support/page.tsx
│   │   └── checkout/
│   │       ├── page.tsx                # Récap + paiement
│   │       └── confirmation/[orderId]/page.tsx
│   │
│   ├── (admin)/                        # Sous-domaine admin.tradeflow.io
│   │   ├── layout.tsx                  # Vérifie role admin
│   │   ├── tableau-de-bord/page.tsx
│   │   ├── produits/
│   │   ├── commandes/
│   │   ├── clients/
│   │   ├── licences/
│   │   └── parametres/
│   │
│   ├── api/                            # Route Handlers (BFF si besoin)
│   │   ├── webhooks/stripe/route.ts    # Forward vers .NET avec verif
│   │   └── revalidate/route.ts         # ISR on-demand
│   │
│   ├── layout.tsx                      # Root layout
│   ├── globals.css
│   └── not-found.tsx
│
├── components/
│   ├── ui/                             # Shadcn/ui (Button, Dialog, etc.)
│   ├── catalog/                        # ProductCard, FilterSidebar, etc.
│   ├── checkout/
│   ├── account/
│   ├── admin/
│   └── shared/                         # Header, Footer, MarkdownRenderer
│
├── lib/
│   ├── api/                            # Client API typé (généré ou main)
│   │   ├── client.ts                   # Fetch wrapper avec auth
│   │   ├── catalog.ts
│   │   ├── orders.ts
│   │   └── types.ts                    # Types DTO partagés
│   ├── auth/
│   │   ├── firebase.ts                 # Init Firebase
│   │   ├── useAuth.ts                  # Hook React
│   │   └── AuthProvider.tsx
│   ├── i18n/                           # next-intl
│   ├── stripe/
│   ├── analytics/
│   └── utils/
│
├── hooks/
│   ├── useCart.ts                      # Zustand store
│   ├── useProducts.ts                  # TanStack Query
│   └── useOrders.ts
│
├── stores/                             # Zustand stores
│   └── cart.ts
│
├── public/
├── messages/                           # Traductions FR/EN
│   ├── fr.json
│   └── en.json
│
├── middleware.ts                       # Auth check, locale routing
├── next.config.js
├── tailwind.config.ts
├── tsconfig.json
└── package.json
```

### 7.2 Choix techniques frontend

| Concern | Solution | Justification |
|---------|----------|---------------|
| Framework | Next.js 14 App Router | SSR/ISR pour SEO produit, RSC pour perf |
| Styling | Tailwind CSS + Shadcn/ui | Rapide, maintenable, composants accessibles |
| State global | Zustand (panier) + TanStack Query (data) | Léger, simple, séparation cache serveur/state UI |
| Forms | React Hook Form + Zod | Validation type-safe, perf |
| i18n | next-intl | Support RSC, FR + EN dès MVP, XOF/XAF pour V3 |
| Auth | Firebase JS SDK | Cohérent avec backend, gestion refresh auto |
| Paiement | Stripe Elements | Sécurité PCI déléguée |
| Analytics | GA4 + Vercel Analytics | Standard + perf |
| Markdown | MDX (blog) + react-markdown (docs produits) | Mix de richesse et simplicité |

### 7.3 Stratégie de rendu par page

| Page | Stratégie | Justification |
|------|-----------|---------------|
| Landing `/` | Static + ISR 1h | Contenu marketing peu changeant |
| Catalogue `/catalogue` | SSR avec filtres URL | SEO + filtres dynamiques |
| Produit `/catalogue/[slug]` | ISR 10min, revalidation on-demand | SEO max + fraîcheur prix |
| Blog `/blog/[slug]` | Static (MDX) | Contenu permanent |
| Compte client | CSR (client component) | Données privées |
| Admin | CSR (client component) | Pas d'enjeu SEO |
| Checkout | CSR | État dynamique, sécurité |

### 7.4 Composant clé : ProductCard

Standard du design : image carrée 4:3, titre, prix, badge type (Indicator/Strategy), micro-stats (timeframes supportés, marchés), CTA "Voir détails". Skeleton loading pour les états de chargement. Hover state avec transition douce. Accessible (aria-labels, focus visible).

### 7.5 Flux checkout

```
1. Panier (page /checkout)
   ├─► Récap items + sous-total
   └─► Si non connecté → redirection connexion

2. Page /checkout (étape 1 : Identité)
   ├─► Saisie/confirmation TradingView username (CRITIQUE)
   ├─► Pré-remplissage email/nom depuis Firebase
   └─► Saisie pays + adresse facturation (calcul TVA)

3. Page /checkout (étape 2 : Paiement)
   ├─► Calcul taxes en temps réel via API
   ├─► Stripe Elements (carte, Apple/Google Pay)
   ├─► Code promo optionnel
   └─► Acceptation CGV + disclaimer financier (obligatoire)

4. Soumission
   ├─► API .NET crée Order en BDD (status = pending)
   ├─► Crée PaymentIntent Stripe
   └─► Frontend confirme côté Stripe

5. Webhook Stripe → .NET
   ├─► payment_intent.succeeded
   ├─► API met à jour Order (status = paid)
   ├─► Crée Licenses (status = pending)
   └─► Envoie email confirmation

6. Redirection /checkout/confirmation/[orderId]
   └─► Affiche récap + instructions
```

---

## 8. Intégrations externes

### 8.1 Stripe

- **Compte** : Stripe Standard (pas de Connect)
- **Produits** : créés dynamiquement via API ou pré-créés et liés en BDD
- **Webhooks** :
  - `payment_intent.succeeded` → marque Order payée, crée Licenses
  - `payment_intent.payment_failed` → notifie client
  - `charge.refunded` → workflow remboursement
  - `invoice.paid` (V2 abonnements) → renouvelle abonnement
  - `customer.subscription.deleted` (V2) → désactive abonnement
- **Sécurité** : vérification HMAC du header `Stripe-Signature` à chaque webhook
- **Tests** : utilisation du Stripe CLI pour forwarder les webhooks en local

### 8.2 Email (Brevo)

Templates transactionnels à créer :

| Template | Déclencheur |
|----------|-------------|
| Bienvenue | Création de compte |
| Confirmation de commande | OrderPlaced |
| Confirmation de paiement + facture PDF | OrderPaid |
| Accès accordé | LicenseAccessGranted |
| Mise à jour produit | Publication d'une nouvelle version |
| Réinitialisation mot de passe | (géré par Firebase) |
| Newsletter mensuelle | Manuel/cron |

### 8.3 TradingView

⚠️ **Pas d'API publique officielle**. Le workflow MVP est **semi-manuel** :

1. Client achète → Order payée → Licenses créées en statut `pending`
2. Notification admin (email + dashboard) : "X licences à accorder"
3. Loïc se connecte sur TradingView, va dans la gestion d'accès du script, ajoute le username
4. Loïc clique "Marquer comme accordé" dans le back-office TradeFlow
5. Email automatique au client avec instructions pour ajouter le script

En V2, possibilité d'automatiser via Selenium/Playwright (à étudier juridiquement).

### 8.4 Cloudflare R2

- Bucket `tradeflow-protected` pour les PDF, vidéos, fichiers Pine Script source
- Bucket `tradeflow-public` pour les images, screenshots
- Accès aux fichiers protégés via **URLs présignées** (durée 15 min) générées par l'API après vérification de licence

---

## 9. Sécurité & conformité

### 9.1 Sécurité applicative

- HTTPS obligatoire partout (HSTS)
- CSP strict côté frontend
- Validation côté serveur de **toutes** les entrées (FluentValidation)
- Protection CSRF pour les actions sensibles (cookies SameSite=Strict)
- Rate limiting :
  - 10 req/sec par IP sur les endpoints publics
  - 5 tentatives de connexion / 15min
  - 3 codes promo / 5min
- SQL injection : exclusivement via EF Core paramétré, jamais de SQL dynamique
- XSS : sanitization du Markdown utilisateur (DOMPurify côté front, HtmlSanitizer côté back)
- Pas de secrets en dur : Azure Key Vault pour prod, .NET User Secrets pour dev
- Logs de sécurité : tentatives d'accès non autorisé, changements de rôle, remboursements

### 9.2 Conformité financière

- **Disclaimer obligatoire** sur chaque produit, page checkout, et email de confirmation :
  > "Les outils proposés sont fournis à titre informatif et éducatif uniquement. Ils ne constituent pas un conseil en investissement. Les performances passées ne préjugent pas des performances futures. Le trading comporte un risque de perte en capital."
- Pas de promesses de rendement dans les descriptions
- Pas d'utilisation des termes "garantie de gains", "profit assuré", etc.
- Politique de remboursement claire (14 jours en UE pour les produits non téléchargés ; à clarifier pour les scripts à accès accordé)

### 9.3 RGPD & vie privée

- Politique de confidentialité détaillée (purposes, durées, destinataires)
- Consentement explicite pour la newsletter (double opt-in)
- Bannière cookies conforme (refus aussi simple qu'accepter)
- Droits des utilisateurs : export (JSON), suppression (soft delete + anonymisation après 90j)
- DPA signé avec Firebase, Stripe, Brevo, Cloudflare
- Registre des traitements à tenir
- Localisation des données : préférer régions UE pour les services qui le permettent (Brevo, Cloudflare R2 EU, Azure West Europe)

### 9.4 Fiscalité Québec/Canada

- TVQ (9,975%) + TPS (5%) si client au Québec
- TPS uniquement pour autres provinces canadiennes
- HST pour Ontario/Maritimes
- Pas de taxes pour clients hors Canada (pour le moment, à valider à >30 000 $ CAD de CA international annuel)
- TVA UE : si CA > 10 000 € en UE → enregistrement OSS
- Numéro de taxe à afficher sur les factures dès l'inscription

---

## 10. DevOps, hébergement & CI/CD

### 10.1 Environnements

| Environnement | URL | Usage |
|---------------|-----|-------|
| Local | localhost | Développement |
| Dev | dev.tradeflow.io | Tests intégration |
| Staging | staging.tradeflow.io | Pré-prod, recette |
| Production | tradeflow.io | Production |

### 10.2 Infrastructure de production (Azure)

- **API .NET** : Azure App Service Linux B1 (≈ 13 €/mois MVP, scalable)
- **PostgreSQL** : Azure Database for PostgreSQL Flexible Server B1ms (≈ 25 €/mois)
- **Redis** : Azure Cache for Redis Basic C0 (≈ 15 €/mois)
- **Storage** : Cloudflare R2 (10 GB gratuits)
- **Frontend** : Vercel Pro (20 $/mois si dépassement Hobby)
- **DNS** : Cloudflare
- **Email transactionnel** : Brevo (gratuit 300/jour, ≈ 19 €/mois si dépassement)
- **Monitoring** : Sentry (gratuit 5k events/mois) + Application Insights

**Coût total mensuel MVP estimé** : ≈ 75 €/mois

### 10.3 Pipeline CI/CD GitHub Actions

```yaml
# .github/workflows/api-cicd.yml (résumé)
on:
  push:
    branches: [main, develop]
    paths: ['src/**', 'tests/**']

jobs:
  build-test:
    - Restore dependencies
    - Build solution
    - Run unit tests
    - Run integration tests (avec Testcontainers PostgreSQL)
    - Code coverage > 70% requis

  security:
    - Snyk scan
    - dotnet-list-package --vulnerable

  deploy-staging:
    needs: [build-test, security]
    if: branch == develop
    - Deploy Azure App Service slot staging
    - Run smoke tests

  deploy-prod:
    needs: [build-test, security]
    if: branch == main
    - Migration DB (EF Core bundle)
    - Deploy Azure App Service slot staging
    - Smoke tests
    - Slot swap → production
    - Rollback automatique si erreurs > 1% sur 5 min
```

### 10.4 Stratégie de déploiement

- **Blue-green via slots** Azure App Service
- **Migrations EF Core** appliquées avant le swap (idempotent, backward-compatible)
- **Feature flags** (LaunchDarkly free tier ou propre table BDD) pour activer progressivement
- **Rollback** : swap inverse + restauration backup BDD si nécessaire

### 10.5 Monitoring & alerting

- **Erreurs** : Sentry → notification Discord/Slack
- **Performance** : Application Insights, alerte si P95 > 1s
- **Disponibilité** : check externe via UptimeRobot (gratuit) toutes les 5 min
- **Business KPIs** : dashboard Metabase ou Grafana en lecture seule sur la BDD

---

## 11. Tests & qualité

### 11.1 Pyramide de tests

```
              ┌──────────────┐
              │ E2E (Playwright)
              │   ~10 scénarios critiques
              └──────────────┘
          ┌──────────────────────┐
          │ Functional API tests │
          │   ~50 endpoints      │
          └──────────────────────┘
      ┌────────────────────────────┐
      │ Integration tests          │
      │   Repos, services externes │
      │   ~80 tests                │
      └────────────────────────────┘
  ┌──────────────────────────────────────┐
  │ Unit tests                            │
  │   Domain, Application handlers        │
  │   ~300 tests                          │
  └──────────────────────────────────────┘
```

### 11.2 Outils

- **xUnit** + **FluentAssertions** + **NSubstitute** (mocking) côté .NET
- **Testcontainers** pour les tests d'intégration PostgreSQL
- **Bogus** pour les fixtures de données
- **Vitest** + **React Testing Library** côté Next.js
- **Playwright** pour E2E
- Couverture cible : **> 80%** sur Domain, **> 70%** sur Application

### 11.3 Tests critiques (E2E à automatiser)

1. Inscription + connexion Google complète
2. Parcours achat MVP : catalogue → panier → checkout → confirmation
3. Webhook Stripe → création licence
4. Action admin "Marquer accès accordé" → email envoyé
5. Téléchargement de fichier protégé avec licence valide
6. Refus de téléchargement sans licence
7. Workflow remboursement → révocation licence

---

## 12. Roadmap de développement par phase

### 12.1 Phase 0 — Fondations (2 semaines)

**Objectif** : poser le socle technique sans fonctionnalité métier.

- Création repo monorepo (`/api`, `/web`, `/admin`, `/shared`) ou multi-repo
- Solution .NET avec Clean Architecture + projets vides
- Next.js 14 initialisé avec Tailwind, Shadcn/ui, structure App Router
- PostgreSQL local via Docker Compose
- Configuration Firebase Auth (project, providers Google + email)
- Pipeline CI minimal (build + tests)
- Documentation README claire

**Livrable** : `dotnet run` + `npm run dev` fonctionnent, page d'accueil vide, healthcheck API OK.

### 12.2 Phase 1 — MVP (8-10 semaines)

#### Sprint 1 (S1-S2) : Auth & Customer

- Firebase Auth intégration frontend
- JWT validation .NET
- Synchronisation Customer en BDD
- Pages connexion/inscription/profil

#### Sprint 2 (S3-S4) : Catalogue

- CRUD produits (admin)
- Listing public avec filtres
- Page produit avec rendu Markdown
- Recherche full-text PostgreSQL
- Upload images (Cloudflare R2)

#### Sprint 3 (S5-S6) : Panier & Checkout

- Panier client (Zustand)
- Calcul taxes Québec/Canada
- Intégration Stripe Elements
- Création Order + PaymentIntent
- Webhook Stripe `payment_intent.succeeded`

#### Sprint 4 (S7-S8) : Licensing & Livraison

- Création Licenses au paiement
- Page "Mes achats" client
- Workflow admin "Accorder accès"
- Email transactionnel post-achat
- Génération facture PDF (QuestPDF)

#### Sprint 5 (S9-S10) : Polish & Lancement

- Pages légales (CGV, mentions, RGPD)
- Newsletter (Brevo)
- SEO metadata, sitemap, robots.txt
- Lead magnet (indicateur gratuit)
- Tests E2E des parcours critiques
- Déploiement production
- Soft launch sur 5 produits

**Livrable MVP** : site en production capable de vendre 5 produits, gestion semi-manuelle de la livraison TradingView.

### 12.3 Phase 2 — V2 (3-4 mois)

| Mois | Focus | Fonctionnalités |
|------|-------|-----------------|
| M4 | Reviews & social proof | Avis clients, modération, affichage notes, "Souvent achetés ensemble" |
| M5 | Abonnement SaaS | Plans Stripe, abonnement bibliothèque, gestion renouvellements |
| M6 | Demandes sur-mesure | Formulaire structuré, devis automatique, workflow projet |
| M7 | Marketing automation | Email automation (bienvenue, abandon panier, post-achat), wishlist, codes promo segmentés |

### 12.4 Phase 3 — V3 (6-12 mois)

- Programme d'affiliation
- Paiement crypto (NOWPayments)
- Paiement mobile money (Wave, Orange Money) — synergie marché ouest-africain
- Comptes équipes / fonds
- Marketplace de développeurs Pine Script tiers (optionnel selon traction)
- Application mobile (React Native ou Flutter) pour consultation portefeuille
- Communauté Discord automatisée
- Webinaires et formations payantes

### 12.5 Plan d'action immédiat (S0 — cette semaine)

1. **Réserver le domaine** `tradeflow.io` (ou alternative `.app`, `.dev`)
2. **Créer le projet Firebase** + configurer Auth Google
3. **Créer le compte Stripe** (vérification d'identité = 3-5 jours)
4. **Initialiser les repos GitHub** (api + web)
5. **Designer le logo et l'identité visuelle** (Loïc ou freelance Fiverr ≈ 50 €)
6. **Lister les 5 premiers produits** à mettre en catalogue (à partir de tes scripts existants : OF Pro Suite, Volume Profile, et 3 autres à développer)
7. **Rédiger 3 articles SEO** ("Comment ajouter un script Pine sur TradingView", "Repainting expliqué", "Backtest fiable : checklist")

---

## 13. Annexes & checklists

### 13.1 Checklist de validation produit (protocole de qualité)

Avant qu'un produit soit mis en vente, il **doit** passer ces 4 étapes :

1. **Tests fonctionnels** : tous les paramètres testés, pas de NaN, pas de division par zéro
2. **Backtest multi-actifs/multi-TF** : minimum 3 actifs représentatifs × 3 timeframes
3. **Walk-forward analysis** : robustesse sur des données out-of-sample
4. **Paper trading 7 jours** : exécution réelle sur compte démo

Document de validation signé (PDF ou Markdown) joint à chaque produit.

### 13.2 Checklist conformité avant lancement

- [ ] CGV rédigées et validées (juriste ou template + adaptation)
- [ ] Politique de confidentialité conforme RGPD
- [ ] Politique de remboursement claire
- [ ] Disclaimer financier sur chaque page produit
- [ ] Mentions légales avec NEQ/numéro d'entreprise
- [ ] DPA signés avec sous-traitants
- [ ] Bannière cookies conforme
- [ ] Process d'export et suppression de données testé
- [ ] Numéro de TVQ/TPS (si applicable selon CA)
- [ ] Compte bancaire pro séparé pour TradeFlow

### 13.3 KPIs à suivre dès le MVP

- **Acquisition** : visiteurs uniques/mois, taux de conversion visiteur → inscrit
- **Activation** : taux d'inscrits qui achètent (>5% est bon)
- **Revenu** : CA mensuel, panier moyen, top 3 produits
- **Rétention** : taux d'achat répété, NPS (V2)
- **Support** : temps de réponse, % tickets résolus < 24h
- **Qualité** : taux de remboursement (<3% est sain)

### 13.4 Risques & mitigations

| Risque | Impact | Probabilité | Mitigation |
|--------|--------|-------------|------------|
| TradingView change ses CGU et bannit la revente de scripts | Élevé | Moyen | Diversifier vers MQL5, NinjaTrader ; offrir le code source en option premium |
| Plainte client pour pertes financières | Moyen | Faible | Disclaimer fort + assurance RC pro (≈ 500 €/an) |
| Faille de sécurité (token Firebase ou DB) | Élevé | Faible | Audits sécurité réguliers, secrets en Key Vault, MFA admin |
| Concurrent IA gratuit / pas cher | Moyen | Élevé | Différenciation par qualité validée + service humain + community |
| Désengagement (burnout solo founder) | Élevé | Moyen | Phases courtes avec wins, automatiser les tâches répétitives, planifier les pauses |

### 13.5 Glossaire

- **AAR** : Annual Average Return (métrique backtest)
- **DCA** : Dollar Cost Averaging
- **DDD** : Domain-Driven Design
- **EF Core** : Entity Framework Core, ORM .NET
- **ISR** : Incremental Static Regeneration (Next.js)
- **JWT** : JSON Web Token
- **MAU** : Monthly Active Users
- **NEQ** : Numéro d'Entreprise du Québec
- **OSS** : One Stop Shop (régime TVA UE simplifié)
- **PCI DSS** : Payment Card Industry Data Security Standard
- **Pine Script** : langage de scripting TradingView
- **PKCE** : Proof Key for Code Exchange (OAuth 2.0)
- **RSC** : React Server Components
- **SAS** : Shared Access Signature (URLs présignées)
- **SSR** : Server-Side Rendering
- **TVQ/TPS** : Taxe de Vente du Québec / Taxe sur les Produits et Services
- **UoW** : Unit of Work pattern

---

## Conclusion

Ce plan est **vivant** : il doit être révisé à la fin de chaque sprint pour ajuster la roadmap selon la réalité du marché et des retours utilisateurs.

**Trois principes directeurs à garder en tête :**

1. **Livrer petit, livrer souvent** : le MVP est volontairement minimal pour valider la demande avant d'investir dans V2/V3.
2. **Qualité avant quantité** : ton positionnement repose sur la rigueur de validation. Chaque produit en catalogue est un engagement de qualité.
3. **Protéger ton temps** : tu es la ressource la plus rare. Automatise tout ce qui n'apporte pas de valeur métier directe, et délègue les tâches non-techniques (compta, design ponctuel) dès que le CA le permet.

Bon développement, Loïc. 🚀
