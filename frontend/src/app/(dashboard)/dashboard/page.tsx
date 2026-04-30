"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { getLicenses, getOrders, getProjects, type License, type Order, type Project } from "@/lib/api";

export default function DashboardOverview() {
  const { user } = useAuth();
  const [licenses, setLicenses] = useState<License[]>([]);
  const [orders, setOrders] = useState<Order[]>([]);
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([getLicenses(), getOrders(), getProjects()])
      .then(([l, o, p]) => { setLicenses(l); setOrders(o); setProjects(p); })
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  const activeProjects = projects.filter(
    (p) => !["Delivered", "Cancelled"].includes(p.status)
  );

  const stats = [
    { label: "Licences actives", value: licenses.filter((l) => !l.isRevoked).length, href: "/dashboard/licenses", color: "text-tertiary" },
    { label: "Commandes", value: orders.length, href: "/dashboard/orders", color: "text-secondary" },
    { label: "Projets en cours", value: activeProjects.length, href: "/dashboard/projects", color: "text-secondary" },
  ];

  return (
    <div className="space-y-8">
      <div>
        <h2
          className="text-2xl font-semibold text-on-surface mb-1"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          Bonjour 👋
        </h2>
        <p className="text-sm text-on-surface-variant">{user?.email}</p>
      </div>

      {/* Stats */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {stats.map((s) => (
          <Link key={s.label} href={s.href} className="glass-panel p-6 rounded-xl hover:border-secondary-container transition-all group">
            <p
              className="text-xs uppercase tracking-widest text-on-primary-container mb-2"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              {s.label}
            </p>
            {loading ? (
              <div className="h-8 w-12 bg-surface-container-high animate-pulse rounded" />
            ) : (
              <p
                className={`text-4xl font-medium ${s.color}`}
                style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
              >
                {s.value}
              </p>
            )}
          </Link>
        ))}
      </div>

      {/* Recent licenses */}
      <div>
        <div className="flex justify-between items-center mb-4">
          <h3
            className="text-base font-semibold text-on-surface"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Licences récentes
          </h3>
          <Link
            href="/dashboard/licenses"
            className="text-xs uppercase tracking-widest text-secondary hover:underline"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Voir tout →
          </Link>
        </div>

        {loading ? (
          <div className="space-y-3">
            {[1, 2].map((i) => (
              <div key={i} className="h-16 bg-surface-container animate-pulse rounded-lg" />
            ))}
          </div>
        ) : licenses.length === 0 ? (
          <div className="glass-panel p-8 rounded-xl text-center">
            <p className="text-on-surface-variant text-sm mb-4">Aucune licence pour l&apos;instant.</p>
            <Link
              href="/catalogue"
              className="text-xs uppercase tracking-widest text-secondary hover:underline"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Explorer la marketplace →
            </Link>
          </div>
        ) : (
          <div className="space-y-3">
            {licenses.slice(0, 3).map((l) => (
              <div key={l.id} className="glass-panel p-4 rounded-lg flex items-center justify-between">
                <div>
                  <p
                    className="text-sm font-medium text-on-surface"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    {l.productName}
                  </p>
                  <p className="text-xs text-outline mt-0.5">
                    {l.downloadsUsed}/{l.maxDownloads} téléchargements
                  </p>
                </div>
                {l.isRevoked ? (
                  <span
                    className="text-[10px] uppercase tracking-widest text-error bg-error-container/20 px-3 py-1"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    Révoquée
                  </span>
                ) : (
                  <span
                    className="text-[10px] uppercase tracking-widest text-tertiary bg-tertiary-container/40 px-3 py-1"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    Active
                  </span>
                )}
              </div>
            ))}
          </div>
        )}
      </div>

      {/* Active projects */}
      {activeProjects.length > 0 && (
        <div>
          <div className="flex justify-between items-center mb-4">
            <h3
              className="text-base font-semibold text-on-surface"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Projets en cours
            </h3>
            <Link
              href="/dashboard/projects"
              className="text-xs uppercase tracking-widest text-secondary hover:underline"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Voir tout →
            </Link>
          </div>
          <div className="space-y-3">
            {activeProjects.slice(0, 2).map((p) => (
              <div key={p.id} className="glass-panel p-4 rounded-lg flex items-center justify-between">
                <div>
                  <p
                    className="text-sm font-medium text-on-surface"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    #{p.strategyTitle}
                  </p>
                  <p className="text-xs text-outline mt-0.5">{p.market} · {p.timeframe}</p>
                </div>
                <StatusBadge status={p.status} />
              </div>
            ))}
          </div>
        </div>
      )}
    </div>
  );
}

function StatusBadge({ status }: { status: string }) {
  const map: Record<string, string> = {
    Submitted: "text-secondary bg-secondary-container/20",
    Quoted: "text-tertiary bg-tertiary-container/40",
    DepositPaid: "text-tertiary bg-tertiary-container/40",
    InProgress: "text-secondary bg-secondary-container/20",
    Delivered: "text-tertiary bg-tertiary-container/40",
    Cancelled: "text-error bg-error-container/20",
  };
  const labels: Record<string, string> = {
    Submitted: "Soumis",
    Quoted: "Devis reçu",
    DepositPaid: "Acompte payé",
    InProgress: "En cours",
    Delivered: "Livré",
    Cancelled: "Annulé",
  };
  return (
    <span
      className={`text-[10px] uppercase tracking-widest px-3 py-1 ${map[status] ?? "text-outline"}`}
      style={{ fontFamily: "var(--font-heading)" }}
    >
      {labels[status] ?? status}
    </span>
  );
}
