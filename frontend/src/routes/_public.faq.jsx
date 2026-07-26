import { createFileRoute } from "@tanstack/react-router";
import { useState } from "react";
import { ChevronDown } from "lucide-react";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";

const faqs = [
  { q: "How does Workly verify freelancers?", a: "Every freelancer goes through a portfolio and skills review before being listed." },
  { q: "What are the fees?", a: "Workly takes a flat 8% service fee on completed projects. No hidden costs." },
  { q: "How do payments work?", a: "Payments are held in escrow and released when milestones are completed and approved." },
  { q: "Can I hire for full-time roles?", a: "Yes — you can hire on a project, hourly, or long-term retainer basis." },
  { q: "Is there a mobile app?", a: "A mobile app is on our roadmap. The web app is fully responsive today." },
  { q: "How do I dispute a project?", a: "Our support team mediates disputes and can pause escrow funds while resolving." },
];

export const Route = createFileRoute("/_public/faq")({
  head: () => ({ meta: [{ title: "FAQ — Workly" }, { name: "description", content: "Answers to common Workly questions." }] }),
  component: FAQ,
});

function FAQ() {
  const [open, setOpen] = useState(0);
  return (
    <div className="mx-auto max-w-3xl px-4 py-16 sm:px-6">
      <Breadcrumbs />
      <h1 className="mt-4 text-4xl font-bold tracking-tight">Frequently asked questions</h1>
      <div className="mt-10 space-y-3">
        {faqs.map((f, i) => (
          <div key={i} className="overflow-hidden rounded-xl border border-border bg-card">
            <button onClick={() => setOpen(open === i ? -1 : i)} className="flex w-full items-center justify-between gap-3 p-5 text-left">
              <span className="font-semibold">{f.q}</span>
              <ChevronDown size={18} className={"transition-transform " + (open === i ? "rotate-180" : "")} />
            </button>
            {open === i && <div className="border-t border-border p-5 pt-4 text-sm text-muted-foreground">{f.a}</div>}
          </div>
        ))}
      </div>
    </div>
  );
}
