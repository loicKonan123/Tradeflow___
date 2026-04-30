# API Reference — TradeFlow

Base URL : `http://localhost:5175`
Swagger : `http://localhost:5175/swagger`

**Authentification** : Bearer token Firebase (header `Authorization: Bearer <token>`)

---

## Public

### GET /api/catalog
Liste les produits publiés.

**Query params** : `?category=forex&type=Indicator`

**Réponse 200**
```json
[
  {
    "id": "uuid",
    "title": "RSI Divergence Pro",
    "slug": "rsi-divergence-pro",
    "shortDescription": "Détecte les divergences RSI en temps réel",
    "type": "Indicator",
    "price": 49.00,
    "currency": "EUR",
    "status": "Published",
    "salesCount": 12,
    "publishedAt": "2026-04-01T00:00:00Z"
  }
]
```

---

### GET /api/catalog/{slug}
Détail d'un produit avec médias et rapports de backtest.

**Réponse 200**
```json
{
  "id": "uuid",
  "title": "RSI Divergence Pro",
  "slug": "rsi-divergence-pro",
  "shortDescription": "...",
  "longDescriptionMarkdown": "# Description\n...",
  "type": "Indicator",
  "price": 49.00,
  "currency": "EUR",
  "medias": [
    { "id": "uuid", "url": "https://...", "type": "Image", "sortOrder": 0 }
  ],
  "backtestReports": [
    {
      "id": "uuid",
      "title": "EUR/USD 2023-2025",
      "winRate": 61.5,
      "maxDrawdown": 14.2,
      "profitFactor": 1.8,
      "periodStart": "2023-01-01",
      "periodEnd": "2025-01-01",
      "markets": "EUR/USD, GBP/USD, BTC/USD"
    }
  ]
}
```

**Réponse 404** : produit introuvable ou non publié

---

### GET /api/categories
Liste toutes les catégories.

**Réponse 200**
```json
[
  { "id": "uuid", "name": "Forex", "slug": "forex" }
]
```

---

### GET /health
**Réponse 200**
```json
{ "status": "healthy", "timestamp": "2026-04-29T12:00:00Z" }
```

---

## Client (🔒 Firebase JWT requis)

### POST /api/account/sync
Synchronise le compte Firebase avec la base de données.
À appeler à chaque connexion.

**Réponse 200**
```json
{ "customerId": "uuid" }
```

---

### PUT /api/account/profile
Met à jour le profil.

**Body**
```json
{
  "displayName": "Marc Dupont",
  "tradingViewUsername": "marc_trader"
}
```

**Réponse 204**

---

### GET /api/orders
Liste mes commandes.

**Réponse 200**
```json
[
  {
    "id": "uuid",
    "orderNumber": "TF-2026-0001",
    "total": 49.00,
    "currency": "EUR",
    "status": "Fulfilled",
    "createdAt": "2026-04-01T10:00:00Z",
    "items": [
      { "productId": "uuid", "productTitle": "RSI Divergence Pro", "unitPrice": 49.00, "currency": "EUR" }
    ]
  }
]
```

---

### POST /api/orders
Crée une commande et retourne l'URL Stripe Checkout.

**Body**
```json
{
  "productIds": ["uuid1", "uuid2"],
  "currency": "EUR"
}
```

**Réponse 200**
```json
{
  "orderId": "uuid",
  "orderNumber": "TF-2026-0001",
  "checkoutUrl": "https://checkout.stripe.com/..."
}
```

**Erreurs**
- `400` : produit indisponible ou déjà acheté

---

### GET /api/licenses
Liste mes licences (achats).

**Réponse 200**
```json
[
  {
    "id": "uuid",
    "productId": "uuid",
    "productTitle": "RSI Divergence Pro",
    "status": "Active",
    "downloadCount": 1,
    "maxDownloads": 5,
    "createdAt": "2026-04-01T10:00:00Z"
  }
]
```

---

### POST /api/licenses/{id}/download
Génère un lien de téléchargement signé (valable 1h).

**Réponse 200**
```json
{ "url": "https://r2.cloudflare.com/...?expires=..." }
```

**Erreurs**
- `400` : limite de téléchargements atteinte (5 max), licence révoquée

---

### GET /api/projects
Liste mes projets sur-mesure.

**Réponse 200**
```json
[
  {
    "id": "uuid",
    "market": "Crypto",
    "timeframe": "1H",
    "status": "Quoted",
    "quotedPrice": 149.00,
    "quotedCurrency": "EUR",
    "depositAmount": 74.50,
    "createdAt": "2026-04-01T10:00:00Z"
  }
]
```

---

### POST /api/projects
Soumet une demande de script sur-mesure.

**Body**
```json
{
  "market": "Crypto",
  "timeframe": "1H",
  "entryConditions": "RSI < 35 ET prix au-dessus de EMA 200",
  "exitConditions": "RSI > 65 OU stop loss 2%",
  "riskManagement": "Stop loss 2%, Take profit 3.5%, taille de position 1%",
  "additionalNotes": "Tester sur BTC/USDT et ETH/USDT"
}
```

**Réponse 200**
```json
{ "projectId": "uuid" }
```

---

### POST /api/projects/{id}/cancel
Annule un projet (impossible si Delivered ou Cancelled).

**Réponse 204**

---

## Admin (🔒 role: admin requis)

### GET /api/admin/products
Liste tous les produits avec pagination.

**Query params** : `?page=1&pageSize=20&status=Draft`

**Réponse 200**
```json
{
  "items": [...],
  "total": 42,
  "page": 1,
  "pageSize": 20,
  "totalPages": 3
}
```

---

### POST /api/admin/products
Crée un produit (statut Draft).

**Body**
```json
{
  "title": "RSI Divergence Pro",
  "shortDescription": "Détecte les divergences RSI",
  "longDescriptionMarkdown": "# Description\n...",
  "type": "Indicator",
  "price": 49.00,
  "currency": "EUR",
  "categoryId": "uuid"
}
```

**Réponse 200** : `{ "id": "uuid" }`

---

### POST /api/admin/products/{id}/file
Upload le fichier `.pine` ou PDF du produit.

**Body** : `multipart/form-data` avec le fichier

**Réponse 200** : `{ "url": "fileKey" }`

---

### POST /api/admin/products/{id}/publish
Publie le produit (doit avoir un fichier uploadé).

**Réponse 204** | **400** si pas de fichier

---

### POST /api/admin/products/{id}/archive
Archive le produit.

**Réponse 204**

---

### GET /api/admin/orders
Liste toutes les commandes avec pagination.

**Query params** : `?page=1&pageSize=20&status=Paid`

---

### POST /api/admin/licenses/{id}/revoke
Révoque une licence.

**Réponse 204**

---

### GET /api/admin/projects
Liste tous les projets sur-mesure avec pagination.

**Query params** : `?page=1&pageSize=20&status=Submitted`

---

### POST /api/admin/projects/{id}/quote
Envoie un devis au client.

**Body**
```json
{
  "price": 149.00,
  "currency": "EUR",
  "adminNotes": "Faisable, délai estimé 5 jours ouvrés."
}
```

**Réponse 204**

---

### POST /api/admin/projects/{id}/start
Démarre le travail (le dépôt doit être payé).

**Réponse 204**

---

### POST /api/admin/projects/{id}/deliver
Livre le projet avec le fichier final.

**Body** : `multipart/form-data`
- `file` : le fichier `.pine`
- `deliveryNotes` : notes de livraison (optionnel)

**Réponse 204**

---

### POST /api/admin/projects/{id}/cancel
Annule un projet.

**Réponse 204**

---

## Webhook Stripe

### POST /api/webhooks/stripe
Reçoit les événements Stripe.

Événement traité : `checkout.session.completed`
→ Marque la commande comme payée
→ Crée automatiquement les licences pour chaque produit acheté

**Sécurité** : vérifié via `Stripe-Signature` header + `WebhookSecret`

---

## Codes d'erreur

| Code | Signification |
|------|---------------|
| 400 | Erreur métier (détail dans `error`) |
| 401 | Non authentifié |
| 403 | Non autorisé (pas admin) |
| 404 | Ressource introuvable |
| 422 | Validation échouée (FluentValidation) |
