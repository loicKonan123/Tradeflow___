"use client";

import Link from "next/link";
import { useEffect, useState, Suspense } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import { getOrders, type Order } from "@/lib/api";

const STATUS_LABELS: Record<string, string> = {
  Pending: "En attente",
  Paid: "Payée",
  Cancelled: "Annulée",
};
const STATUS_CLASSES: Record<string, string> = {
  Pending: "text-secondary bg-secondary-container/20",
  Paid: "text-tertiary bg-tertiary-container/40",
  Cancelled: "text-error bg-error-container/20",
};

function OrdersContent() {
  const searchParams = useSearchParams();
  const router = useRouter();
  const [orders, setOrders] = useState<Order[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  const isSuccess = searchParams.get("success") === "true";
  const isCancelled = searchParams.get("cancelled") === "true";

  useEffect(() => {
    getOrders()
      .then(setOrders)
      .catch(() => setError("Impossible de charger les commandes."))
      .finally(() => setLoading(false));
  }, []);

  function dismissBanner() {
    router.replace("/dashboard/orders");
  }

  if (loading) {
    return (
      <div className="space-y-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-28 bg-surface-container animate-pulse rounded-xl" />
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Success banner */}
      {isSuccess && (
        <div className="glass-panel p-4 rounded-xl border border-tertiary/40 flex items-start justify-between gap-4">
          <div className="flex items-start gap-3">
            <span className="text-tertiary text-xl mt-0.5">✓</span>
            <div>
              <p
                className="text-sm font-semibold text-on-surface"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Paiement confirmé !
              </p>
              <p className="text-xs text-on-surface-variant mt-0.5">
                Votre commande est enregistrée. Votre licence est disponible dans{" "}
                <Link href="/dashboard/licenses" className="text-secondary hover:underline">
                  Mes licences
                </Link>.
              </p>
            </div>
          </div>
          <button
            onClick={dismissBanner}
            className="text-outline hover:text-on-surface text-lg leading-none shrink-0"
            aria-label="Fermer"
          >
            ×
          </button>
        </div>
      )}

      {/* Cancelled banner */}
      {isCancelled && (
        <div className="glass-panel p-4 rounded-xl border border-outline-variant/40 flex items-start justify-between gap-4">
          <div className="flex items-start gap-3">
            <span className="text-outline text-xl mt-0.5">✕</span>
            <div>
              <p
                className="text-sm font-semibold text-on-surface"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Paiement annulé
              </p>
              <p className="text-xs text-on-surface-variant mt-0.5">
                Votre commande n&apos;a pas été finalisée. Retournez au{" "}
                <Link href="/catalogue" className="text-secondary hover:underline">
                  catalogue
                </Link>{" "}
                pour réessayer.
              </p>
            </div>
          </div>
          <button
            onClick={dismissBanner}
            className="text-outline hover:text-on-surface text-lg leading-none shrink-0"
            aria-label="Fermer"
          >
            ×
          </button>
        </div>
      )}

      <p className="text-sm text-on-surface-variant">
        {orders.length} commande{orders.length !== 1 ? "s" : ""}
      </p>

      {error && (
        <div className="glass-panel p-4 rounded-lg border border-error/30">
          <p className="text-sm text-error">{error}</p>
        </div>
      )}

      {orders.length === 0 ? (
        <div className="glass-panel p-12 rounded-xl text-center">
          <p className="text-on-surface-variant mb-4">Aucune commande pour l&apos;instant.</p>
          <Link
            href="/catalogue"
            className="bg-secondary-container text-on-secondary-container px-6 py-2 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_15px_rgba(5,102,217,0.3)] transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Explorer la marketplace
          </Link>
        </div>
      ) : (
        <div className="space-y-4">
          {orders.map((order) => (
            <div key={order.id} className="glass-panel p-6 rounded-xl">
              <div className="flex flex-col sm:flex-row sm:items-start justify-between gap-4 mb-4">
                <div>
                  <div className="flex items-center gap-3 mb-1">
                    <h3
                      className="text-base font-semibold text-on-surface"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      #{order.orderNumber}
                    </h3>
                    <span
                      className={`text-[10px] uppercase tracking-widest px-2 py-0.5 ${STATUS_CLASSES[order.status] ?? "text-outline"}`}
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {STATUS_LABELS[order.status] ?? order.status}
                    </span>
                  </div>
                  <p className="text-xs text-outline">
                    {new Date(order.createdAt).toLocaleDateString("fr-FR", {
                      day: "2-digit",
                      month: "long",
                      year: "numeric",
                    })}
                    {order.paidAt && (
                      <span className="ml-2 text-tertiary">
                        · Payée le {new Date(order.paidAt).toLocaleDateString("fr-FR")}
                      </span>
                    )}
                  </p>
                </div>
                <span
                  className="text-2xl font-medium text-on-surface shrink-0"
                  style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
                >
                  {order.totalAmount.toFixed(2)} {order.currency}
                </span>
              </div>

              <div className="border-t border-outline-variant/20 pt-4 space-y-2">
                {order.items.map((item, i) => (
                  <div key={i} className="flex justify-between items-center">
                    <p className="text-sm text-on-surface-variant">{item.productName}</p>
                    <p
                      className="text-sm text-on-surface"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {item.unitPrice.toFixed(2)} {item.currency}
                    </p>
                  </div>
                ))}
              </div>

              {order.status === "Paid" && (
                <div className="mt-4 pt-4 border-t border-outline-variant/20">
                  <Link
                    href="/dashboard/licenses"
                    className="text-xs uppercase tracking-widest text-secondary hover:underline"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    Accéder à mes licences →
                  </Link>
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}

export default function OrdersPage() {
  return (
    <Suspense fallback={
      <div className="space-y-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-28 bg-surface-container animate-pulse rounded-xl" />
        ))}
      </div>
    }>
      <OrdersContent />
    </Suspense>
  );
}
