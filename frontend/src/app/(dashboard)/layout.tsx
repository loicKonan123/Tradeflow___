"use client";

export const dynamic = "force-dynamic";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useEffect } from "react";
import { useAuth } from "@/contexts/AuthContext";

const NAV = [
  { href: "/dashboard", label: "Vue d'ensemble", icon: "⊞" },
  { href: "/dashboard/licenses", label: "Mes licences", icon: "🔑" },
  { href: "/dashboard/orders", label: "Commandes", icon: "📦" },
  { href: "/dashboard/projects", label: "Projets sur-mesure", icon: "🏗" },
];

export default function DashboardLayout({ children }: { children: React.ReactNode }) {
  const { user, loading, logout } = useAuth();
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
        </div>

        <nav className="flex-1 px-3 py-4 space-y-1">
          {NAV.map((item) => {
            const active = pathname === item.href;
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
                <span className="text-base">{item.icon}</span>
                {item.label}
              </Link>
            );
          })}
        </nav>

        <div className="px-3 py-4 border-t border-outline-variant/30">
          <div className="px-3 py-2 mb-2">
            <p className="text-xs text-outline truncate">{user.email}</p>
          </div>
          <button
            onClick={async () => { await logout(); router.push("/"); }}
            className="w-full flex items-center gap-3 px-3 py-2.5 text-sm text-on-surface-variant hover:text-error hover:bg-error-container/10 transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            <span>↩</span> Déconnexion
          </button>
        </div>
      </aside>

      {/* Main */}
      <div className="flex-1 flex flex-col min-w-0">
        <header className="px-8 py-4 border-b border-outline-variant/30 bg-surface-container-lowest/40 backdrop-blur-sm">
          <h1
            className="text-lg font-semibold text-on-surface"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            {NAV.find((n) => n.href === pathname)?.label ?? "Dashboard"}
          </h1>
        </header>
        <main className="flex-1 px-8 py-8 overflow-auto">{children}</main>
      </div>
    </div>
  );
}
