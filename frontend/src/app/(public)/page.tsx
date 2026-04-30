import Link from "next/link";

export default function HomePage() {
  return (
    <div className="trading-bg pt-20">
      {/* Hero */}
      <section className="relative min-h-[819px] flex items-center overflow-hidden px-8">
        <div className="absolute inset-0 opacity-20 pointer-events-none">
          <div className="absolute top-1/4 -left-20 w-96 h-96 bg-[#0566d9] blur-[120px] rounded-full" />
          <div className="absolute bottom-1/4 -right-20 w-96 h-96 bg-[#4ae176] blur-[120px] rounded-full" />
        </div>

        <div className="max-w-screen-2xl mx-auto w-full grid grid-cols-1 lg:grid-cols-2 gap-16 items-center relative z-10">
          <div className="space-y-6">
            <div className="inline-flex items-center gap-2 px-3 py-1 bg-[#26364a] border border-[#45464d] rounded-full">
              <span className="w-2 h-2 bg-[#4ae176] rounded-full" />
              <span
                className="text-[#4ae176] uppercase tracking-widest text-xs font-semibold"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Pine Script v5 Mastery
              </span>
            </div>

            <h1
              className="text-5xl font-bold leading-[1.1] text-[#d3e4fe]"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Expertise Pine Script v5 &amp;{" "}
              <span className="text-[#adc6ff]">Stratégies Validées</span>
            </h1>

            <p className="text-[#c6c6cd] max-w-xl text-base leading-relaxed">
              Oubliez les générateurs IA. Misez sur la rigueur humaine et des
              backtests robustes pour des performances institutionnelles.
            </p>

            <div className="flex flex-wrap gap-4 pt-1">
              <Link
                href="/sur-mesure"
                className="bg-[#0566d9] text-[#e6ecff] px-8 py-4 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_20px_rgba(5,102,217,0.4)] transition-all"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Démarrer un projet
              </Link>
              <Link
                href="/catalogue"
                className="border border-[#adc6ff] text-[#adc6ff] px-8 py-4 uppercase tracking-widest text-xs font-semibold hover:bg-[#adc6ff]/10 transition-all"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Explorer la marketplace
              </Link>
            </div>
          </div>

          {/* Right card — placeholder chart aesthetic */}
          <div className="hidden lg:block relative">
            <div className="glass-panel p-4 rounded-xl neon-border">
              <div className="rounded-lg border border-[#45464d] bg-[#102034] h-72 flex items-center justify-center overflow-hidden">
                {/* Simulated trading terminal */}
                <svg viewBox="0 0 480 280" className="w-full h-full opacity-80">
                  <defs>
                    <linearGradient id="gr" x1="0" y1="0" x2="0" y2="1">
                      <stop offset="0%" stopColor="#4ae176" stopOpacity="0.3" />
                      <stop offset="100%" stopColor="#4ae176" stopOpacity="0" />
                    </linearGradient>
                  </defs>
                  {/* Grid lines */}
                  {[60, 120, 180, 240].map((y) => (
                    <line key={y} x1="0" y1={y} x2="480" y2={y} stroke="#1b2b3f" strokeWidth="1" />
                  ))}
                  {[80, 160, 240, 320, 400].map((x) => (
                    <line key={x} x1={x} y1="0" x2={x} y2="280" stroke="#1b2b3f" strokeWidth="1" />
                  ))}
                  {/* Price line */}
                  <polyline
                    points="0,200 60,180 120,160 160,175 200,140 240,120 280,130 320,100 360,80 420,90 480,60"
                    fill="none"
                    stroke="#4ae176"
                    strokeWidth="2"
                  />
                  <polygon
                    points="0,200 60,180 120,160 160,175 200,140 240,120 280,130 320,100 360,80 420,90 480,60 480,280 0,280"
                    fill="url(#gr)"
                  />
                  {/* Candles */}
                  {[
                    [40, 190, 175, 195, 170],
                    [100, 170, 155, 172, 152],
                    [160, 178, 160, 180, 158],
                    [220, 145, 125, 148, 122],
                    [280, 135, 118, 137, 115],
                    [340, 108, 88, 110, 85],
                    [400, 95, 78, 97, 75],
                    [460, 68, 52, 70, 50],
                  ].map(([x, open, close, high, low]) => (
                    <g key={x}>
                      <line x1={x} y1={high} x2={x} y2={low} stroke={close < open ? "#4ae176" : "#ffb4ab"} strokeWidth="1" />
                      <rect
                        x={x - 6}
                        y={Math.min(open, close)}
                        width={12}
                        height={Math.abs(open - close) || 2}
                        fill={close < open ? "#4ae176" : "#ffb4ab"}
                      />
                    </g>
                  ))}
                </svg>
              </div>
            </div>

            {/* Floating profit factor card */}
            <div className="absolute -bottom-10 -left-10 glass-panel p-4 rounded-lg border border-[#4ae176]/30 shadow-xl">
              <div
                className="text-[#4ae176] uppercase tracking-widest text-xs font-semibold mb-1"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                PROFIT FACTOR
              </div>
              <div
                className="text-2xl font-medium text-[#d3e4fe]"
                style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
              >
                2.84
              </div>
              <div className="w-full bg-[#1b2b3f] h-1 mt-2">
                <div
                  className="bg-[#4ae176] h-full shadow-[0_0_8px_#4ae176]"
                  style={{ width: "84%" }}
                />
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Trust Banner */}
      <section className="py-10 border-y border-slate-900 bg-[#000f21]/50">
        <div className="max-w-screen-2xl mx-auto px-8 flex flex-col md:flex-row justify-between items-center gap-6 opacity-60 grayscale">
          <div
            className="uppercase tracking-widest text-xs text-[#c6c6cd]"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            TECH STACK:
          </div>
          <div className="flex gap-16 items-center flex-wrap justify-center">
            {[".NET 9", "Stripe", "Firebase", "Cloudflare R2"].map((t) => (
              <div
                key={t}
                className="text-2xl font-bold"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                {t}
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Services Bento Grid */}
      <section className="py-16 px-8">
        <div className="max-w-screen-2xl mx-auto">
          <div className="mb-10">
            <h2
              className="text-[2rem] font-semibold leading-[1.2] text-[#d3e4fe] mb-2"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Services Spécialisés
            </h2>
            <p className="text-[#c6c6cd]">
              L&apos;excellence technique au service de votre rentabilité.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Boutique */}
            <div className="glass-panel p-6 rounded-xl hover:border-[#adc6ff] transition-all group">
              <div className="mb-6 text-[#adc6ff] group-hover:scale-110 transition-transform duration-300 text-4xl">
                🛍
              </div>
              <h3
                className="text-2xl font-medium text-[#d3e4fe] mb-4"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Boutique
              </h3>
              <p className="text-sm text-[#c6c6cd] mb-6">
                Produits scalables et indicateurs Plug &amp; Play validés par
                des milliers d&apos;heures de trading réel.
              </p>
              <ul className="space-y-2 mb-10">
                {["LICENCE À VIE", "FICHIER .PINE + PDF"].map((f) => (
                  <li
                    key={f}
                    className="flex items-center gap-2 uppercase text-xs text-[#798098]"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    <span className="text-[#4ae176]">✓</span> {f}
                  </li>
                ))}
              </ul>
              <Link
                href="/catalogue"
                className="text-xs uppercase tracking-widest text-[#adc6ff] hover:underline"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Explorer →
              </Link>
            </div>

            {/* Sur-mesure */}
            <div className="glass-panel p-6 rounded-xl border-[#adc6ff]/50 bg-[#0566d9]/5 relative overflow-hidden">
              <div className="absolute top-0 right-0 p-2 bg-[#adc6ff] text-[#002e6a] uppercase tracking-widest text-[10px] font-semibold px-4"
                style={{ fontFamily: "var(--font-heading)" }}>
                Premium
              </div>
              <div className="mb-6 text-[#adc6ff] text-4xl">🏗</div>
              <h3
                className="text-2xl font-medium text-[#d3e4fe] mb-4"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Sur-mesure
              </h3>
              <p className="text-sm text-[#c6c6cd] mb-6">
                Développement de vos stratégies propriétaires complexes. Vous
                décrivez, je code.
              </p>
              <ul className="space-y-2 mb-10">
                {["CODE SOURCE OUVERT", "NDA INCLUS", "ACOMPTE 50%"].map((f) => (
                  <li
                    key={f}
                    className="flex items-center gap-2 uppercase text-xs text-[#798098]"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    <span className="text-[#4ae176]">✓</span> {f}
                  </li>
                ))}
              </ul>
              <Link
                href="/sur-mesure"
                className="text-xs uppercase tracking-widest text-[#adc6ff] hover:underline"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Démarrer un projet →
              </Link>
            </div>

            {/* Audit */}
            <div className="glass-panel p-6 rounded-xl hover:border-[#adc6ff] transition-all group">
              <div className="mb-6 text-[#adc6ff] group-hover:scale-110 transition-transform duration-300 text-4xl">
                📊
              </div>
              <h3
                className="text-2xl font-medium text-[#d3e4fe] mb-4"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Audit
              </h3>
              <p className="text-sm text-[#c6c6cd] mb-6">
                Conseil stratégique et correction de code pour optimiser vos
                systèmes existants.
              </p>
              <ul className="space-y-2 mb-10">
                {["RAPPORT DE PERFORMANCE", "DEBUGGING EXPERT"].map((f) => (
                  <li
                    key={f}
                    className="flex items-center gap-2 uppercase text-xs text-[#798098]"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    <span className="text-[#4ae176]">✓</span> {f}
                  </li>
                ))}
              </ul>
            </div>
          </div>
        </div>
      </section>

      {/* Featured Products */}
      <section className="py-16 bg-[#0b1c30]/30 px-8">
        <div className="max-w-screen-2xl mx-auto">
          <div className="flex justify-between items-end mb-10">
            <div>
              <h2
                className="text-[2rem] font-semibold leading-[1.2] text-[#d3e4fe] mb-2"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                Stratégies Vedettes
              </h2>
              <p className="text-[#c6c6cd]">
                Nos systèmes les plus performants du trimestre.
              </p>
            </div>
            <Link
              href="/catalogue"
              className="uppercase tracking-widest text-xs text-[#adc6ff] hover:underline"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              VOIR TOUT →
            </Link>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
            {[
              {
                name: "LiquidFlow Engine",
                tag: "PINE V5",
                tagColor: "text-[#4ae176] bg-[#4ae176]/10",
                desc: "Algorithme d'analyse de liquidité institutionnelle avec filtres de volatilité adaptatifs.",
                price: "$249",
              },
              {
                name: "NeuralTrend Pro",
                tag: "SCALABLE",
                tagColor: "text-[#adc6ff] bg-[#adc6ff]/10",
                desc: "Indicateur de suivi de tendance par régression linéaire optimisée pour le Nasdaq/S&P500.",
                price: "$189",
              },
              {
                name: "Volatility Guardian",
                tag: "SYSTEM",
                tagColor: "text-[#4ae176] bg-[#4ae176]/10",
                desc: "Gestionnaire de risque dynamique capable de calculer la taille de position en temps réel.",
                price: "$129",
              },
            ].map((p) => (
              <div
                key={p.name}
                className="bg-[#102034] rounded-xl overflow-hidden border border-[#45464d] hover:border-[#adc6ff] transition-colors"
              >
                {/* Placeholder chart thumbnail */}
                <div className="w-full h-48 bg-[#000f21] flex items-end px-4 pb-4 gap-1">
                  {[30, 50, 35, 65, 45, 80, 60, 90, 70, 100, 80, 95].map((h, i) => (
                    <div
                      key={i}
                      className="flex-1 rounded-sm opacity-70"
                      style={{
                        height: `${h}%`,
                        background: i % 3 === 0 ? "#ffb4ab" : "#4ae176",
                      }}
                    />
                  ))}
                </div>
                <div className="p-6">
                  <div className="flex justify-between items-start mb-4">
                    <h4
                      className="text-2xl font-medium text-[#d3e4fe]"
                      style={{ fontFamily: "var(--font-heading)", fontSize: "1.25rem" }}
                    >
                      {p.name}
                    </h4>
                    <span
                      className={`${p.tagColor} px-2 py-1 text-[10px] font-semibold uppercase tracking-widest`}
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {p.tag}
                    </span>
                  </div>
                  <p className="text-sm text-[#c6c6cd] mb-6">{p.desc}</p>
                  <div className="flex justify-between items-center border-t border-[#45464d] pt-4">
                    <span
                      className="text-2xl font-medium text-[#d3e4fe]"
                      style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
                    >
                      {p.price}
                    </span>
                    <Link
                      href="/catalogue"
                      className="bg-[#26364a] hover:bg-[#0566d9] text-[#d3e4fe] uppercase tracking-widest text-xs px-4 py-2 transition-all"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      Détails
                    </Link>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* Quality Protocol */}
      <section className="py-16 px-8" id="about">
        <div className="max-w-screen-2xl mx-auto flex flex-col lg:flex-row gap-16 items-center">
          <div className="flex-1 space-y-6">
            <h2
              className="text-[2rem] font-semibold leading-[1.2] text-[#d3e4fe]"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Protocole de Qualité
            </h2>
            <p className="text-[#c6c6cd]">
              Chaque ligne de code est soumise à un processus de validation
              rigoureux avant livraison.
            </p>
            <div className="space-y-4">
              {[
                {
                  n: "1",
                  title: "Backtest Multicycle",
                  desc: "Validation sur marchés haussiers, baissiers et rangés.",
                },
                {
                  n: "2",
                  title: "Nettoyage de Code",
                  desc: "Optimisation Pine Script v5 pour une latence minimale.",
                },
                {
                  n: "3",
                  title: "Validation Out-of-Sample",
                  desc: "Tests de robustesse pour éviter l'overfitting.",
                },
              ].map((s) => (
                <div key={s.n} className="flex items-center gap-6 p-4 glass-panel rounded-lg">
                  <div
                    className="w-10 h-10 flex-shrink-0 flex items-center justify-center bg-[#adc6ff]/20 text-[#adc6ff] font-semibold"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    {s.n}
                  </div>
                  <div>
                    <div
                      className="text-base font-semibold text-[#d3e4fe]"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {s.title}
                    </div>
                    <p className="text-xs text-[#c6c6cd]">{s.desc}</p>
                  </div>
                </div>
              ))}
            </div>
          </div>

          <div className="flex-1">
            <div className="relative group">
              <div className="absolute -inset-1 bg-gradient-to-r from-[#adc6ff] to-[#4ae176] rounded-xl blur opacity-25 group-hover:opacity-40 transition duration-1000" />
              <div className="relative bg-[#000f21] p-4 rounded-xl border border-[#45464d]">
                {/* Pine Script code aesthetic */}
                <div className="rounded-lg p-6 font-mono text-xs text-[#c6c6cd] space-y-1 overflow-hidden">
                  <div><span className="text-[#909097]">// TradeFlow Quality Protocol v2</span></div>
                  <div><span className="text-[#adc6ff]">strategy</span><span>(</span><span className="text-[#4ae176]">&quot;LiquidFlow Engine&quot;</span><span>, overlay=true)</span></div>
                  <div className="mt-2"><span className="text-[#adc6ff]">length</span> = input.int(<span className="text-[#4ae176]">14</span>, <span className="text-[#4ae176]">&quot;Period&quot;</span>)</div>
                  <div><span className="text-[#adc6ff]">src</span> = input.source(close, <span className="text-[#4ae176]">&quot;Source&quot;</span>)</div>
                  <div className="mt-2"><span className="text-[#909097]">// Profit Factor: 2.84 | Win Rate: 72.1%</span></div>
                  <div><span className="text-[#adc6ff]">signal</span> = ta.ema(src, length)</div>
                  <div><span className="text-[#adc6ff]">longCond</span> = ta.crossover(src, signal)</div>
                  <div className="mt-2"><span className="text-[#adc6ff]">if</span> longCond</div>
                  <div className="pl-4">strategy.entry(<span className="text-[#4ae176]">&quot;Long&quot;</span>, strategy.long)</div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="py-16 px-8 relative overflow-hidden">
        <div className="absolute inset-0 bg-[#0566d9]/5" />
        <div className="max-w-screen-2xl mx-auto glass-panel p-16 rounded-xl text-center relative z-10 neon-border">
          <h2
            className="text-5xl font-bold leading-[1.1] text-[#d3e4fe] mb-6"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Prêt à élever votre trading ?
          </h2>
          <p className="text-[#c6c6cd] max-w-2xl mx-auto mb-10">
            Rejoignez les traders qui ne laissent rien au hasard. Profitez
            d&apos;une expertise technique de pointe.
          </p>
          <div className="flex flex-col sm:flex-row justify-center gap-6">
            <Link
              href="/sur-mesure"
              className="bg-[#0566d9] text-[#e6ecff] px-10 py-5 uppercase tracking-widest text-sm font-semibold hover:shadow-[0_0_30px_rgba(5,102,217,0.6)] transition-all"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Démarrez votre projet sur-mesure
            </Link>
            <Link
              href="/catalogue"
              className="border border-[#adc6ff] text-[#adc6ff] px-10 py-5 uppercase tracking-widest text-sm font-semibold hover:bg-[#adc6ff]/10 transition-all"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Explorer la marketplace
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
}
