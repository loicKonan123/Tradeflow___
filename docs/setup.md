# Guide de démarrage — TradeFlow

## Prérequis

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [Node.js 20+](https://nodejs.org) (pour le frontend)
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

Crée le fichier `backend/src/TradeFlow.Api/appsettings.Development.json` (ignoré par git) :

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
  }
}
```

### Migration de la base de données

```bash
cd backend/src/TradeFlow.Api
dotnet ef database update --project ../TradeFlow.Infrastructure
```

### Lancer l'API

```bash
cd backend/src/TradeFlow.Api
dotnet run
```

L'API démarre sur `http://localhost:5175`

- Swagger UI : `http://localhost:5175/swagger`
- Health check : `http://localhost:5175/health`

---

## 3. Frontend (Next.js 14)

```bash
cd web
npm install
cp .env.example .env.local
# Remplis les variables dans .env.local
npm run dev
```

Le frontend démarre sur `http://localhost:3000`

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
2. Active **Authentication** → Email/Password + Google
3. Copie le **Project ID** dans `appsettings.Development.json`
4. Pour le frontend, récupère la config Firebase (`firebaseConfig`) depuis les paramètres du projet

---

## Variables d'environnement (résumé)

| Variable | Description |
|----------|-------------|
| `Firebase:ProjectId` | ID du projet Firebase |
| `Stripe:SecretKey` | Clé secrète Stripe (`sk_test_...`) |
| `Stripe:WebhookSecret` | Secret webhook Stripe (`whsec_...`) |
| `R2:AccountId` | ID du compte Cloudflare |
| `R2:AccessKeyId` | Clé d'accès R2 |
| `R2:SecretAccessKey` | Clé secrète R2 |
| `R2:BucketName` | Nom du bucket R2 |
| `ConnectionStrings:DefaultConnection` | URL PostgreSQL |
