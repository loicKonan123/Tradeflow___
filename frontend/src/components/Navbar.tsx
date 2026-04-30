"use client";

import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useAuth } from "@/contexts/AuthContext";

const navLinks = [
  { href: "/catalogue", label: "Marketplace" },
  { href: "/sur-mesure", label: "Sur-mesure" },
  { href: "/#about", label: "About" },
];

export default function Navbar() {
  const pathname = usePathname();
  const router = useRouter();
  const { user, isAdmin, logout } = useAuth();

  async function handleLogout() {
    await logout();
    router.push("/");
  }

  return (
    <header className="fixed top-0 w-full z-50 border-b border-slate-800/60 bg-slate-950/80 backdrop-blur-xl shadow-[0_4px_30px_rgba(0,0,0,0.5)]">
      <div className="flex justify-between items-center px-8 py-3 max-w-screen-2xl mx-auto">
        <div className="flex items-center gap-8">
          <Link
            href="/"
            className="text-xl font-bold tracking-tighter text-slate-50"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            TradeFlow
          </Link>
          <nav className="hidden md:flex gap-6">
            {navLinks.map((link) => {
              const active =
                link.href !== "/#about" &&
                (pathname === link.href || pathname.startsWith(link.href + "/"));
              return (
                <Link
                  key={link.href}
                  href={link.href}
                  className={`uppercase tracking-widest text-xs transition-colors px-2 py-1 ${
                    active
                      ? "text-blue-400 border-b border-blue-500 font-bold"
                      : "text-slate-400 hover:text-slate-200"
                  }`}
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  {link.label}
                </Link>
              );
            })}
          </nav>
        </div>

        <div className="flex items-center gap-3">
          {user ? (
            <>
              {isAdmin && (
                <Link
                  href="/admin"
                  className={`uppercase tracking-widest text-xs px-2 py-1 transition-colors ${
                    pathname.startsWith("/admin")
                      ? "text-tertiary font-bold"
                      : "text-slate-400 hover:text-tertiary"
                  }`}
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  Admin
                </Link>
              )}
              <Link
                href="/dashboard"
                className={`uppercase tracking-widest text-xs px-2 py-1 transition-colors ${
                  pathname.startsWith("/dashboard")
                    ? "text-blue-400 font-bold"
                    : "text-slate-400 hover:text-slate-200"
                }`}
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Dashboard
              </Link>
              <button
                onClick={handleLogout}
                className="uppercase tracking-widest text-xs text-slate-400 hover:text-slate-200 transition-colors"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Déconnexion
              </button>
            </>
          ) : (
            <>
              <Link
                href="/auth/login"
                className="uppercase tracking-widest text-xs text-slate-400 hover:text-slate-200 transition-colors"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Login
              </Link>
              <Link
                href="/auth/register"
                className="bg-secondary-container text-on-secondary-container px-4 py-2 uppercase tracking-widest text-xs font-bold hover:shadow-[0_0_15px_rgba(5,102,217,0.4)] transition-all active:scale-95"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Get Started
              </Link>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
