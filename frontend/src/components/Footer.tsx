import Link from "next/link";

export default function Footer() {
  return (
    <footer className="bg-slate-950 border-t border-slate-900 mt-auto">
      <div className="flex flex-col md:flex-row justify-between items-start gap-8 px-8 py-12 max-w-screen-2xl mx-auto">
        <div className="max-w-xs">
          <div
            className="text-lg font-black text-slate-200 mb-4"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            TradeFlow
          </div>
          <p
            className="font-light text-slate-500 text-sm leading-relaxed"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            © 2025 TradeFlow. High-performance Pine Script systems. Financial
            Disclaimer: Trading involves significant risk of loss and is not
            suitable for all investors.
          </p>
        </div>

        <div className="grid grid-cols-2 md:grid-cols-3 gap-16">
          <div className="space-y-4">
            <h5
              className="text-slate-200 uppercase tracking-widest text-xs"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Solutions
            </h5>
            <ul className="space-y-2">
              {[
                { href: "/catalogue", label: "Marketplace" },
                { href: "/sur-mesure", label: "Sur-mesure" },
              ].map((l) => (
                <li key={l.href}>
                  <Link
                    href={l.href}
                    className="text-slate-500 hover:text-blue-400 transition-all text-sm"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    {l.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          <div className="space-y-4">
            <h5
              className="text-slate-200 uppercase tracking-widest text-xs"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Compagnie
            </h5>
            <ul className="space-y-2">
              {[
                { href: "/#about", label: "About" },
              ].map((l) => (
                <li key={l.href}>
                  <Link
                    href={l.href}
                    className="text-slate-500 hover:text-blue-400 transition-all text-sm"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    {l.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          <div className="space-y-4">
            <h5
              className="text-slate-200 uppercase tracking-widest text-xs"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              Légal
            </h5>
            <ul className="space-y-2">
              {[
                { href: "/terms", label: "Terms" },
                { href: "/privacy", label: "Privacy" },
              ].map((l) => (
                <li key={l.href}>
                  <Link
                    href={l.href}
                    className="text-slate-500 hover:text-blue-400 transition-all text-sm"
                    style={{ fontFamily: "var(--font-heading)" }}
                  >
                    {l.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>
        </div>
      </div>
    </footer>
  );
}
