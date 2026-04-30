"use client";

import Link from "next/link";
import { useState } from "react";

const CATEGORIES = ["Tout", "Indicators", "Strategies", "Bundles"] as const;
type Category = (typeof CATEGORIES)[number];

const PRODUCTS = [
  {
    id: "of-pro-suite",
    name: "OF Pro Suite",
    type: "Strategy",
    typeColor: "text-[#4ae176]",
    badge: "Top Rated",
    badgeColor: "bg-[#0566d9]/90 text-white",
    stats: [
      { label: "Win Rate", value: "68.4%", color: "text-[#4ae176]" },
      { label: "Profit Factor", value: "2.14", color: "text-[#4ae176]" },
      { label: "Max DD", value: "4.2%", color: "text-[#ffb4ab]" },
    ],
    price: "49€",
    category: "Strategies",
  },
  {
    id: "volume-profile-master",
    name: "Volume Profile Master",
    type: "Indicator",
    typeColor: "text-[#d3e4fe]",
    badge: null,
    stats: [
      { label: "Accuracy", value: "N/A", color: "text-[#d3e4fe]" },
      { label: "Levels", value: "Dynamic", color: "text-[#4ae176]" },
      { label: "Lag", value: "0ms", color: "text-[#4ae176]" },
    ],
    price: "29€",
    category: "Indicators",
  },
  {
    id: "neural-flux-engine",
    name: "Neural Flux Engine",
    type: "Strategy",
    typeColor: "text-[#4ae176]",
    badge: null,
    stats: [
      { label: "Win Rate", value: "72.1%", color: "text-[#4ae176]" },
      { label: "PF", value: "2.84", color: "text-[#4ae176]" },
      { label: "Max DD", value: "6.1%", color: "text-[#ffb4ab]" },
    ],
    price: "89€",
    category: "Strategies",
  },
  {
    id: "scalpers-toolkit",
    name: "Scalper's Toolkit",
    type: "Bundle",
    typeColor: "text-[#adc6ff]",
    badge: null,
    stats: [
      { label: "Assets", value: "4 Units", color: "text-[#d3e4fe]" },
      { label: "Type", value: "Hybrid", color: "text-[#4ae176]" },
      { label: "Savings", value: "35%", color: "text-[#4ae176]" },
    ],
    price: "129€",
    category: "Bundles",
  },
  {
    id: "liquidity-mapper",
    name: "Liquidity Mapper",
    type: "Indicator",
    typeColor: "text-[#d3e4fe]",
    badge: null,
    stats: [
      { label: "Refresh", value: "Realtime", color: "text-[#4ae176]" },
      { label: "Markets", value: "All", color: "text-[#d3e4fe]" },
      { label: "Difficulty", value: "Mid", color: "text-[#d3e4fe]" },
    ],
    price: "39€",
    category: "Indicators",
  },
  {
    id: "arb-hunter-x",
    name: "Arb Hunter X",
    type: "Strategy",
    typeColor: "text-[#4ae176]",
    badge: "Hot",
    badgeColor: "bg-[#93000a]/80 text-white",
    stats: [
      { label: "Win Rate", value: "59.2%", color: "text-[#4ae176]" },
      { label: "PF", value: "1.92", color: "text-[#4ae176]" },
      { label: "Max DD", value: "3.8%", color: "text-[#ffb4ab]" },
    ],
    price: "59€",
    category: "Strategies",
  },
  {
    id: "vol-grid-2",
    name: "Vol Grid 2.0",
    type: "Indicator",
    typeColor: "text-[#d3e4fe]",
    badge: null,
    stats: [
      { label: "Precision", value: "High", color: "text-[#d3e4fe]" },
      { label: "Nodes", value: "256", color: "text-[#4ae176]" },
      { label: "CPU", value: "Low", color: "text-[#4ae176]" },
    ],
    price: "45€",
    category: "Indicators",
  },
  {
    id: "momentum-surge",
    name: "Momentum Surge",
    type: "Strategy",
    typeColor: "text-[#4ae176]",
    badge: null,
    stats: [
      { label: "Win Rate", value: "61.8%", color: "text-[#4ae176]" },
      { label: "PF", value: "2.21", color: "text-[#4ae176]" },
      { label: "Max DD", value: "5.3%", color: "text-[#ffb4ab]" },
    ],
    price: "79€",
    category: "Strategies",
  },
];

export default function CataloguePage() {
  const [activeCategory, setActiveCategory] = useState<Category>("Tout");
  const [search, setSearch] = useState("");

  const filtered = PRODUCTS.filter((p) => {
    const matchCat = activeCategory === "Tout" || p.category === activeCategory;
    const matchSearch =
      !search ||
      p.name.toLowerCase().includes(search.toLowerCase()) ||
      p.type.toLowerCase().includes(search.toLowerCase());
    return matchCat && matchSearch;
  });

  return (
    <div className="trading-bg pt-24 pb-16 px-8 max-w-screen-2xl mx-auto min-h-screen">
      {/* Header */}
      <header className="mb-12">
        <div className="flex flex-col md:flex-row md:items-end justify-between gap-8 mb-8">
          <div>
            <h1
              className="text-5xl font-bold leading-[1.1] text-[#d3e4fe] mb-2 tracking-tight"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Marketplace de Stratégies{" "}
              <span className="text-[#adc6ff]">TradingView</span>
            </h1>
            <p className="text-[#c6c6cd] max-w-2xl">
              Accédez à des systèmes Pine Script de haute performance. Optimisés
              pour la précision et la gestion du risque institutionnelle.
            </p>
          </div>

          <div className="flex flex-col gap-4 min-w-[320px]">
            <div className="relative">
              <span className="absolute left-3 top-1/2 -translate-y-1/2 text-[#909097]">
                🔍
              </span>
              <input
                type="text"
                placeholder="Rechercher un indicateur..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="w-full bg-[#000f21] border border-[#45464d] py-3 pl-10 pr-4 text-sm focus:border-[#0566d9] focus:ring-1 focus:ring-[#0566d9] outline-none text-[#d3e4fe] placeholder:text-[#909097]"
              />
            </div>
          </div>
        </div>

        {/* Filters */}
        <div className="flex flex-wrap items-center gap-4 border-b border-[#45464d]/30 pb-6">
          {CATEGORIES.map((cat) => (
            <button
              key={cat}
              onClick={() => setActiveCategory(cat)}
              className={`px-6 py-2 rounded-full uppercase tracking-widest text-xs font-semibold transition-all ${
                activeCategory === cat
                  ? "bg-[#0566d9] text-white"
                  : "bg-[#0b1c30] border border-[#45464d]/30 text-[#c6c6cd] hover:border-[#0566d9]"
              }`}
              style={{ fontFamily: "var(--font-heading)" }}
            >
              {cat}
            </button>
          ))}
          <div
            className="ml-auto flex items-center gap-2 text-[#c6c6cd] uppercase tracking-tighter text-[10px]"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            ≡ Trier par: Récents
          </div>
        </div>
      </header>

      {/* Product Grid */}
      <section className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
        {filtered.map((product) => (
          <ProductCard key={product.id} product={product} />
        ))}
        {filtered.length === 0 && (
          <div className="col-span-full text-center py-20 text-[#909097]">
            Aucun produit trouvé.
          </div>
        )}
      </section>
    </div>
  );
}

function ProductCard({ product }: { product: (typeof PRODUCTS)[number] }) {
  return (
    <div className="glass-card flex flex-col group overflow-hidden hover:border-[#0566d9]/50 transition-all duration-300">
      {/* Thumbnail */}
      <div className="aspect-[4/3] relative overflow-hidden bg-[#000f21] flex items-end px-3 pb-3 gap-0.5">
        {Array.from({ length: 16 }).map((_, i) => {
          const h = 20 + Math.abs(Math.sin(i * 1.3 + product.name.length) * 80);
          const up = i % 2 === 0;
          return (
            <div
              key={i}
              className="flex-1 rounded-sm opacity-75 group-hover:opacity-100 transition-opacity duration-500"
              style={{
                height: `${h}%`,
                background: up ? "#4ae176" : "#ffb4ab",
              }}
            />
          );
        })}
        <div
          className="absolute top-3 left-3 bg-slate-950/80 backdrop-blur-md border border-[#45464d]/40 px-3 py-1 rounded-full text-[10px] uppercase tracking-wider font-semibold"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          <span className={product.typeColor}>{product.type}</span>
        </div>
        {product.badge && (
          <div
            className={`absolute top-3 right-3 ${product.badgeColor} backdrop-blur-md px-3 py-1 rounded-full text-[10px] uppercase tracking-wider font-semibold`}
            style={{ fontFamily: "var(--font-heading)" }}
          >
            {product.badge}
          </div>
        )}
      </div>

      {/* Content */}
      <div className="p-5 flex flex-col flex-grow">
        <h3
          className="text-lg mb-4 text-[#d3e4fe] group-hover:text-[#adc6ff] transition-colors font-semibold"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          {product.name}
        </h3>

        <div className="grid grid-cols-3 gap-2 mb-6 border-y border-[#45464d]/20 py-4">
          {product.stats.map((s) => (
            <div key={s.label} className="flex flex-col">
              <span
                className="text-[10px] uppercase text-[#798098]"
                style={{ fontFamily: "var(--font-heading)" }}
              >
                {s.label}
              </span>
              <span
                className={`${s.color} text-lg font-medium`}
                style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
              >
                {s.value}
              </span>
            </div>
          ))}
        </div>

        <div className="mt-auto flex items-center justify-between">
          <span
            className="text-2xl text-[#d3e4fe] font-medium"
            style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
          >
            {product.price}
          </span>
          <Link
            href={`/catalogue/${product.id}`}
            className="bg-transparent border border-[#0566d9] text-[#adc6ff] hover:bg-[#0566d9] hover:text-white px-5 py-2 uppercase tracking-widest text-[11px] font-semibold transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Détails &amp; Achat
          </Link>
        </div>
      </div>
    </div>
  );
}
