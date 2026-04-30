# Guide de démarrage — TradeFlow

## Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js 20+](https://nodejs.org)
- Un compte [Firebase](https://firebase.google.com)
- Un compte [Stripe](https://stripe.com)
- Un compte [Cloudflare](https://cloudflare.com) (pour R2)

---

## 1. Base de données (PostgreSQL)

```bash
docker compose up -d
```

La base `tradeflow` sera disponible sur `localhost:5432`.

---

## 2. Backend (.NET 9)

### Configuration

Crée `backend/src/TradeFlow.Api/appsettings.Development.json` (ignoré par git) :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=tradeflow;Username=tradeflow;Password=tradeflow_dev"
  },
  "Firebase": {
    "ProjectId": "ton-projet-firebase"
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "R2": {
    "AccountId": "ton-account-id",
    "AccessKeyId": "ta-cle",
    "SecretAccessKey": "ton-secret",
    "BucketName": "tradeflow-files",
    "PublicUrl": "https://..."
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000"]
  }
}
```

### Migration & démarrage

```bash
cd backend/src/TradeFlow.Api
dotnet ef database update --project ../TradeFlow.Infrastructure
dotnet run
```

- API : `http://localhost:5175`
- Swagger UI : `http://localhost:5175/swagger`
- Health check : `http://localhost:5175/health`

---

## 3. Frontend (Next.js 16)

```bash
cd web
npm install
cp .env.local.example .env.local
# Remplis les variables dans .env.local
npm run dev
```

Le frontend démarre sur `http://localhost:3000`.

### Variables frontend (`.env.local`)

```env
NEXT_PUBLIC_API_URL=http://localhost:5175
NEXT_PUBLIC_FIREBASE_API_KEY=AIza...
NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN=ton-projet.firebaseapp.com
NEXT_PUBLIC_FIREBASE_PROJECT_ID=ton-projet
NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY=pk_test_...
```

---

## 4. Stripe Webhook (développement)

Installe la [Stripe CLI](https://stripe.com/docs/stripe-cli) puis :

```bash
stripe listen --forward-to localhost:5175/api/webhooks/stripe
```

Copie le `whsec_...` affiché dans `appsettings.Development.json`.

---

## 5. Firebase

1. Crée un projet sur [Firebase Console](https://console.firebase.google.com)
2. Active **Authentication** → **Email/Password**
3. Note le **Project ID** → `appsettings.Development.json` (backend)
4. Dans **Paramètres du projet** → **Vos applications** → crée une app Web
5. Copie `apiKey`, `authDomain`, `projectId` → `.env.local` (frontend)

---

## Résumé des variables

### Backend

| Variable | Exemple |
|----------|---------|
| `ConnectionStrings:DefaultConnection` | `Host=localhost;...` |
| `Firebase:ProjectId` | `tradeflow-prod` |
| `Stripe:SecretKey` | `sk_test_...` |
| `Stripe:WebhookSecret` | `whsec_...` |
| `R2:AccountId` | `abc123def456` |
| `R2:AccessKeyId` | `...` |
| `R2:SecretAccessKey` | `...` |
| `R2:BucketName` | `tradeflow-files` |

### Frontend

| Variable | Exemple |
|----------|---------|
| `NEXT_PUBLIC_API_URL` | `http://localhost:5175` |
| `NEXT_PUBLIC_FIREBASE_API_KEY` | `AIza...` |
| `NEXT_PUBLIC_FIREBASE_AUTH_DOMAIN` | `projet.firebaseapp.com` |
| `NEXT_PUBLIC_FIREBASE_PROJECT_ID` | `tradeflow-prod` |
| `NEXT_PUBLIC_STRIPE_PUBLISHABLE_KEY` | `pk_test_...` |

---

## Déploiement (production)

| Service | Plateforme | Notes |
|---------|-----------|-------|
| Backend .NET | Railway | Variables d'env dans le dashboard Railway |
| Frontend Next.js | Vercel | Variables dans les settings Vercel |
| Base de données | Neon | URL de connexion PostgreSQL |
| Fichiers | Cloudflare R2 | Bucket avec CORS configuré |
