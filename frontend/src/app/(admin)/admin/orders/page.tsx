"use client";

import { useEffect, useState } from "react";
import { getAdminOrders, type AdminOrder } from "@/lib/api";

const STATUS_LABEL: Record<string, string> = {
  Pending: "En attente",
  Paid: "Payée",
  Cancelled: "Annulée",
};
const STATUS_CLASS: Record<string, string> = {
  Pending: "text-secondary bg-secondary-container/20",
  Paid: "text-tertiary bg-tertiary-container/40",
  Cancelled: "text-error bg-error-container/20",
};

export default function AdminOrdersPage() {
  const [orders, setOrders] = useState<AdminOrder[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");

  useEffect(() => {
    setLoading(true);
    getAdminOrders(page, statusFilter || undefined)
      .then((r) => { setOrders(r.items); setTotal(r.total); })
      .catch(() => setError("Impossible de charger les commandes."))
      .finally(() => setLoading(false));
  }, [page, statusFilter]);

  const totalPages = Math.ceil(total / 20);

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 justify-between items-start sm:items-center">
        <p className="text-sm text-on-surface-variant">{total} commande{total !== 1 ? "s" : ""}</p>
        <div className="flex gap-2 flex-wrap">
          {["", "Pending", "Paid", "Cancelled"].map((s) => (
            <button
              key={s}
              onClick={() => { setStatusFilter(s); setPage(1); }}
              className={`px-4 py-1.5 text-xs uppercase tracking-widest font-semibold transition-all ${
                statusFilter === s
                  ? "bg-secondary-container text-on-secondary-container"
                  : "bg-surface-container text-on-surface-variant hover:text-on-surface"
              }`}
              style={{ fontFamily: "var(--font-heading)" }}
            >
              {s || "Toutes"}
            </button>
          ))}
        </div>
      </div>

      {error && (
        <div className="glass-panel p-3 rounded-lg border border-error/30">
          <p className="text-sm text-error">{error}</p>
        </div>
      )}

      {loading ? (
        <div className="space-y-3">
          {[1, 2, 3].map((i) => <div key={i} className="h-20 bg-surface-container animate-pulse rounded-xl" />)}
        </div>
      ) : orders.length === 0 ? (
        <div className="glass-panel p-12 rounded-xl text-center">
          <p className="text-on-surface-variant">Aucune commande.</p>
        </div>
      ) : (
        <div className="space-y-3">
          {orders.map((order) => (
            <div key={order.id} className="glass-panel p-5 rounded-xl flex flex-col sm:flex-row sm:items-center gap-4">
              <div className="flex-1 min-w-0">
                <div className="flex items-center gap-3 mb-1">
                  <span
                    className="text-sm font-semibold text-on-surface"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    #{order.orderNumber}
                  </span>
                  <span
                    className={`text-[10px] uppercase tracking-widest px-2 py-0.5 ${STATUS_CLASS[order.status] ?? "text-outline"}`}
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    {STATUS_LABEL[order.status] ?? order.status}
                  </span>
                </div>
                <p className="text-xs text-outline">
                  {order.customerEmail} · {order.itemCount} article{order.itemCount !== 1 ? "s" : ""} ·{" "}
                  {new Date(order.createdAt).toLocaleDateString("fr-FR")}
                </p>
              </div>
              <span
                className="text-xl font-medium text-on-surface shrink-0"
                style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
              >
                {order.totalAmount.toFixed(2)} {order.currency}
              </span>
            </div>
          ))}
        </div>
      )}

      {totalPages > 1 && (
        <div className="flex items-center justify-center gap-2 pt-4">
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
            className="px-4 py-2 text-xs uppercase tracking-widest text-on-surface-variant border border-outline-variant hover:border-outline disabled:opacity-40 transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >← Préc</button>
          <span className="text-xs text-outline" style={{ fontFamily: "var(--font-heading)" }}>
            {page} / {totalPages}
          </span>
          <button
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
            disabled={page === totalPages}
            className="px-4 py-2 text-xs uppercase tracking-widest text-on-surface-variant border border-outline-variant hover:border-outline disabled:opacity-40 transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >Suiv →</button>
        </div>
      )}
    </div>
  );
}
