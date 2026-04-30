"use client";

import { useRouter } from "next/navigation";
import { useState, useRef } from "react";
import { submitProject, uploadProjectAttachment, type SubmitProjectPayload } from "@/lib/api";

const TIMEFRAMES = ["1m", "5m", "15m", "30m", "1h", "4h", "Daily", "Weekly"];
const STRATEGY_TYPES = ["Trend Following", "Mean Reversion", "Breakout", "Scalping", "Swing Trading", "Arbitrage"];
const BUDGET_RANGES = ["< 200 €", "200 – 500 €", "500 – 1 000 €", "> 1 000 €"];

const inputClass =
  "w-full bg-surface-container-lowest border border-outline-variant py-3 px-4 text-sm text-on-surface placeholder:text-outline focus:border-secondary-container focus:ring-1 focus:ring-secondary-container outline-none";
const labelClass =
  "block text-xs uppercase tracking-widest text-on-surface-variant mb-2";

export default function NewProjectPage() {
  const router = useRouter();
  const fileRef = useRef<HTMLInputElement>(null);

  const [form, setForm] = useState<SubmitProjectPayload>({
    strategyTitle: "",
    market: "",
    timeframe: "",
    entryConditions: "",
    exitConditions: "",
    riskManagement: "",
    indicators: "",
    additionalNotes: "",
    strategyType: "",
    tradingViewChartUrl: "",
    budgetRange: "",
    desiredDeadline: "",
  });
  const [file, setFile] = useState<File | null>(null);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  function handleChange(
    e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>
  ) {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError("");
    setLoading(true);
    try {
      const { projectId } = await submitProject(form);
      if (file && projectId) {
        await uploadProjectAttachment(projectId, file);
      }
      router.push("/dashboard/projects");
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Erreur lors de la soumission.");
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
          Nouveau projet sur-mesure
        </h2>
        <p className="text-sm text-on-surface-variant">
          Décrivez votre stratégie en détail. Loïc vous enverra un devis sous 48h.
        </p>
      </div>

      <form onSubmit={handleSubmit} className="space-y-6">

        {/* Titre */}
        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
            Titre de la stratégie *
          </label>
          <input
            type="text"
            name="strategyTitle"
            required
            value={form.strategyTitle}
            onChange={handleChange}
            placeholder="Ex: Stratégie de breakout BTC sur Daily"
            className={inputClass}
          />
        </div>

        {/* Marché + Timeframe */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
              Marché *
            </label>
            <input
              type="text"
              name="market"
              required
              value={form.market}
              onChange={handleChange}
              placeholder="Ex: BTC/USD, NQ1!, EUR/USD"
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
              Timeframe *
            </label>
            <select
              name="timeframe"
              required
              value={form.timeframe}
              onChange={handleChange}
              className={inputClass}
            >
              <option value="">Sélectionner...</option>
              {TIMEFRAMES.map((tf) => (
                <option key={tf} value={tf}>{tf}</option>
              ))}
            </select>
          </div>
        </div>

        {/* Indicateurs */}
        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
            Indicateurs TradingView utilisés *
          </label>
          <input
            type="text"
            name="indicators"
            required
            value={form.indicators}
            onChange={handleChange}
            placeholder="Ex: RSI 14, EMA 20/50, MACD, Bollinger Bands..."
            className={inputClass}
          />
          <p className="text-xs text-outline mt-1">Séparez les indicateurs par des virgules</p>
        </div>

        {/* Type de stratégie + Budget */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
              Type de stratégie
            </label>
            <select
              name="strategyType"
              value={form.strategyType}
              onChange={handleChange}
              className={inputClass}
            >
              <option value="">Sélectionner...</option>
              {STRATEGY_TYPES.map((t) => (
                <option key={t} value={t}>{t}</option>
              ))}
            </select>
          </div>
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
              Budget estimé
            </label>
            <select
              name="budgetRange"
              value={form.budgetRange}
              onChange={handleChange}
              className={inputClass}
            >
              <option value="">Sélectionner...</option>
              {BUDGET_RANGES.map((b) => (
                <option key={b} value={b}>{b}</option>
              ))}
            </select>
          </div>
        </div>

        {/* Conditions d'entrée */}
        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
            Conditions d&apos;entrée *
          </label>
          <textarea
            name="entryConditions"
            required
            rows={4}
            value={form.entryConditions}
            onChange={handleChange}
            placeholder="Décrivez précisément quand entrer en position : croisement d'indicateurs, niveaux de prix, confirmations..."
            className={inputClass + " resize-none"}
          />
        </div>

        {/* Conditions de sortie */}
        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
            Conditions de sortie *
          </label>
          <textarea
            name="exitConditions"
            required
            rows={4}
            value={form.exitConditions}
            onChange={handleChange}
            placeholder="Stop-loss, take-profit, trailing stop, conditions de clôture partielle..."
            className={inputClass + " resize-none"}
          />
        </div>

        {/* Gestion du risque */}
        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
            Gestion du risque *
          </label>
          <textarea
            name="riskManagement"
            required
            rows={3}
            value={form.riskManagement}
            onChange={handleChange}
            placeholder="% du capital par trade, drawdown max accepté, taille de position fixe ou dynamique..."
            className={inputClass + " resize-none"}
          />
        </div>

        {/* Lien chart + Délai */}
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
              Lien chart TradingView
            </label>
            <input
              type="url"
              name="tradingViewChartUrl"
              value={form.tradingViewChartUrl}
              onChange={handleChange}
              placeholder="https://www.tradingview.com/chart/..."
              className={inputClass}
            />
          </div>
          <div>
            <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
              Délai souhaité
            </label>
            <input
              type="date"
              name="desiredDeadline"
              value={form.desiredDeadline}
              onChange={handleChange}
              className={inputClass}
            />
          </div>
        </div>

        {/* Pièce jointe */}
        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
            Pièce jointe (capture d&apos;écran, PDF...)
          </label>
          <div
            className="border border-dashed border-outline-variant rounded-lg p-6 text-center cursor-pointer hover:border-secondary/60 transition-colors"
            onClick={() => fileRef.current?.click()}
          >
            {file ? (
              <div className="flex items-center justify-center gap-3">
                <span className="text-tertiary text-lg">✓</span>
                <span className="text-sm text-on-surface">{file.name}</span>
                <button
                  type="button"
                  onClick={(e) => { e.stopPropagation(); setFile(null); }}
                  className="text-outline hover:text-error text-xs ml-2"
                >
                  ×
                </button>
              </div>
            ) : (
              <div>
                <p className="text-sm text-on-surface-variant">Cliquez pour sélectionner un fichier</p>
                <p className="text-xs text-outline mt-1">PNG, JPG, PDF — max 10 Mo</p>
              </div>
            )}
          </div>
          <input
            ref={fileRef}
            type="file"
            accept="image/*,.pdf"
            className="hidden"
            onChange={(e) => setFile(e.target.files?.[0] ?? null)}
          />
        </div>

        {/* Notes */}
        <div>
          <label className={labelClass} style={{ fontFamily: "var(--font-heading)" }}>
            Notes additionnelles
          </label>
          <textarea
            name="additionalNotes"
            rows={3}
            value={form.additionalNotes}
            onChange={handleChange}
            placeholder="Inspirations, références, contraintes spécifiques, livrables souhaités..."
            className={inputClass + " resize-none"}
          />
        </div>

        <div className="glass-panel p-4 rounded-lg border border-secondary-container/20">
          <p className="text-xs text-on-surface-variant">
            <span className="text-secondary font-semibold">Processus :</span> Soumission → devis sous 48h → acompte{" "}
            <span className="text-on-surface font-semibold">50%</span> pour démarrer → développement → livraison .pine + PDF → solde.
          </p>
        </div>

        {error && (
          <div className="glass-panel p-4 rounded-lg border border-error/30">
            <p className="text-sm text-error">{error}</p>
          </div>
        )}

        <div className="flex gap-4">
          <button
            type="button"
            onClick={() => router.back()}
            className="px-6 py-3 border border-outline-variant text-on-surface-variant uppercase tracking-widest text-xs font-semibold hover:border-outline transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Annuler
          </button>
          <button
            type="submit"
            disabled={loading}
            className="flex-1 bg-secondary-container text-on-secondary-container py-3 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_20px_rgba(5,102,217,0.4)] transition-all disabled:opacity-60"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            {loading ? "Soumission en cours..." : "Soumettre le brief"}
          </button>
        </div>
      </form>
    </div>
  );
}
