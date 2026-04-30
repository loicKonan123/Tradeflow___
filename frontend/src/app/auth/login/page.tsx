"use client";

import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import { useState, Suspense } from "react";
import { useAuth } from "@/contexts/AuthContext";

function LoginForm() {
  const { login, loginWithGoogle } = useAuth();
  const router = useRouter();
  const searchParams = useSearchParams();
  const redirect = searchParams.get("redirect") ?? "/dashboard";
  const [form, setForm] = useState({ email: "", password: "" });
  const [error, setError] = useState("");
  const [loading, setLoading] = useState(false);
  const [googleLoading, setGoogleLoading] = useState(false);

  function handleChange(e: React.ChangeEvent<HTMLInputElement>) {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      await login(form.email, form.password);
      router.push(redirect);
    } catch {
      setError("Email ou mot de passe incorrect.");
    } finally {
      setLoading(false);
    }
  }

  async function handleGoogle() {
    setError("");
    setGoogleLoading(true);
    try {
      await loginWithGoogle();
      router.push(redirect);
    } catch {
      setError("Connexion Google annulée ou échouée.");
    } finally {
      setGoogleLoading(false);
    }
  }

  return (
    <div className="glass-panel p-8 rounded-xl neon-border">
      <h1
        className="text-3xl font-bold text-on-surface mb-2"
        style={{ fontFamily: "var(--font-heading)" }}
      >
        Connexion
      </h1>
      <p className="text-sm text-on-surface-variant mb-8">
        Accédez à votre espace client TradeFlow.
      </p>

      {/* Google button */}
      <button
        type="button"
        onClick={handleGoogle}
        disabled={googleLoading || loading}
        className="w-full flex items-center justify-center gap-3 bg-surface-container border border-outline-variant py-3 text-sm text-on-surface hover:border-secondary/60 hover:bg-surface-container-high transition-all disabled:opacity-60 mb-6"
      >
        <svg width="18" height="18" viewBox="0 0 48 48" fill="none">
          <path d="M43.611 20.083H42V20H24v8h11.303c-1.649 4.657-6.08 8-11.303 8-6.627 0-12-5.373-12-12s5.373-12 12-12c3.059 0 5.842 1.154 7.961 3.039l5.657-5.657C34.046 6.053 29.268 4 24 4 12.955 4 4 12.955 4 24s8.955 20 20 20 20-8.955 20-20c0-1.341-.138-2.65-.389-3.917z" fill="#FFC107"/>
          <path d="M6.306 14.691l6.571 4.819C14.655 15.108 18.961 12 24 12c3.059 0 5.842 1.154 7.961 3.039l5.657-5.657C34.046 6.053 29.268 4 24 4 16.318 4 9.656 8.337 6.306 14.691z" fill="#FF3D00"/>
          <path d="M24 44c5.166 0 9.86-1.977 13.409-5.192l-6.19-5.238C29.211 35.091 26.715 36 24 36c-5.202 0-9.619-3.317-11.283-7.946l-6.522 5.025C9.505 39.556 16.227 44 24 44z" fill="#4CAF50"/>
          <path d="M43.611 20.083H42V20H24v8h11.303c-.792 2.237-2.231 4.166-4.087 5.571l6.19 5.238C42.012 35.245 44 30 44 24c0-1.341-.138-2.65-.389-3.917z" fill="#1976D2"/>
        </svg>
        {googleLoading ? "Connexion..." : "Continuer avec Google"}
      </button>

      <div className="flex items-center gap-3 mb-6">
        <div className="flex-1 h-px bg-outline-variant/40" />
        <span className="text-xs text-outline uppercase tracking-widest">ou</span>
        <div className="flex-1 h-px bg-outline-variant/40" />
      </div>

      <form onSubmit={handleSubmit} className="space-y-4">
        <div>
          <label
            className="block text-xs uppercase tracking-widest text-on-surface-variant mb-2"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Email
          </label>
          <input
            type="email"
            name="email"
            required
            value={form.email}
            onChange={handleChange}
            placeholder="votre@email.com"
            className="w-full bg-surface-container-lowest border border-outline-variant py-3 px-4 text-sm text-on-surface placeholder:text-outline focus:border-secondary-container focus:ring-1 focus:ring-secondary-container outline-none"
          />
        </div>

        <div>
          <label
            className="block text-xs uppercase tracking-widest text-on-surface-variant mb-2"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Mot de passe
          </label>
          <input
            type="password"
            name="password"
            required
            value={form.password}
            onChange={handleChange}
            placeholder="••••••••"
            className="w-full bg-surface-container-lowest border border-outline-variant py-3 px-4 text-sm text-on-surface placeholder:text-outline focus:border-secondary-container focus:ring-1 focus:ring-secondary-container outline-none"
          />
        </div>

        {error && <p className="text-xs text-error">{error}</p>}

        <button
          type="submit"
          disabled={loading || googleLoading}
          className="w-full bg-secondary-container text-on-secondary-container py-3 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_20px_rgba(5,102,217,0.4)] transition-all disabled:opacity-60"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          {loading ? "Connexion..." : "Se connecter"}
        </button>
      </form>

      <div className="mt-6 pt-6 border-t border-outline-variant text-center">
        <p className="text-sm text-outline">
          Pas encore de compte ?{" "}
          <Link href="/auth/register" className="text-secondary hover:underline">
            Créer un compte
          </Link>
        </p>
      </div>
    </div>
  );
}

export default function LoginPage() {
  return (
    <div className="w-full max-w-md">
      <Suspense fallback={<div className="glass-panel p-8 rounded-xl h-96 animate-pulse" />}>
        <LoginForm />
      </Suspense>
    </div>
  );
}
