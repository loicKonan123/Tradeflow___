"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { getProjects, cancelProject, type Project, type ProjectStatus } from "@/lib/api";

const STATUS_LABELS: Record<ProjectStatus, string> = {
  Submitted: "Soumis",
  Quoted: "Devis reçu",
  DepositPaid: "Acompte payé",
  InProgress: "En cours",
  Delivered: "Livré",
  Cancelled: "Annulé",
};

const STATUS_CLASSES: Record<ProjectStatus, string> = {
  Submitted: "text-secondary bg-secondary-container/20",
  Quoted: "text-tertiary bg-tertiary-container/40",
  DepositPaid: "text-tertiary bg-tertiary-container/40",
  InProgress: "text-secondary bg-secondary-container/20",
  Delivered: "text-tertiary bg-tertiary-container/40",
  Cancelled: "text-error bg-error-container/20",
};

const STEPS: ProjectStatus[] = [
  "Submitted",
  "Quoted",
  "DepositPaid",
  "InProgress",
  "Delivered",
];

export default function ProjectsPage() {
  const [projects, setProjects] = useState<Project[]>([]);
  const [loading, setLoading] = useState(true);
  const [cancelling, setCancelling] = useState<string | null>(null);
  const [error, setError] = useState("");

  useEffect(() => {
    getProjects()
      .then(setProjects)
      .catch(() => setError("Impossible de charger les projets."))
      .finally(() => setLoading(false));
  }, []);

  async function handleCancel(id: string) {
    if (!confirm("Annuler ce projet ?")) return;
    setCancelling(id);
    try {
      await cancelProject(id);
      setProjects((prev) =>
        prev.map((p) => (p.id === id ? { ...p, status: "Cancelled" as ProjectStatus } : p))
      );
    } catch (e: unknown) {
      setError(e instanceof Error ? e.message : "Erreur lors de l'annulation.");
    } finally {
      setCancelling(null);
    }
  }

  if (loading) {
    return (
      <div className="space-y-4">
        {[1, 2].map((i) => (
          <div key={i} className="h-40 bg-surface-container animate-pulse rounded-xl" />
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <p className="text-sm text-on-surface-variant">
          {projects.length} projet{projects.length !== 1 ? "s" : ""}
        </p>
        <Link
          href="/dashboard/projects/new"
          className="bg-secondary-container text-on-secondary-container px-5 py-2 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_15px_rgba(5,102,217,0.3)] transition-all"
          style={{ fontFamily: "var(--font-heading)" }}
        >
          + Nouveau projet
        </Link>
      </div>

      {error && (
        <div className="glass-panel p-4 rounded-lg border border-error/30">
          <p className="text-sm text-error">{error}</p>
        </div>
      )}

      {projects.length === 0 ? (
        <div className="glass-panel p-12 rounded-xl text-center">
          <p className="text-on-surface-variant mb-4">
            Aucun projet sur-mesure pour l&apos;instant.
          </p>
          <Link
            href="/dashboard/projects/new"
            className="bg-secondary-container text-on-secondary-container px-6 py-2 uppercase tracking-widest text-xs font-semibold hover:shadow-[0_0_15px_rgba(5,102,217,0.3)] transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Soumettre un brief
          </Link>
        </div>
      ) : (
        <div className="space-y-6">
          {projects.map((project) => (
            <ProjectCard
              key={project.id}
              project={project}
              cancelling={cancelling === project.id}
              onCancel={handleCancel}
            />
          ))}
        </div>
      )}
    </div>
  );
}

function ProjectCard({
  project,
  cancelling,
  onCancel,
}: {
  project: Project;
  cancelling: boolean;
  onCancel: (id: string) => void;
}) {
  const stepIndex = STEPS.indexOf(project.status as ProjectStatus);
  const isCancellable =
    project.status === "Submitted" || project.status === "Quoted";

  return (
    <div className="glass-panel p-6 rounded-xl space-y-5">
      <div className="flex flex-col sm:flex-row sm:items-start justify-between gap-3">
        <div>
          <div className="flex items-center gap-3 mb-1">
            <h3
              className="text-base font-semibold text-on-surface"
              style={{ fontFamily: "var(--font-heading)" }}
            >
              #{project.strategyTitle}
            </h3>
            <span
              className={`text-[10px] uppercase tracking-widest px-2 py-0.5 ${STATUS_CLASSES[project.status as ProjectStatus] ?? "text-outline"}`}
              style={{ fontFamily: "var(--font-heading)" }}
            >
              {STATUS_LABELS[project.status as ProjectStatus] ?? project.status}
            </span>
          </div>
          <p className="text-xs text-outline">
            {project.market} · {project.timeframe} ·{" "}
            {new Date(project.createdAt).toLocaleDateString("fr-FR")}
          </p>
        </div>

        {project.quotedPrice && (
          <div className="flex-shrink-0 text-right">
            <p className="text-xs text-outline mb-0.5" style={{ fontFamily: "var(--font-heading)" }}>
              Devis
            </p>
            <p
              className="text-xl font-medium text-on-surface"
              style={{ fontFamily: "var(--font-heading)", letterSpacing: "-0.02em" }}
            >
              {project.quotedPrice} {project.quotedCurrency}
            </p>
            {project.depositAmount && (
              <p className="text-xs text-on-surface-variant">
                Acompte : {project.depositAmount} {project.quotedCurrency}
              </p>
            )}
          </div>
        )}
      </div>

      {/* Progress stepper */}
      {project.status !== "Cancelled" && (
        <div className="flex items-center gap-1">
          {STEPS.map((step, i) => {
            const done = i <= stepIndex;
            const current = i === stepIndex;
            return (
              <div key={step} className="flex items-center flex-1">
                <div
                  className={`h-1.5 flex-1 transition-all ${done ? "bg-tertiary" : "bg-surface-container-high"} ${current ? "shadow-[0_0_6px_#4ae176]" : ""}`}
                />
                {i < STEPS.length - 1 && (
                  <div
                    className={`w-2 h-2 rounded-full flex-shrink-0 ${done ? "bg-tertiary" : "bg-surface-container-high"}`}
                  />
                )}
              </div>
            );
          })}
        </div>
      )}

      {/* Description preview */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4 text-sm">
        <div>
          <p
            className="text-xs uppercase tracking-widest text-outline mb-1"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Entrée
          </p>
          <p className="text-on-surface-variant line-clamp-2">{project.entryConditions}</p>
        </div>
        <div>
          <p
            className="text-xs uppercase tracking-widest text-outline mb-1"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Sortie
          </p>
          <p className="text-on-surface-variant line-clamp-2">{project.exitConditions}</p>
        </div>
      </div>

      {project.adminNotes && (
        <div className="glass-panel p-3 rounded-lg border border-secondary-container/30">
          <p
            className="text-xs uppercase tracking-widest text-secondary mb-1"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            Note de Loïc
          </p>
          <p className="text-sm text-on-surface-variant">{project.adminNotes}</p>
        </div>
      )}

      {isCancellable && (
        <div className="flex justify-end pt-2 border-t border-outline-variant/20">
          <button
            onClick={() => onCancel(project.id)}
            disabled={cancelling}
            className="text-xs uppercase tracking-widest text-error hover:underline disabled:opacity-50 transition-all"
            style={{ fontFamily: "var(--font-heading)" }}
          >
            {cancelling ? "Annulation..." : "Annuler le projet"}
          </button>
        </div>
      )}
    </div>
  );
}
