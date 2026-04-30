"use client";

import Link from "next/link";
import { useEffect, useRef, useState } from "react";
import {
  getAdminProducts,
  publishProduct,
  archiveProduct,
  uploadProductFile,
  type AdminProduct,
} from "@/lib/api";

const STATUS_LABEL: Record<string, string> = {
  Draft: "Brouillon",
  Published: "Publié",
  Archived: "Archivé",
};
const STATUS_CLASS: Record<string, string> = {
  Draft: "text-on-surface-variant bg-surface-container",
  Published: "text-tertiary bg-tertiary-container/40",
  Archived: "text-outline bg-surface-container",
};

export default function AdminProductsPage() {
  const [products, setProducts] = useState<AdminProduct[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [acting, setActing] = useState<string | null>(null);
  const fileRefs = useRef<Record<string, HTMLInputElement | null>>({});

  function load(p = page, s = statusFilter) {
    setLoading(true);
    getAdminProducts(p, s || undefined)
      .then((r) => { setProducts(r.items); setTotal(r.total); })
      .catch(() => setError("Impossible de charger les produits."))
      .finally(() => setLoading(false));
  }

  useEffect(() => { load(); }, [page, statusFilter]); // eslint-disable-line react-hooks/exhaustive-deps

  async function handlePublish(id: string) {
    setActing(id);
    try {
      await publishProduct(id);
      setProducts((prev) => prev.map((p) => p.id === id ? { ...p, status: "Published" } : p));
    } catch (e: unknown) { setError(e instanceof Error ? e.message : "Erreur"); }
    finally { setActing(null); }
  }

  async function handleArchive(id: string) {
    setActing(id);
    try {
      await archiveProduct(id);
      setProducts((prev) => prev.map((p) => p.id === id ? { ...p, status: "Archived" } : p));
    } catch (e: unknown) { setError(e instanceof Error ? e.message : "Erreur"); }
    finally { setActing(null); }
  }

  async function handleFileUpload(id: string, file: File) {
    setActing(id);
    setError("");
    try {
      await uploadProductFile(id, file);
      setProducts((prev) => prev.map((p) => p.id === id ? { ...p, fileUrl: "uploaded" } : p));
    } catch (e: unknown) { setError(e instanceof Error ? e.message : "Erreur upload"); }
    finally { setActing(null); }
  }

  const totalPages = Math.ceil(total / 20);

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row gap-4 justify-between items-start sm:items-center">
        <p className="text-sm text-on-surface-variant">{total} produit{total !== 1 ? "s" : ""}</p>
        <div className="flex gap-3">
          {["", "Draft", "Published", "Archived"].map((s) => (
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
              {s || "Tous"}
            </button>
          ))}
          <Link
            href="/admin/products/new"
            className="bg-secondary-container text-on-secondary-container px-4 py-1.5 text-xs uppercase tracking-widest font-semibold hover:shadow-[0_0_15px_rgba(5,102,217,0.3)] transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            + Nouveau
          </Link>
        </div>
      </div>

      {error && (
        <div className="glass-panel p-3 rounded-lg border border-error/30">
          <p className="text-sm text-error">{error}</p>
        </div>
      )}

      {loading ? (
        <div className="space-y-3">
          {[1, 2, 3, 4].map((i) => (
            <div key={i} className="h-20 bg-surface-container animate-pulse rounded-xl" />
          ))}
        </div>
      ) : products.length === 0 ? (
        <div className="glass-panel p-12 rounded-xl text-center">
          <p className="text-on-surface-variant mb-4">Aucun produit.</p>
          <Link
            href="/admin/products/new"
            className="bg-secondary-container text-on-secondary-container px-6 py-2 uppercase tracking-widest text-xs font-semibold"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Créer le premier produit
          </Link>
        </div>
      ) : (
        <div className="space-y-3">
          {products.map((product) => {
            const busy = acting === product.id;
            return (
              <div key={product.id} className="glass-panel p-4 rounded-xl flex flex-col sm:flex-row sm:items-center gap-4">
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-3 mb-1">
                    <h3
                      className="text-sm font-semibold text-on-surface truncate"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {product.title}
                    </h3>
                    <span
                      className={`text-[10px] uppercase tracking-widest px-2 py-0.5 shrink-0 ${STATUS_CLASS[product.status] ?? "text-outline"}`}
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {STATUS_LABEL[product.status] ?? product.status}
                    </span>
                  </div>
                  <p className="text-xs text-outline">
                    {product.type} · {product.price} {product.currency} ·{" "}
                    {product.salesCount} vente{product.salesCount !== 1 ? "s" : ""}
                    {!product.fileUrl && (
                      <span className="ml-2 text-error">⚠ Pas de fichier</span>
                    )}
                  </p>
                </div>

                {/* Actions */}
                <div className="flex items-center gap-2 shrink-0 flex-wrap">
                  {/* File upload */}
                  <input
                    ref={(el) => { fileRefs.current[product.id] = el; }}
                    type="file"
                    accept=".pine,.pdf"
                    className="hidden"
                    onChange={(e) => {
                      const f = e.target.files?.[0];
                      if (f) handleFileUpload(product.id, f);
                      e.target.value = "";
                    }}
                  />
                  <button
                    onClick={() => fileRefs.current[product.id]?.click()}
                    disabled={busy}
                    className="text-xs uppercase tracking-widest text-on-surface-variant border border-outline-variant px-3 py-1.5 hover:border-outline transition-all disabled:opacity-50"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    {busy ? "..." : product.fileUrl ? "↑ Remplacer" : "↑ Upload"}
                  </button>

                  {product.status === "Draft" && (
                    <button
                      onClick={() => handlePublish(product.id)}
                      disabled={busy || !product.fileUrl}
                      title={!product.fileUrl ? "Upload un fichier d'abord" : ""}
                      className="text-xs uppercase tracking-widest text-tertiary border border-tertiary/30 px-3 py-1.5 hover:bg-tertiary/10 transition-all disabled:opacity-50"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      Publier
                    </button>
                  )}
                  {product.status === "Published" && (
                    <button
                      onClick={() => handleArchive(product.id)}
                      disabled={busy}
                      className="text-xs uppercase tracking-widest text-outline border border-outline-variant px-3 py-1.5 hover:border-outline transition-all disabled:opacity-50"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      Archiver
                    </button>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}

      {/* Pagination */}
      {totalPages > 1 && (
        <div className="flex items-center justify-center gap-2 pt-4">
          <button
            onClick={() => setPage((p) => Math.max(1, p - 1))}
            disabled={page === 1}
            className="px-4 py-2 text-xs uppercase tracking-widest text-on-surface-variant border border-outline-variant hover:border-outline disabled:opacity-40 transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            ← Préc
          </button>
          <span className="text-xs text-outline" style={{ fontFamily: "var(--font-heading)" }}>
            {page} / {totalPages}
          </span>
          <button
            onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
            disabled={page === totalPages}
            className="px-4 py-2 text-xs uppercase tracking-widest text-on-surface-variant border border-outline-variant hover:border-outline disabled:opacity-40 transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Suiv →
          </button>
        </div>
      )}
    </div>
  );
}
