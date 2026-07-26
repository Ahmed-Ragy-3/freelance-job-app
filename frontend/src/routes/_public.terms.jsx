import { createFileRoute } from "@tanstack/react-router";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";

export const Route = createFileRoute("/_public/terms")({
  head: () => ({ meta: [{ title: "Terms of Service — Workly" }, { name: "description", content: "The terms governing your use of Workly." }] }),
  component: () => (
    <div className="mx-auto max-w-3xl px-4 py-16 sm:px-6">
      <Breadcrumbs />
      <h1 className="mt-4 text-4xl font-bold tracking-tight">Terms of Service</h1>
      <p className="mt-3 text-sm text-muted-foreground">Last updated: January 2026</p>
      <div className="prose prose-neutral dark:prose-invert mt-8 max-w-none">
        <p>These terms govern your use of Workly. By creating an account, you agree to them.</p>
        <h2>Acceptable use</h2><p>Don't spam, misrepresent yourself, or use the platform for illegal activity.</p>
        <h2>Fees & payments</h2><p>Workly charges 8% service fee on completed contracts. Payments are held in escrow.</p>
        <h2>Termination</h2><p>We may suspend or terminate accounts that violate these terms.</p>
      </div>
    </div>
  ),
});
