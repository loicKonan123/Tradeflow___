"use client";

import { useState, useRef } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { useAuth } from "@/contexts/AuthContext";
import { submitProject, uploadProjectAttachment } from "@/lib/api";

const STEPS = [
  {
    n: "01",
    title: "Décrivez votre stratégie",
    desc: "Expliquez en langage naturel votre idée de trading — signaux d'entrée, sorties, gestion du risque.",
  },
  {
    n: "02",
    title: "Recevez un devis",
    desc: "On analyse votre brief et vous envoie un devis détaillé sous 48h. Acompte de 50% pour démarrer.",
  },
  {
    n: "03",
    title: "Développement & livraison",
    desc: "Code Pine Script v5 optimisé + PDF guide. Livré via lien sécurisé. Révisions incluses.",
  },
];

const TIMEFRAMES = ["1m", "5m", "15m", "30m", "1h", "4h", "Daily", "Weekly"];
const STRATEGY_TYPES = ["Trend Following", "Mean Reversion", "Breakout", "Scalping", "Swing Trading", "Arbitrage"];
const BUDGET_RANGES = ["< 200 €", "200 – 500 €", "500 – 1 000 €", "> 1 000 €"];

const input =
  "w-full bg-[#000f21] border border-[#45464d] py-3 px-4 text-sm text-[#d3e4fe] placeholder:text-[#909097] focus:border-[#0566d9] focus:ring-1 focus:ring-[#0566d9] outline-none";
const label =
  "block text-xs uppercase tracking-widest text-[#c6c6cd] mb-2";

export default function SurMesurePage() {
  const { user } = useAuth();
  const router = useRouter();
  const fileRef = useRef<HTMLInputElement>(null);

  const [form, setForm] = useState({
    strategyTitle: "",
    market: "",
    timeframe: "",
    indicators: "",
    entryConditions: "",
    exitConditions: "",
    riskManagement: "",
    strategyType: "",
    budgetRange: "",
    tradingViewChartUrl: "",
    desiredDeadline: "",
    additionalNotes: "",
  });
  const [file, setFile] = useState<File | null>(null);
  const [submitted, setSubmitted] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  function handleChange(e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement | HTMLSelectElement>) {
    setForm((f) => ({ ...f, [e.target.name]: e.target.value }));
  }

  async function handleSubmit(e: React.FormEvent<HTMLFormElement>) {
    e.preventDefault();
    setError("");

    if (!user) {
      // Redirect to register — user will complete via dashboard after signing up
      router.push("/auth/register?redirect=/dashboard/projects/new");
      return;
    }

    setLoading(true);
    try {
      const { projectId } = await submitProject(form);
      if (file && projectId) {
        await uploadProjectAttachment(projectId, file);
      }
      setSubmitted(true);
    } catch (err: unknown) {
      setError(err instanceof Error ? err.message : "Erreur lors de la soumission.");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="trading-bg pt-24 pb-16 min-h-screen">
      {/* Hero */}
      <section className="px-8 py-16 max-w-screen-2xl mx-auto">
        <div className="max-w-3xl">
          <div className="inline-flex items-center gap-2 px-3 py-1 bg-[#26364a] border border-[#45464d] rounded-full mb-6">
            <span className="w-2 h-2 bg-[#0566d9] rounded-full" />
            <span
              className="text-[#adc6ff] uppercase tracking-widest text-xs font-semibold"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Développement Sur-mesure
            </span>
          </div>
          <h1
            className="text-5xl font-bold leading-[1.1] text-[#d3e4fe] mb-4"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Votre stratégie,{" "}
            <span className="text-[#adc6ff]">codée à la perfection</span>
          </h1>
          <p className="text-[#c6c6cd] text-lg leading-relaxed">
            Décrivez votre idée en langage naturel. On transforme en script
            Pine Script v5 robuste avec rapport de backtest complet.
          </p>
        </div>
      </section>

      {/* Steps */}
      <section className="px-8 pb-16 max-w-screen-2xl mx-auto">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
          {STEPS.map((s) => (
            <div key={s.n} className="glass-panel p-6 rounded-xl">
              <div
                className="text-4xl font-bold text-[#0566d9] mb-4"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                {s.n}
              </div>
              <h3
                className="text-lg font-semibold text-[#d3e4fe] mb-2"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                {s.title}
              </h3>
              <p className="text-sm text-[#c6c6cd]">{s.desc}</p>
            </div>
          ))}
        </div>
      </section>

      {/* Form */}
      <section className="px-8 max-w-screen-2xl mx-auto">
        <div className="max-w-2xl">
          <h2
            className="text-[2rem] font-semibold leading-[1.2] text-[#d3e4fe] mb-8"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Soumettre un brief
          </h2>

          {submitted ? (
            <div className="glass-panel p-8 rounded-xl border border-[#4ae176]/30 text-center">
              <div className="text-[#4ae176] text-4xl mb-4">✓</div>
              <h3
                className="text-2xl font-semibold text-[#d3e4fe] mb-2"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Brief reçu !
              </h3>
              <p className="text-[#c6c6cd] mb-6">
                Loïc vous contactera sous 48h avec un devis détaillé.
              </p>
              <Link
                href="/dashboard/projects"
                className="text-xs uppercase tracking-widest text-[#adc6ff] hover:underline"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Voir mes projets →
              </Link>
            </div>
          ) : (
            <form onSubmit={handleSubmit} className="space-y-6">

              {/* Titre */}
              <div>
                <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                  Titre de la stratégie *
                </label>
                <input
                  type="text"
                  name="strategyTitle"
                  required
                  value={form.strategyTitle}
                  onChange={handleChange}
                  placeholder="Ex: Stratégie breakout sur BTC avec filtrage RSI"
                  className={input}
                />
              </div>

              {/* Marché + Timeframe */}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                    Marché *
                  </label>
                  <input
                    type="text"
                    name="market"
                    required
                    value={form.market}
                    onChange={handleChange}
                    placeholder="Ex: BTC/USD, NQ1!, EUR/USD"
                    className={input}
                  />
                </div>
                <div>
                  <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                    Timeframe *
                  </label>
                  <select
                    name="timeframe"
                    required
                    value={form.timeframe}
                    onChange={handleChange}
                    className={input}
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
                <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                  Indicateurs TradingView utilisés *
                </label>
                <input
                  type="text"
                  name="indicators"
                  required
                  value={form.indicators}
                  onChange={handleChange}
                  placeholder="Ex: RSI 14, EMA 20/50, MACD, Bollinger Bands..."
                  className={input}
                />
                <p className="text-xs text-[#909097] mt-1">Séparez les indicateurs par des virgules</p>
              </div>

              {/* Type + Budget */}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                    Type de stratégie
                  </label>
                  <select
                    name="strategyType"
                    value={form.strategyType}
                    onChange={handleChange}
                    className={input}
                  >
                    <option value="">Sélectionner...</option>
                    {STRATEGY_TYPES.map((t) => (
                      <option key={t} value={t}>{t}</option>
                    ))}
                  </select>
                </div>
                <div>
                  <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                    Budget estimé
                  </label>
                  <select
                    name="budgetRange"
                    value={form.budgetRange}
                    onChange={handleChange}
                    className={input}
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
                <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                  Conditions d&apos;entrée *
                </label>
                <textarea
                  name="entryConditions"
                  required
                  rows={4}
                  value={form.entryConditions}
                  onChange={handleChange}
                  placeholder="Croisement d'indicateurs, niveaux de prix, confirmations..."
                  className={input + " resize-none"}
                />
              </div>

              {/* Conditions de sortie */}
              <div>
                <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                  Conditions de sortie *
                </label>
                <textarea
                  name="exitConditions"
                  required
                  rows={3}
                  value={form.exitConditions}
                  onChange={handleChange}
                  placeholder="Stop-loss, take-profit, trailing stop..."
                  className={input + " resize-none"}
                />
              </div>

              {/* Gestion du risque */}
              <div>
                <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                  Gestion du risque *
                </label>
                <textarea
                  name="riskManagement"
                  required
                  rows={2}
                  value={form.riskManagement}
                  onChange={handleChange}
                  placeholder="% du capital par trade, drawdown max, taille de position..."
                  className={input + " resize-none"}
                />
              </div>

              {/* Lien chart + Délai */}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                    Lien chart TradingView
                  </label>
                  <input
                    type="url"
                    name="tradingViewChartUrl"
                    value={form.tradingViewChartUrl}
                    onChange={handleChange}
                    placeholder="https://www.tradingview.com/chart/..."
                    className={input}
                  />
                </div>
                <div>
                  <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                    Délai souhaité
                  </label>
                  <input
                    type="date"
                    name="desiredDeadline"
                    value={form.desiredDeadline}
                    onChange={handleChange}
                    className={input}
                  />
                </div>
              </div>

              {/* Pièce jointe */}
              <div>
                <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                  Pièce jointe (capture d&apos;écran, PDF...)
                </label>
                <div
                  className="border border-dashed border-[#45464d] rounded-lg p-6 text-center cursor-pointer hover:border-[#0566d9]/60 transition-colors"
                  onClick={() => fileRef.current?.click()}
                >
                  {file ? (
                    <div className="flex items-center justify-center gap-3">
                      <span className="text-[#4ae176] text-lg">✓</span>
                      <span className="text-sm text-[#d3e4fe]">{file.name}</span>
                      <button
                        type="button"
                        onClick={(e) => { e.stopPropagation(); setFile(null); }}
                        className="text-[#909097] hover:text-[#ff5449] text-xs ml-2"
                      >
                        ×
                      </button>
                    </div>
                  ) : (
                    <div>
                      <p className="text-sm text-[#c6c6cd]">Cliquez pour sélectionner un fichier</p>
                      <p className="text-xs text-[#909097] mt-1">PNG, JPG, PDF — max 10 Mo</p>
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
                <label className={label} style={{ fontFamily: "var(--font-heading)" }}>
                  Notes additionnelles
                </label>
                <textarea
                  name="additionalNotes"
                  rows={3}
                  value={form.additionalNotes}
                  onChange={handleChange}
                  placeholder="Inspirations, contraintes spécifiques, livrables souhaités..."
                  className={input + " resize-none"}
                />
              </div>

              <div className="glass-panel p-4 rounded-lg border border-[#adc6ff]/20">
                <p className="text-xs text-[#c6c6cd]">
                  <span className="text-[#adc6ff] font-semibold">Processus :</span>{" "}
                  Soumission → devis sous 48h → acompte{" "}
                  <span className="text-[#d3e4fe] font-semibold">50%</span> pour démarrer → développement → livraison .pine + PDF → solde.
                </p>
              </div>

              {!user && (
                <div className="glass-panel p-4 rounded-lg border border-[#adc6ff]/20">
                  <p className="text-xs text-[#c6c6cd]">
                    <span className="text-[#adc6ff] font-semibold">Note :</span>{" "}
                    Vous devez être connecté pour soumettre.{" "}
                    <Link href="/auth/register" className="text-[#adc6ff] hover:underline">
                      Créer un compte gratuitement
                    </Link>{" "}
                    ou{" "}
                    <Link href="/auth/login" className="text-[#adc6ff] hover:underline">
                      se connecter
                    </Link>.
                  </p>
                </div>
              )}

              {error && (
                <p className="text-xs text-[#ff5449]">{error}</p>
              )}

              <button
                type="submit"
                disabled={loading}
                className="w-full bg-[#0566d9] text-[#e6ecff] py-4 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_20px_rgba(5,102,217,0.4)] transition-all disabled:opacity-60"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                {loading
                  ? "Soumission en cours..."
                  : user
                  ? "Soumettre le brief"
                  : "Continuer — créer un compte"}
              </button>
            </form>
          )}
        </div>
      </section>
    </div>
  );
}
