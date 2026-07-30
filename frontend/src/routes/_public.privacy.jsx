import { createFileRoute } from "@tanstack/react-router";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";

export const Route = createFileRoute("/_public/privacy")({
  head: () => ({ meta: [{ title: "Privacy Policy — Workly" }, { name: "description", content: "How Workly handles your data and privacy." }] }),
  component: () => (
    <div className="mx-auto max-w-3xl px-4 py-16 sm:px-6">
      <Breadcrumbs />
      <h1 className="mt-4 text-4xl font-bold tracking-tight">Privacy Policy</h1>
      <p className="mt-3 text-sm text-muted-foreground">Last updated: January 2026</p>
      <div className="prose prose-neutral dark:prose-invert mt-8 max-w-none">
        <p>Your privacy is important to us. This policy explains what data we collect, how we use it, and your rights.</p>
        <h2>What we collect</h2><p>Account information, usage data, and payment metadata. We never sell your data.</p>
        <h2>How we use it</h2><p>To operate the marketplace, match talent, prevent fraud, and improve our services.</p>
        <h2>Your rights</h2><p>Access, correct, export, or delete your data at any time via account settings.</p>
      </div>
    </div>
  ),
});
