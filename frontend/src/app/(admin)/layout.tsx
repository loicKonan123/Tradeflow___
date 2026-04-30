"use client";

export const dynamic = "force-dynamic";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect } from "react";
import { useAuth } from "@/contexts/AuthContext";

const NAV = [
  { href: "/admin", label: "Vue d'ensemble", icon: "⊞", exact: true },
  { href: "/admin/products", label: "Catalogue", icon: "📦" },
  { href: "/admin/projects", label: "Projets", icon: "🏗" },
  { href: "/admin/orders", label: "Commandes", icon: "💳" },
];

export default function AdminLayout({ children }: { children: React.ReactNode }) {
  const { user, loading } = useAuth();
  const router = useRouter();
  const pathname = usePathname();

  useEffect(() => {
    if (!loading && !user) router.push("/auth/login");
  }, [loading, user, router]);

  if (loading || !user) {
    return (
      <div className="min-h-screen bg-background flex items-center justify-center">
        <div className="w-6 h-6 border-2 border-secondary-container border-t-transparent rounded-full animate-spin" />
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-background trading-bg flex">
      {/* Sidebar */}
      <aside className="w-64 shrink-0 border-r border-outline-variant/30 bg-surface-container-lowest/80 flex flex-col">
        <div className="px-6 py-4 border-b border-outline-variant/30">
          <Link
            href="/"
            className="text-xl font-bold tracking-tighter text-on-surface"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            TradeFlow
          </Link>
          <span
            className="ml-2 text-[10px] uppercase tracking-widest text-secondary bg-secondary-container/20 px-2 py-0.5"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Admin
          </span>
        </div>

        <nav className="flex-1 px-3 py-4 space-y-1">
          {NAV.map((item) => {
            const active = item.exact
              ? pathname === item.href
              : pathname === item.href || pathname.startsWith(item.href + "/");
            return (
              <Link
                key={item.href}
                href={item.href}
                className={`flex items-center gap-3 px-3 py-2.5 text-sm transition-all ${
                  active
                    ? "bg-secondary-container/20 text-secondary border-l-2 border-secondary-container"
                    : "text-on-surface-variant hover:text-on-surface hover:bg-surface-container"
                }`}
                style={{ fontFamily: "var(--font-heading)" }}
              >
                <span>{item.icon}</span>
                {item.label}
              </Link>
            );
          })}
        </nav>

        <div className="px-3 py-4 border-t border-outline-variant/30">
          <Link
            href="/dashboard"
            className="flex items-center gap-3 px-3 py-2.5 text-sm text-on-surface-variant hover:text-on-surface hover:bg-surface-container transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            <span>←</span> Espace client
          </Link>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 flex flex-col min-w-0">
        <header className="px-8 py-4 border-b border-outline-variant/30 bg-surface-container-lowest/40 backdrop-blur-sm flex items-center justify-between">
          <h1
            className="text-lg font-semibold text-on-surface"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            {NAV.find((n) =>
              n.exact ? pathname === n.href : pathname.startsWith(n.href)
            )?.label ?? "Admin"}
          </h1>
          <span className="text-xs text-outline">{user.email}</span>
        </header>
        <main className="flex-1 px-8 py-8 overflow-auto">{children}</main>
      </div>
    </div>
  );
}
