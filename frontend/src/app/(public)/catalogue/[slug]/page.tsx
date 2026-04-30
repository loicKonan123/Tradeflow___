"use client";

import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { useEffect, useState } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { getProduct, syncAccount, createOrder, type ProductDetail } from "@/lib/api";

export default function ProductPage() {
  const { slug } = useParams<{ slug: string }>();
  const router = useRouter();
  const { user } = useAuth();

  const [product, setProduct] = useState<ProductDetail | null>(null);
  const [notFound, setNotFound] = useState(false);
  const [loading, setLoading] = useState(true);
  const [buying, setBuying] = useState(false);
  const [buyError, setBuyError] = useState("");

  useEffect(() => {
    getProduct(slug)
      .then(setProduct)
      .catch((e: Error) => {
        if (e.message === "NOT_FOUND") setNotFound(true);
      })
      .finally(() => setLoading(false));
  }, [slug]);

  async function handleBuy() {
    if (!user) {
      router.push(`/auth/login?redirect=/catalogue/${slug}`);
      return;
    }
    if (!product) return;
    setBuyError("");
    setBuying(true);
    try {
      await syncAccount();
      const { checkoutUrl } = await createOrder([product.id]);
      window.location.href = checkoutUrl;
    } catch (e: unknown) {
      setBuyError(e instanceof Error ? e.message : "Erreur lors de la création de commande.");
      setBuying(false);
    }
  }

  if (loading) {
    return (
      <div className="trading-bg pt-24 pb-16 px-8 max-w-screen-2xl mx-auto">
        <div className="grid grid-cols-1 lg:grid-cols-3 gap-12">
          <div className="lg:col-span-2 space-y-6">
            <div className="h-10 w-3/4 bg-surface-container animate-pulse rounded" />
            <div className="h-4 w-full bg-surface-container animate-pulse rounded" />
            <div className="h-4 w-5/6 bg-surface-container animate-pulse rounded" />
          </div>
          <div className="h-64 bg-surface-container animate-pulse rounded-xl" />
        </div>
      </div>
    );
  }

  if (notFound || !product) {
    return (
      <div className="trading-bg pt-24 pb-16 px-8 max-w-screen-2xl mx-auto text-center">
        <h1 className="text-3xl font-bold text-on-surface mb-4" style={{ fontFamily: "var(--font-heading)" }}>
          Produit introuvable
        </h1>
        <Link href="/catalogue" className="text-secondary hover:underline text-sm" style={{ fontFamily: "var(--font-heading)" }}>
          ← Retour au catalogue
        </Link>
      </div>
    );
  }

  const hasBacktest = product.backtestReports.length > 0;
  const mainReport = product.backtestReports[0];

  return (
    <div className="trading-bg pt-24 pb-16 min-h-screen">
      <div className="px-8 max-w-screen-2xl mx-auto">
        {/* Breadcrumb */}
        <div className="flex items-center gap-2 text-xs text-outline mb-8" style={{ fontFamily: "var(--font-heading)" }}>
          <Link href="/catalogue" className="hover:text-secondary transition-colors uppercase tracking-widest">
            Marketplace
          </Link>
          <span>/</span>
          <span className="text-on-surface-variant uppercase tracking-widest">{product.type}</span>
          <span>/</span>
          <span className="text-on-surface uppercase tracking-widest truncate max-w-xs">{product.title}</span>
        </div>

        <div className="grid grid-cols-1 lg:grid-cols-3 gap-12">
          {/* Left — product info */}
          <div className="lg:col-span-2 space-y-8">
            <div>
              <div className="flex items-center gap-3 mb-3">
                <span
                  className="text-[10px] uppercase tracking-widest text-tertiary bg-tertiary-container/30 px-3 py-1"
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  {product.type}
                </span>
                <span
                  className="text-[10px] uppercase tracking-widest text-outline"
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  Pine Script v5
                </span>
              </div>
              <h1
                className="text-4xl font-bold text-on-surface leading-tight mb-3"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                {product.title}
              </h1>
              <p className="text-on-surface-variant text-lg leading-relaxed">
                {product.shortDescription}
              </p>
            </div>

            {/* Backtest stats */}
            {hasBacktest && mainReport && (
              <div className="glass-panel p-6 rounded-xl">
                <h2
                  className="text-xs uppercase tracking-widest text-outline mb-4"
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  Résultats de backtest — {mainReport.title}
                </h2>
                <div className="grid grid-cols-3 gap-4">
                  {[
                    { label: "Win Rate", value: mainReport.winRate != null ? `${mainReport.winRate}%` : "N/A", color: "text-tertiary" },
                    { label: "Profit Factor", value: mainReport.profitFactor?.toFixed(2) ?? "N/A", color: "text-tertiary" },
                    { label: "Max Drawdown", value: mainReport.maxDrawdown != null ? `${mainReport.maxDrawdown}%` : "N/A", color: "text-error" },
                  ].map((s) => (
                    <div key={s.label} className="text-center p-3 bg-surface-container-lowest rounded-lg">
                      <p
                        className="text-[10px] uppercase tracking-widest text-on-primary-container mb-2"
                        style={{ fontFamily: "var(--font-heading)" }}
                      >
                        {s.label}
                      </p>
                      <p
                        className={`text-2xl font-medium ${s.color}`}
                        style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
                      >
                        {s.value}
                      </p>
                    </div>
                  ))}
                </div>
                {(mainReport.periodStart || mainReport.markets) && (
                  <p className="text-xs text-outline mt-3">
                    {mainReport.markets && <span>{mainReport.markets}</span>}
                    {mainReport.periodStart && mainReport.periodEnd && (
                      <span className="ml-2">
                        · {new Date(mainReport.periodStart).getFullYear()}–{new Date(mainReport.periodEnd).getFullYear()}
                      </span>
                    )}
                  </p>
                )}
              </div>
            )}

            {/* Long description */}
            {product.longDescriptionMarkdown && (
              <div className="glass-panel p-6 rounded-xl">
                <h2
                  className="text-xs uppercase tracking-widest text-outline mb-4"
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  Description complète
                </h2>
                <div className="text-on-surface-variant text-sm leading-relaxed whitespace-pre-wrap font-mono">
                  {product.longDescriptionMarkdown}
                </div>
              </div>
            )}

            {/* Inclus */}
            <div className="glass-panel p-6 rounded-xl">
              <h2
                className="text-xs uppercase tracking-widest text-outline mb-4"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Ce qui est inclus
              </h2>
              <ul className="space-y-3">
                {[
                  "Fichier .pine prêt à l'emploi (copier-coller dans TradingView)",
                  "Guide PDF d'utilisation et de paramétrage",
                  "Licence à vie — jusqu'à 5 téléchargements",
                  "Support par email",
                ].map((item) => (
                  <li key={item} className="flex items-start gap-3 text-sm text-on-surface-variant">
                    <span className="text-tertiary mt-0.5 shrink-0">✓</span>
                    {item}
                  </li>
                ))}
              </ul>
            </div>
          </div>

          {/* Right — purchase card (sticky) */}
          <div className="lg:col-span-1">
            <div className="glass-panel p-6 rounded-xl neon-border lg:sticky lg:top-24 space-y-5">
              <div>
                <p
                  className="text-[10px] uppercase tracking-widest text-outline mb-1"
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  Prix
                </p>
                <p
                  className="text-4xl font-medium text-on-surface"
                  style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
                >
                  {product.price} {product.currency}
                </p>
                <p className="text-xs text-outline mt-1">Paiement unique — licence à vie</p>
              </div>

              <div className="border-t border-outline-variant/20 pt-4 space-y-2 text-xs text-outline">
                {[
                  { icon: "↓", label: "Livraison instantanée" },
                  { icon: "🔑", label: "5 téléchargements inclus" },
                  { icon: "✓", label: "Backtest validé" },
                ].map((f) => (
                  <div key={f.label} className="flex items-center gap-2">
                    <span className="text-tertiary">{f.icon}</span>
                    <span>{f.label}</span>
                  </div>
                ))}
              </div>

              {buyError && (
                <div className="bg-error-container/20 border border-error/30 p-3 rounded-lg">
                  <p className="text-xs text-error">{buyError}</p>
                </div>
              )}

              <button
                onClick={handleBuy}
                disabled={buying}
                className="w-full bg-secondary-container text-on-secondary-container py-4 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_25px_rgba(5,102,217,0.5)] transition-all disabled:opacity-60"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                {buying
                  ? "Redirection vers Stripe..."
                  : user
                  ? "Acheter maintenant"
                  : "Se connecter pour acheter"}
              </button>

              {!user && (
                <p className="text-xs text-center text-outline">
                  <Link href={`/auth/register`} className="text-secondary hover:underline">
                    Créer un compte
                  </Link>{" "}
                  pour accéder à votre licence après paiement.
                </p>
              )}

              <div className="border-t border-outline-variant/20 pt-4">
                <p
                  className="text-[10px] uppercase tracking-widest text-outline mb-2"
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  Paiement sécurisé via
                </p>
                <p
                  className="text-lg font-bold text-on-surface-variant italic"
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  stripe
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
