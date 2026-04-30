import Link from "next/link";

export default function AuthLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="trading-bg min-h-screen flex flex-col">
      <div className="px-8 py-4">
        <Link
          href="/"
          className="text-xl font-bold tracking-tighter text-slate-50"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          TradeFlow
        </Link>
      </div>
      <div className="flex-1 flex items-center justify-center px-8 py-16">
        {children}
      </div>
    </div>
  );
}
