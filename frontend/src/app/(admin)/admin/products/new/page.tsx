"use client";

import { useRouter } from "next/navigation";
import { useState } from "react";
import { createProduct } from "@/lib/api";

const TYPES = ["Indicator", "Strategy", "Bundle"];
const CURRENCIES = ["EUR", "USD"];

const inputClass =
  "w-full bg-surface-container-lowest border border-outline-variant py-3 px-4 text-sm text-on-surface placeholder:text-outline focus:border-secondary-container focus:ring-1 focus:ring-secondary-container outline-none";
const labelClass =
  "block text-xs uppercase tracking-widest text-on-surface-variant mb-2";

export default function NewProductPage() {
  const router = useRouter();
  const [form, setForm] = useState({
    title: "",
    shortDescription: "",
    longDescriptionMarkdown: "",
    type: "Indicator",
    price: "",
    currency: "EUR",
    categoryId: "",
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  function handleChange(
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const { id } = await createProduct({
        ...form,
        price: parseFloat(form.price),
        categoryId: form.categoryId || undefined,
      });
      router.push(`/admin/products?created=${id}`);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Erreur lors de la création.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="max-w-2xl space-y-8">
      <div>
        <h2
          className="text-2xl font-semibold text-on-surface mb-1"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          Nouveau produit
        </h2>
        <p className="text-sm text-on-surface-variant">
          Créé en statut <span className="text-on-surface font-semibold">Brouillon</span> — uploadez le fichier puis publiez.
        </p>
      </div>

      <form onSubmit={handleSubmit} className="space-y-5">
        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>Titre *</label>
          <input
            type="text" name="title" required value={form.title}
            onChange={handleChange} placeholder="Ex: RSI Divergence Pro"
            className={inputClass}
          />
        </div>

        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>Description courte *</label>
          <input
            type="text" name="shortDescription" required value={form.shortDescription}
            onChange={handleChange} placeholder="Résumé en une ligne visible sur les cards"
            className={inputClass}
          />
        </div>

        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>Description longue (Markdown)</label>
          <textarea
            name="longDescriptionMarkdown" rows={8} value={form.longDescriptionMarkdown}
            onChange={handleChange}
            placeholder="# Description complète&#10;&#10;Décrivez les fonctionnalités, l'utilisation, les marchés compatibles..."
            className={inputClass + " resize-none font-mono text-xs"}
          />
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>Type *</label>
            <select name="type" value={form.type} onChange={handleChange} className={inputClass}>
              {TYPES.map((t) => <option key={t} value={t}>{t}</option>)}
            </select>
          </div>
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>Prix *</label>
            <input
              type="number" name="price" required min="0" step="0.01"
              value={form.price} onChange={handleChange} placeholder="49.00"
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>Devise</label>
            <select name="currency" value={form.currency} onChange={handleChange} className={inputClass}>
              {CURRENCIES.map((c) => <option key={c} value={c}>{c}</option>)}
            </select>
          </div>
        </div>

        {error && (
          <div className="glass-panel p-3 rounded-lg border border-error/30">
            <p className="text-sm text-error">{error}</p>
          </div>
        )}

        <div className="flex gap-4 pt-2">
          <button
            type="button" onClick={() => router.back()}
            className="px-6 py-3 border border-outline-variant text-on-surface-variant uppercase tracking-widest text-xs font-semibold hover:border-outline transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Annuler
          </button>
          <button
            type="submit" disabled={loading}
            className="flex-1 bg-secondary-container text-on-secondary-container py-3 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_20px_rgba(5,102,217,0.4)] transition-all disabled:opacity-60"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            {loading ? "Création..." : "Créer le produit"}
          </button>
        </div>
      </form>
    </div>
  );
}
