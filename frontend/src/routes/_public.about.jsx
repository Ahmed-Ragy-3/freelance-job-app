import { createFileRoute } from "@tanstack/react-router";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { Users, Target, Award, Globe } from "lucide-react";

export const Route = createFileRoute("/_public/about")({
  head: () => ({ meta: [{ title: "About — Workly" }, { name: "description", content: "Learn about Workly's mission to connect world-class freelancers with ambitious teams." }] }),
  component: About,
});

function About() {
  return (
    <div className="mx-auto max-w-5xl px-4 py-16 sm:px-6">
      <Breadcrumbs />
      <h1 className="mt-4 text-4xl font-bold tracking-tight">About Workly</h1>
      <p className="mt-4 max-w-2xl text-lg text-muted-foreground">We're on a mission to make hiring — and being hired — feel effortless. Workly connects vetted freelancers with ambitious teams building the future.</p>
      <div className="mt-12 grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
        {[
          { icon: Users, label: "Talent", value: "42k+ vetted freelancers" },
          { icon: Target, label: "Focus", value: "Quality over quantity" },
          { icon: Award, label: "Rated", value: "4.9 average rating" },
          { icon: Globe, label: "Global", value: "60+ countries" },
        ].map((s) => (
          <div key={s.label} className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <s.icon className="text-primary" size={20} />
            <div className="mt-3 text-sm text-muted-foreground">{s.label}</div>
            <div className="mt-1 font-semibold">{s.value}</div>
          </div>
        ))}
      </div>
      <div className="prose prose-neutral dark:prose-invert mt-12 max-w-none">
        <h2>Our story</h2>
        <p>Founded in 2023, Workly was built after our founders spent years frustrated hiring freelancers through outdated platforms. We believe great work happens when trust, quality, and speed converge — that's the marketplace we're building.</p>
        <h2>Our values</h2>
        <ul>
          <li><strong>Vetted first.</strong> Every freelancer is manually reviewed.</li>
          <li><strong>Fair fees.</strong> Transparent, low pricing for both sides.</li>
          <li><strong>Speed to hire.</strong> Post today, hire tomorrow.</li>
        </ul>
      </div>
    </div>
  );
}
