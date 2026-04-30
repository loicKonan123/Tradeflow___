"use client";

import { useEffect, useRef, useState } from "react";
import {
  getAdminProjects,
  sendQuote,
  startProjectWork,
  deliverProject,
  type AdminProject,
  type ProjectStatus,
} from "@/lib/api";

const STATUS_LABEL: Record<string, string> = {
  Submitted: "Soumis",
  Quoted: "Devis envoyé",
  DepositPaid: "Acompte reçu",
  InProgress: "En cours",
  Delivered: "Livré",
  Cancelled: "Annulé",
};
const STATUS_CLASS: Record<string, string> = {
  Submitted: "text-secondary bg-secondary-container/20",
  Quoted: "text-tertiary bg-tertiary-container/20",
  DepositPaid: "text-tertiary bg-tertiary-container/40",
  InProgress: "text-secondary bg-secondary-container/20",
  Delivered: "text-tertiary bg-tertiary-container/40",
  Cancelled: "text-error bg-error-container/20",
};

export default function AdminProjectsPage() {
  const [projects, setProjects] = useState<AdminProject[]>([]);
  const [total, setTotal] = useState(0);
  const [page, setPage] = useState(1);
  const [statusFilter, setStatusFilter] = useState("");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [acting, setActing] = useState<string | null>(null);
  const [quoteForm, setQuoteForm] = useState<{ id: string; price: string; notes: string } | null>(null);
  const [deliverForm, setDeliverForm] = useState<{ id: string; notes: string } | null>(null);
  const fileRef = useRef<HTMLInputElement | null>(null);

  useEffect(() => {
    setLoading(true);
    getAdminProjects(page, statusFilter || undefined)
      .then((r) => { setProjects(r.items); setTotal(r.total); })
      .catch(() => setError("Impossible de charger les projets."))
      .finally(() => setLoading(false));
  }, [page, statusFilter]);

  async function handleSendQuote() {
    if (!quoteForm) return;
    setActing(quoteForm.id);
    try {
      await sendQuote(quoteForm.id, parseFloat(quoteForm.price), "EUR", quoteForm.notes || undefined);
      setProjects((prev) => prev.map((p) => p.id === quoteForm.id ? { ...p, status: "Quoted" as ProjectStatus } : p));
      setQuoteForm(null);
    } catch (e: unknown) { setError(e instanceof Error ? e.message : "Erreur"); }
    finally { setActing(null); }
  }

  async function handleStart(id: string) {
    setActing(id);
    try {
      await startProjectWork(id);
      setProjects((prev) => prev.map((p) => p.id === id ? { ...p, status: "InProgress" as ProjectStatus } : p));
    } catch (e: unknown) { setError(e instanceof Error ? e.message : "Erreur"); }
    finally { setActing(null); }
  }

  async function handleDeliver(file: File) {
    if (!deliverForm) return;
    setActing(deliverForm.id);
    try {
      await deliverProject(deliverForm.id, file, deliverForm.notes || undefined);
      setProjects((prev) => prev.map((p) => p.id === deliverForm.id ? { ...p, status: "Delivered" as ProjectStatus } : p));
      setDeliverForm(null);
    } catch (e: unknown) { setError(e instanceof Error ? e.message : "Erreur"); }
    finally { setActing(null); }
  }

  const totalPages = Math.ceil(total / 20);

  return (
    <div className="space-y-6">
      {/* Quote modal */}
      {quoteForm && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <div className="glass-panel p-8 rounded-xl w-full max-w-md neon-border">
            <h3 className="text-lg font-semibold text-on-surface mb-6" style={{ fontFamily: "var(--font-heading)" }}>
              Envoyer un devis
            </h3>
            <div className="space-y-4">
              <div>
                <label className="block text-xs uppercase tracking-widest text-on-surface-variant mb-2" style={{ fontFamily: "var(--font-heading)" }}>
                  Prix (EUR) *
                </label>
                <input
                  type="number" min="0" step="0.01" required
                  value={quoteForm.price}
                  onChange={(e) => setQuoteForm((f) => f && { ...f, price: e.target.value })}
                  placeholder="149.00"
                  className="w-full bg-surface-container-lowest border border-outline-variant py-3 px-4 text-sm text-on-surface placeholder:text-outline focus:border-secondary-container outline-none"
                />
              </div>
              <div>
                <label className="block text-xs uppercase tracking-widest text-on-surface-variant mb-2" style={{ fontFamily: "var(--font-heading)" }}>
                  Note pour le client
                </label>
                <textarea
                  rows={3} value={quoteForm.notes}
                  onChange={(e) => setQuoteForm((f) => f && { ...f, notes: e.target.value })}
                  placeholder="Délai estimé, détails techniques..."
                  className="w-full bg-surface-container-lowest border border-outline-variant py-3 px-4 text-sm text-on-surface placeholder:text-outline focus:border-secondary-container outline-none resize-none"
                />
              </div>
              <p className="text-xs text-on-surface-variant">
                Acompte calculé automatiquement : <span className="text-on-surface font-semibold">
                  {quoteForm.price ? (parseFloat(quoteForm.price) * 0.5).toFixed(2) : "—"} EUR (50%)
                </span>
              </p>
            </div>
            <div className="flex gap-3 mt-6">
              <button
                onClick={() => setQuoteForm(null)}
                className="flex-1 py-2.5 border border-outline-variant text-on-surface-variant text-xs uppercase tracking-widest font-semibold hover:border-outline transition-all"
                style={{ fontFamily: "var(--font-heading)" }}
              >Annuler</button>
              <button
                onClick={handleSendQuote}
                disabled={!quoteForm.price || !!acting}
                className="flex-1 py-2.5 bg-secondary-container text-on-secondary-container text-xs uppercase tracking-widest font-semibold hover:shadow-[0_0_15px_rgba(5,102,217,0.3)] transition-all disabled:opacity-60"
                style={{ fontFamily: "var(--font-heading)" }}
              >Envoyer</button>
            </div>
          </div>
        </div>
      )}

      {/* Deliver modal */}
      {deliverForm && (
        <div className="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <div className="glass-panel p-8 rounded-xl w-full max-w-md neon-border">
            <h3 className="text-lg font-semibold text-on-surface mb-6" style={{ fontFamily: "var(--font-heading)" }}>
              Livrer le projet
            </h3>
            <div className="space-y-4">
              <div>
                <label className="block text-xs uppercase tracking-widest text-on-surface-variant mb-2" style={{ fontFamily: "var(--font-heading)" }}>
                  Fichier .pine / PDF *
                </label>
                <input
                  ref={fileRef} type="file" accept=".pine,.pdf"
                  className="w-full bg-surface-container-lowest border border-outline-variant py-3 px-4 text-sm text-on-surface-variant file:mr-4 file:py-1 file:px-3 file:border-0 file:bg-surface-container file:text-on-surface-variant file:text-xs file:uppercase file:tracking-widest file:font-semibold"
                />
              </div>
              <div>
                <label className="block text-xs uppercase tracking-widest text-on-surface-variant mb-2" style={{ fontFamily: "var(--font-heading)" }}>
                  Notes de livraison
                </label>
                <textarea
                  rows={3} value={deliverForm.notes}
                  onChange={(e) => setDeliverForm((f) => f && { ...f, notes: e.target.value })}
                  placeholder="Instructions d'utilisation, particularités..."
                  className="w-full bg-surface-container-lowest border border-outline-variant py-3 px-4 text-sm text-on-surface placeholder:text-outline focus:border-secondary-container outline-none resize-none"
                />
              </div>
            </div>
            <div className="flex gap-3 mt-6">
              <button
                onClick={() => setDeliverForm(null)}
                className="flex-1 py-2.5 border border-outline-variant text-on-surface-variant text-xs uppercase tracking-widest font-semibold hover:border-outline transition-all"
                style={{ fontFamily: "var(--font-heading)" }}
              >Annuler</button>
              <button
                onClick={() => {
                  const f = fileRef.current?.files?.[0];
                  if (f) handleDeliver(f);
                }}
                disabled={!!acting}
                className="flex-1 py-2.5 bg-tertiary/20 text-tertiary border border-tertiary/30 text-xs uppercase tracking-widest font-semibold hover:bg-tertiary/30 transition-all disabled:opacity-60"
                style={{ fontFamily: "var(--font-heading)" }}
              >Livrer</button>
            </div>
          </div>
        </div>
      )}

      {/* Filters */}
      <div className="flex flex-col sm:flex-row gap-4 justify-between items-start sm:items-center">
        <p className="text-sm text-on-surface-variant">{total} projet{total !== 1 ? "s" : ""}</p>
        <div className="flex gap-2 flex-wrap">
          {["", "Submitted", "Quoted", "DepositPaid", "InProgress", "Delivered"].map((s) => (
            <button
              key={s}
              onClick={() => { setStatusFilter(s); setPage(1); }}
              className={`px-3 py-1.5 text-xs uppercase tracking-widest font-semibold transition-all ${
                statusFilter === s
                  ? "bg-secondary-container text-on-secondary-container"
                  : "bg-surface-container text-on-surface-variant hover:text-on-surface"
              }`}
              style={{ fontFamily: "var(--font-heading)" }}
            >
              {s ? (STATUS_LABEL[s] ?? s) : "Tous"}
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
          {[1, 2, 3].map((i) => <div key={i} className="h-36 bg-surface-container animate-pulse rounded-xl" />)}
        </div>
      ) : projects.length === 0 ? (
        <div className="glass-panel p-12 rounded-xl text-center">
          <p className="text-on-surface-variant">Aucun projet.</p>
        </div>
      ) : (
        <div className="space-y-4">
          {projects.map((project) => {
            const busy = acting === project.id;
            return (
              <div key={project.id} className="glass-panel p-5 rounded-xl space-y-4">
                <div className="flex flex-col sm:flex-row sm:items-start justify-between gap-3">
                  <div>
                    <div className="flex items-center gap-3 mb-1">
                      <span className="text-sm font-semibold text-on-surface" style={{ fontFamily: "var(--font-heading)" }}>
                        #{project.projectNumber}
                      </span>
                      <span
                        className={`text-[10px] uppercase tracking-widest px-2 py-0.5 ${STATUS_CLASS[project.status] ?? "text-outline"}`}
                        style={{ fontFamily: "var(--font-heading)" }}
                      >
                        {STATUS_LABEL[project.status] ?? project.status}
                      </span>
                    </div>
                    <p className="text-xs text-outline">
                      {project.customerEmail} · {project.market} · {project.timeframe} ·{" "}
                      {new Date(project.createdAt).toLocaleDateString("fr-FR")}
                    </p>
                  </div>
                  {project.quotedPrice && (
                    <p className="text-sm font-medium text-on-surface shrink-0" style={{ fontFamily: "var(--font-heading)" }}>
                      {project.quotedPrice} {project.quotedCurrency}
                      <span className="text-xs text-outline ml-1">(acompte {project.depositAmount})</span>
                    </p>
                  )}
                </div>

                {/* Brief preview */}
                <div className="grid grid-cols-1 md:grid-cols-2 gap-3 text-xs">
                  <div>
                    <p className="text-[10px] uppercase tracking-widest text-outline mb-1" style={{ fontFamily: "var(--font-heading)" }}>Entrée</p>
                    <p className="text-on-surface-variant line-clamp-2">{project.entryConditions}</p>
                  </div>
                  <div>
                    <p className="text-[10px] uppercase tracking-widest text-outline mb-1" style={{ fontFamily: "var(--font-heading)" }}>Risque</p>
                    <p className="text-on-surface-variant line-clamp-2">{project.riskManagement}</p>
                  </div>
                </div>

                {/* Actions */}
                <div className="flex items-center gap-2 pt-3 border-t border-outline-variant/20 flex-wrap">
                  {project.status === "Submitted" && (
                    <button
                      onClick={() => setQuoteForm({ id: project.id, price: "", notes: "" })}
                      disabled={busy}
                      className="text-xs uppercase tracking-widest text-secondary border border-secondary/30 px-4 py-2 hover:bg-secondary/10 transition-all disabled:opacity-50"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      Envoyer un devis
                    </button>
                  )}
                  {project.status === "DepositPaid" && (
                    <button
                      onClick={() => handleStart(project.id)}
                      disabled={busy}
                      className="text-xs uppercase tracking-widest text-tertiary border border-tertiary/30 px-4 py-2 hover:bg-tertiary/10 transition-all disabled:opacity-50"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {busy ? "..." : "Démarrer le travail"}
                    </button>
                  )}
                  {project.status === "InProgress" && (
                    <button
                      onClick={() => setDeliverForm({ id: project.id, notes: "" })}
                      disabled={busy}
                      className="text-xs uppercase tracking-widest text-tertiary bg-tertiary/10 border border-tertiary/30 px-4 py-2 hover:bg-tertiary/20 transition-all disabled:opacity-50"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      Livrer le fichier
                    </button>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}

      {totalPages > 1 && (
        <div className="flex items-center justify-center gap-2 pt-4">
          <button onClick={() => setPage((p) => Math.max(1, p - 1))} disabled={page === 1}
            className="px-4 py-2 text-xs uppercase tracking-widest text-on-surface-variant border border-outline-variant hover:border-outline disabled:opacity-40 transition-all"
            style={{ fontFamily: "var(--font-heading)" }}>← Préc</button>
          <span className="text-xs text-outline" style={{ fontFamily: "var(--font-heading)" }}>{page} / {totalPages}</span>
          <button onClick={() => setPage((p) => Math.min(totalPages, p + 1))} disabled={page === totalPages}
            className="px-4 py-2 text-xs uppercase tracking-widest text-on-surface-variant border border-outline-variant hover:border-outline disabled:opacity-40 transition-all"
            style={{ fontFamily: "var(--font-heading)" }}>Suiv →</button>
        </div>
      )}
    </div>
  );
}
