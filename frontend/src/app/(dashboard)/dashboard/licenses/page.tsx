"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { getLicenses, downloadLicense, type License } from "@/lib/api";

export default function LicensesPage() {
  const [licenses, setLicenses] = useState<License[]>([]);
  const [loading, setLoading] = useState(true);
  const [downloading, setDownloading] = useState<string | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    getLicenses()
      .then(setLicenses)
      .catch(() => setError("Impossible de charger les licences."))
      .finally(() => setLoading(false));
  }, []);

  async function handleDownload(license: License) {
    if (license.isRevoked || license.downloadsUsed >= license.maxDownloads) return;
    setDownloading(license.id);
    setError("");
    try {
      const { url } = await downloadLicense(license.id);
      window.open(url, "_blank");
      setLicenses((prev) =>
        prev.map((l) =>
          l.id === license.id ? { ...l, downloadsUsed: l.downloadsUsed + 1 } : l
        )
      );
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Erreur lors du téléchargement.");
    } finally {
      setDownloading(null);
    }
  }

  if (loading) {
    return (
      <div className="space-y-4">
        {[1, 2, 3].map((i) => (
          <div key={i} className="h-24 bg-surface-container animate-pulse rounded-xl" />
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <p className="text-sm text-on-surface-variant">
          {licenses.length} licence{licenses.length !== 1 ? "s" : ""}
        </p>
        <Link
          href="/catalogue"
          className="text-xs uppercase tracking-widest text-secondary hover:underline"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          + Acheter un produit
        </Link>
      </div>

      {error && (
        <div className="glass-panel p-4 rounded-lg border border-error/30">
          <p className="text-sm text-error">{error}</p>
        </div>
      )}

      {licenses.length === 0 ? (
        <div className="glass-panel p-12 rounded-xl text-center">
          <p className="text-on-surface-variant mb-4">Aucune licence pour l&apos;instant.</p>
          <Link
            href="/catalogue"
            className="bg-secondary-container text-on-secondary-container px-6 py-2 uppercase tracking-widest text-xs font-semibold transition-all hover:shadow-[0_0_15px_rgba(5,102,217,0.3)]"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Explorer la marketplace
          </Link>
        </div>
      ) : (
        <div className="space-y-4">
          {licenses.map((license) => {
            const exhausted = license.downloadsUsed >= license.maxDownloads;
            const canDownload = !license.isRevoked && !exhausted;
            const isDownloading = downloading === license.id;

            return (
              <div
                key={license.id}
                className="glass-panel p-6 rounded-xl flex flex-col md:flex-row md:items-center gap-4"
              >
                <div className="flex-1 min-w-0">
                  <div className="flex items-center gap-3 mb-1">
                    <h3
                      className="text-base font-semibold text-on-surface"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {license.productName}
                    </h3>
                    {license.isRevoked ? (
                      <span
                        className="text-[10px] uppercase tracking-widest text-error bg-error-container/20 px-2 py-0.5"
                        style={{ fontFamily: "var(--font-heading)" }}
                      >
                        Révoquée
                      </span>
                    ) : exhausted ? (
                      <span
                        className="text-[10px] uppercase tracking-widest text-outline bg-surface-container px-2 py-0.5"
                        style={{ fontFamily: "var(--font-heading)" }}
                      >
                        Épuisée
                      </span>
                    ) : (
                      <span
                        className="text-[10px] uppercase tracking-widest text-tertiary bg-tertiary-container/40 px-2 py-0.5"
                        style={{ fontFamily: "var(--font-heading)" }}
                      >
                        Active
                      </span>
                    )}
                  </div>

                  <p className="text-xs text-outline font-mono">{license.downloadToken}</p>

                  {/* Download progress bar */}
                  <div className="mt-3 flex items-center gap-3">
                    <div className="flex-1 bg-surface-container-high h-1">
                      <div
                        className="bg-tertiary h-full transition-all duration-300"
                        style={{
                          width: `${(license.downloadsUsed / license.maxDownloads) * 100}%`,
                          boxShadow: canDownload ? "0 0 6px #4ae176" : undefined,
                        }}
                      />
                    </div>
                    <span
                      className="text-xs text-on-surface-variant flex-shrink-0"
                      style={{ fontFamily: "var(--font-heading)" }}
                    >
                      {license.downloadsUsed}/{license.maxDownloads}
                    </span>
                  </div>
                </div>

                <button
                  onClick={() => handleDownload(license)}
                  disabled={!canDownload || isDownloading}
                  className={`flex-shrink-0 px-6 py-2.5 uppercase tracking-widest text-xs font-semibold transition-all ${
                    canDownload && !isDownloading
                      ? "bg-secondary-container text-on-secondary-container hover:shadow-[0_0_15px_rgba(5,102,217,0.3)]"
                      : "bg-surface-container text-outline cursor-not-allowed"
                  }`}
                  style={{ fontFamily: "var(--font-heading)" }}
                >
                  {isDownloading ? "..." : "↓ Télécharger"}
                </button>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
