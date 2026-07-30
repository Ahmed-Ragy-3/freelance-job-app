import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { motion } from "framer-motion";
import { ArrowRight, Search, TrendingUp, Users, Briefcase, Award, Sparkles } from "lucide-react";
import { staticData } from "@/services";
import { JobCard } from "@/components/jobs/JobCard";
import { FreelancerCard } from "@/components/profile/FreelancerCard";
import { ClientCard } from "@/components/profile/ClientCard";

import { formatNumber, formatMoney } from "@/utils/format";
import { Avatar } from "@/components/common/Avatar";
import { Rating } from "@/components/common/Rating";
import { Navbar } from "@/components/layout/Navbar";
import { Footer } from "@/components/layout/Footer";

export const Route = createFileRoute("/")({
  head: () => ({
    meta: [
      { title: "Workly — The premier freelance marketplace" },
      { name: "description", content: "Hire vetted freelancers or find high-quality work. Design, development, marketing, and more." },
      { property: "og:title", content: "Workly — Hire top freelance talent" },
      { property: "og:description", content: "The modern freelance marketplace for vetted talent and ambitious clients." },
    ],
  }),
  component: Landing,
});

function SectionHeader({ eyebrow, title, subtitle }) {
  return (
    <div>
      <div className="text-xs font-semibold uppercase tracking-wider text-primary">{eyebrow}</div>
      <h2 className="mt-2 text-3xl font-bold tracking-tight text-foreground">{title}</h2>
      {subtitle && <p className="mt-2 max-w-xl text-muted-foreground">{subtitle}</p>}
    </div>
  );
}

function Landing() {
  const [cats, setCats] = useState([]);
  const [featJobs, setFeatJobs] = useState([]);
  const [flancers, setFlancers] = useState([]);
  const [clients, setClients] = useState([]);
  const [tests, setTests] = useState([]);
  const [stats, setStats] = useState(null);
  const [q, setQ] = useState("");

  useEffect(() => {
    staticData.categories().then(setCats);
    staticData.featuredJobs().then(setFeatJobs);
    staticData.topFreelancers().then(setFlancers);
    staticData.topClients().then(setClients);
    staticData.testimonials().then(setTests);
    staticData.stats().then(setStats);
  }, []);


  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <main className="flex-1">
        <section className="relative overflow-hidden">
          <div className="absolute inset-0 -z-10" style={{ background: "var(--gradient-hero)" }} />
          <div className="mx-auto max-w-7xl px-4 py-20 sm:px-6 md:py-28">
            <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} className="mx-auto max-w-3xl text-center">
              <span className="inline-flex items-center gap-1.5 rounded-full border border-border bg-card/60 px-3 py-1 text-xs font-medium text-muted-foreground">
                <Sparkles size={12} className="text-primary" /> Trusted by 12,000+ teams
              </span>
              <h1 className="mt-5 text-4xl font-extrabold tracking-tight text-foreground sm:text-6xl">
                Hire <span className="gradient-text">world-class</span> freelance talent
              </h1>
              <p className="mx-auto mt-5 max-w-xl text-lg text-muted-foreground">
                From product designers to full-stack engineers — find vetted specialists ready to ship.
              </p>
              <form onSubmit={(e) => { e.preventDefault(); window.location.href = `/jobs?q=${encodeURIComponent(q)}`; }} className="mx-auto mt-8 flex max-w-xl overflow-hidden rounded-full border border-border bg-card shadow-elevated">
                <div className="flex flex-1 items-center gap-2 px-5">
                  <Search size={18} className="text-muted-foreground" />
                  <input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Try 'React developer' or 'brand designer'" className="w-full bg-transparent py-3 outline-none" />
                </div>
                <button className="m-1.5 rounded-full bg-primary px-5 py-3 text-sm font-semibold text-primary-foreground hover:opacity-90">Search</button>
              </form>
              <div className="mt-4 flex flex-wrap justify-center gap-2 text-xs text-muted-foreground">
                {["React","Figma","Copywriter","SEO","iOS","Data"].map((t) => (
                  <Link key={t} to="/jobs" className="rounded-full border border-border bg-card px-3 py-1 hover:border-primary hover:text-foreground">{t}</Link>
                ))}
              </div>
            </motion.div>

            {stats && (
              <div className="mx-auto mt-16 grid max-w-4xl grid-cols-2 gap-6 md:grid-cols-4">
                {[
                  { label: "Freelancers", value: formatNumber(stats.freelancers), icon: Users },
                  { label: "Clients", value: formatNumber(stats.clients), icon: Briefcase },
                  { label: "Jobs posted", value: formatNumber(stats.jobsPosted), icon: TrendingUp },
                  { label: "Paid out", value: formatMoney(stats.paidOut), icon: Award },
                ].map((s) => (
                  <div key={s.label} className="rounded-2xl border border-border bg-card/60 p-4 text-center">
                    <s.icon size={18} className="mx-auto mb-2 text-primary" />
                    <div className="text-2xl font-bold text-foreground">{s.value}</div>
                    <div className="text-xs text-muted-foreground">{s.label}</div>
                  </div>
                ))}
              </div>
            )}
          </div>
        </section>

        <section className="mx-auto max-w-7xl px-4 py-16 sm:px-6">
          <SectionHeader eyebrow="Categories" title="Browse popular categories" subtitle="Find the right talent for every kind of project." />
          <div className="mt-8 grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
            {cats.map((c, i) => (
              <motion.div key={c.id} initial={{ opacity: 0, y: 12 }} whileInView={{ opacity: 1, y: 0 }} viewport={{ once: true }} transition={{ delay: i * 0.03 }}>
                <Link to="/jobs" className="group flex h-full flex-col rounded-2xl border border-border bg-card p-6 shadow-soft transition-all hover:-translate-y-1 hover:shadow-elevated">
                  <div className="flex h-10 w-10 items-center justify-center rounded-xl bg-primary/10 text-primary"><Briefcase size={18} /></div>
                  <h3 className="mt-4 font-semibold text-foreground group-hover:text-primary">{c.name}</h3>
                  <p className="mt-1 text-sm text-muted-foreground">{c.jobs} open jobs</p>
                </Link>
              </motion.div>
            ))}
          </div>
        </section>

        <section className="border-y border-border bg-card/30">
          <div className="mx-auto max-w-7xl px-4 py-16 sm:px-6">
            <div className="flex items-end justify-between gap-4">
              <SectionHeader eyebrow="Featured" title="Handpicked jobs" subtitle="Top opportunities from vetted clients." />
              <Link to="/jobs" className="hidden shrink-0 items-center gap-1 text-sm font-medium text-primary hover:underline sm:inline-flex">View all <ArrowRight size={14} /></Link>
            </div>
            <div className="mt-8 grid gap-5 md:grid-cols-2 lg:grid-cols-3">
              {featJobs.slice(0, 6).map((j) => <JobCard key={j.id} job={j} />)}
            </div>
          </div>
        </section>

        <section className="mx-auto max-w-7xl px-4 py-16 sm:px-6">
          <SectionHeader eyebrow="Top talent" title="Meet top-rated freelancers" subtitle="Handpicked pros with proven track records." />
          <div className="mt-8 grid gap-5 md:grid-cols-2 lg:grid-cols-4">
            {flancers.map((f) => <FreelancerCard key={f.userId} f={f} />)}
          </div>
        </section>

        <section className="border-y border-border bg-card/30">
          <div className="mx-auto max-w-7xl px-4 py-16 sm:px-6">
            <SectionHeader eyebrow="Top clients" title="Companies hiring on Workly" subtitle="Trusted businesses actively posting jobs." />
            <div className="mt-8 grid gap-5 md:grid-cols-2 lg:grid-cols-3">
              {clients.map((c) => <ClientCard key={c.userId} c={c} />)}
            </div>
          </div>
        </section>


        <section className="border-t border-border bg-gradient-to-b from-transparent to-primary-soft/40">
          <div className="mx-auto max-w-7xl px-4 py-20 sm:px-6">
            <SectionHeader eyebrow="How it works" title="Start in three simple steps" subtitle="Post, review, and hire — usually within 48 hours." />
            <div className="mt-10 grid gap-6 md:grid-cols-3">
              {[
                { n: "01", t: "Post your project", d: "Tell us what you need and set your budget in minutes." },
                { n: "02", t: "Review proposals", d: "Get proposals from top-vetted freelancers within hours." },
                { n: "03", t: "Hire and ship", d: "Collaborate seamlessly and release payment when satisfied." },
              ].map((s) => (
                <div key={s.n} className="rounded-2xl border border-border bg-card p-8 shadow-soft">
                  <div className="gradient-text text-4xl font-black">{s.n}</div>
                  <h3 className="mt-4 text-lg font-semibold">{s.t}</h3>
                  <p className="mt-2 text-sm text-muted-foreground">{s.d}</p>
                </div>
              ))}
            </div>
          </div>
        </section>

        <section className="mx-auto max-w-7xl px-4 py-16 sm:px-6">
          <SectionHeader eyebrow="Loved by teams" title="What our clients say" />
          <div className="mt-8 grid gap-5 md:grid-cols-3">
            {tests.map((t) => (
              <div key={t.id} className="rounded-2xl border border-border bg-card p-6 shadow-soft">
                <Rating value={5} showValue={false} />
                <p className="mt-3 text-sm text-foreground">"{t.text}"</p>
                <div className="mt-4 flex items-center gap-3">
                  <Avatar src={t.avatar} name={t.name} size={40} />
                  <div>
                    <div className="text-sm font-semibold">{t.name}</div>
                    <div className="text-xs text-muted-foreground">{t.role}</div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        </section>

        <section className="mx-auto max-w-7xl px-4 pb-20 sm:px-6">
          <div className="relative overflow-hidden rounded-3xl p-10 shadow-elevated md:p-16" style={{ background: "var(--gradient-primary)" }}>
            <div className="max-w-2xl text-primary-foreground">
              <h2 className="text-3xl font-bold sm:text-4xl">Ready to build something great?</h2>
              <p className="mt-3 text-primary-foreground/90">Join thousands of teams shipping faster with Workly.</p>
              <div className="mt-6 flex flex-wrap gap-3">
                <Link to="/register" className="rounded-full bg-white px-6 py-3 text-sm font-semibold text-primary hover:opacity-90">Get started free</Link>
                <Link to="/jobs" className="rounded-full border border-white/40 bg-white/10 px-6 py-3 text-sm font-semibold text-white hover:bg-white/20">Browse jobs</Link>
              </div>
            </div>
          </div>
        </section>
      </main>
      <Footer />
    </div>
  );
}
