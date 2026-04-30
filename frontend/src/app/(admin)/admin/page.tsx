"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { getAdminOrders, getAdminProjects, getAdminProducts } from "@/lib/api";

export default function AdminOverview() {
  const [stats, setStats] = useState({
    totalProducts: 0,
    publishedProducts: 0,
    pendingOrders: 0,
    paidOrders: 0,
    activeProjects: 0,
    submittedProjects: 0,
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    Promise.all([
      getAdminProducts(1),
      getAdminOrders(1),
      getAdminProjects(1),
    ])
      .then(([products, orders, projects]) => {
        setStats({
          totalProducts: products.total,
          publishedProducts: products.items.filter((p) => p.status === "Published").length,
          pendingOrders: orders.items.filter((o) => o.status === "Pending").length,
          paidOrders: orders.items.filter((o) => o.status === "Paid").length,
          activeProjects: projects.items.filter((p) =>
            ["Submitted", "Quoted", "DepositPaid", "InProgress"].includes(p.status)
          ).length,
          submittedProjects: projects.items.filter((p) => p.status === "Submitted").length,
        });
      })
      .catch(() => {})
      .finally(() => setLoading(false));
  }, []);

  const kpis = [
    { label: "Produits publiés", value: stats.publishedProducts, total: stats.totalProducts, href: "/admin/products", color: "text-secondary" },
    { label: "Commandes payées", value: stats.paidOrders, total: stats.paidOrders + stats.pendingOrders, href: "/admin/orders", color: "text-tertiary" },
    { label: "Projets actifs", value: stats.activeProjects, sub: `${stats.submittedProjects} en attente de devis`, href: "/admin/projects", color: "text-secondary" },
  ];

  return (
    <div className="space-y-8">
      <div>
        <h2
          className="text-2xl font-semibold text-on-surface mb-1"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          Tableau de bord
        </h2>
        <p className="text-sm text-on-surface-variant">Vue d&apos;ensemble de TradeFlow.</p>
      </div>

      {/* KPIs */}
      <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
        {kpis.map((k) => (
          <Link
            key={k.label}
            href={k.href}
            className="glass-panel p-6 rounded-xl hover:border-secondary-container transition-all"
          >
            <p
              className="text-xs uppercase tracking-widest text-on-primary-container mb-3"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              {k.label}
            </p>
            {loading ? (
              <div className="h-10 w-16 bg-surface-container-high animate-pulse rounded" />
            ) : (
              <>
                <p
                  className={`text-4xl font-medium ${k.color}`}
                  style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
                >
                  {k.value}
                </p>
                {k.sub && (
                  <p className="text-xs text-outline mt-1">{k.sub}</p>
                )}
                {k.total !== undefined && k.total !== k.value && (
                  <p className="text-xs text-outline mt-1">sur {k.total} total</p>
                )}
              </>
            )}
          </Link>
        ))}
      </div>

      {/* Quick actions */}
      <div>
        <h3
          className="text-base font-semibold text-on-surface mb-4"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          Actions rapides
        </h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
          {[
            { href: "/admin/products/new", label: "+ Nouveau produit", desc: "Ajouter un script au catalogue" },
            { href: "/admin/projects?status=Submitted", label: "Projets à devis", desc: `${stats.submittedProjects} brief(s) en attente` },
            { href: "/admin/orders?status=Pending", label: "Commandes en attente", desc: `${stats.pendingOrders} paiement(s) à confirmer` },
          ].map((a) => (
            <Link
              key={a.href}
              href={a.href}
              className="glass-panel p-4 rounded-xl hover:border-secondary-container transition-all group"
            >
              <p
                className="text-sm font-semibold text-on-surface group-hover:text-secondary transition-colors"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                {a.label}
              </p>
              <p className="text-xs text-on-surface-variant mt-1">{a.desc}</p>
            </Link>
          ))}
        </div>
      </div>
    </div>
  );
}
