import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { jobService, staticData } from "@/services";
import { JobCard } from "@/components/jobs/JobCard";
import { Pagination } from "@/components/common/Pagination";
import { EmptyState, Skeleton } from "@/components/common/States";
import { Field, Input, Select, Button } from "@/components/ui/FormKit";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { LayoutGrid, List, Search, SlidersHorizontal, Briefcase } from "lucide-react";
import { Badge } from "@/components/common/Badge";

export const Route = createFileRoute("/_public/jobs/")({
  head: () => ({ meta: [{ title: "Browse jobs — Workly" }, { name: "description", content: "Discover freelance jobs across design, development, marketing, and more." }] }),
  component: JobsList,
});

function JobsList() {
  const [items, setItems] = useState([]);
  const [total, setTotal] = useState(0);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [view, setView] = useState("grid");
  const [cats, setCats] = useState([]);
  const [allSkills, setAllSkills] = useState([]);
  const [filters, setFilters] = useState({ search: "", category: "", status: "", sort: "newest", minBudget: 0, maxBudget: 100000, skills: [] });
  const [showFilters, setShowFilters] = useState(false);

  useEffect(() => { staticData.categories().then(setCats); staticData.skills().then(setAllSkills); }, []);
  useEffect(() => {
    setLoading(true);
    jobService.list({ ...filters, page, pageSize: 9 }).then(({ items, total }) => { setItems(items); setTotal(total); setLoading(false); });
  }, [filters, page]);

  const toggleSkill = (skillId) => setFilters((f) => ({
    ...f,
    skills: f.skills.includes(skillId) ? f.skills.filter((x) => x !== skillId) : [...f.skills, skillId],
  }));

  return (
    <div className="mx-auto max-w-7xl px-4 py-8 sm:px-6">
      <Breadcrumbs />
      <div className="mt-4 flex flex-wrap items-end justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Browse jobs</h1>
          <p className="mt-1 text-sm text-muted-foreground">{total} opportunities matching your filters</p>
        </div>
        <div className="flex items-center gap-2">
          <button onClick={() => setShowFilters((v) => !v)} className="inline-flex items-center gap-2 rounded-lg border border-border px-3 py-2 text-sm lg:hidden"><SlidersHorizontal size={14} /> Filters</button>
          <div className="flex rounded-lg border border-border p-0.5">
            <button onClick={() => setView("grid")} className={"rounded-md p-1.5 " + (view === "grid" ? "bg-accent text-primary" : "text-muted-foreground")}><LayoutGrid size={16} /></button>
            <button onClick={() => setView("list")} className={"rounded-md p-1.5 " + (view === "list" ? "bg-accent text-primary" : "text-muted-foreground")}><List size={16} /></button>
          </div>
        </div>
      </div>

      <div className="mt-6 grid gap-6 lg:grid-cols-[260px_1fr]">
        <aside className={"space-y-5 " + (showFilters ? "block" : "hidden lg:block")}>
          <div className="rounded-2xl border border-border bg-card p-5 shadow-soft">
            <h3 className="text-sm font-semibold">Filters</h3>
            <div className="mt-4 space-y-4">
              <Field label="Search"><div className="relative"><Search size={14} className="absolute left-3 top-3 text-muted-foreground" /><Input value={filters.search} onChange={(e) => { setFilters({ ...filters, search: e.target.value }); setPage(1); }} placeholder="Keyword" className="pl-9" /></div></Field>
              <Field label="Category">
                <Select value={filters.category} onChange={(e) => { setFilters({ ...filters, category: e.target.value }); setPage(1); }}>
                  <option value="">All categories</option>
                  {cats.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
                </Select>
              </Field>
              <Field label="Status">
                <Select value={filters.status} onChange={(e) => { setFilters({ ...filters, status: e.target.value }); setPage(1); }}>
                  <option value="">All statuses</option>
                  <option>Open</option><option>In Progress</option><option>Closed</option>
                </Select>
              </Field>
              <Field label={`Budget: $${filters.minBudget.toLocaleString()} — $${filters.maxBudget.toLocaleString()}`}>
                <input type="range" min="0" max="10000" step="500" value={filters.maxBudget} onChange={(e) => { setFilters({ ...filters, maxBudget: +e.target.value }); setPage(1); }} className="w-full accent-primary" />
              </Field>
              <Field label="Sort by">
                <Select value={filters.sort} onChange={(e) => setFilters({ ...filters, sort: e.target.value })}>
                  <option value="newest">Newest</option>
                  <option value="oldest">Oldest</option>
                  <option value="budget">Highest budget</option>
                  <option value="deadline">Closest deadline</option>
                </Select>
              </Field>
              <div>
                <label className="mb-1.5 block text-sm font-medium">Skills</label>
                <div className="flex max-h-40 flex-wrap gap-1.5 overflow-y-auto">
                  {allSkills.slice(0, 16).map((s) => {
                    const id = typeof s === "object" ? s.id : s;
                    const label = typeof s === "object" ? s.name : s;
                    return (
                      <button key={id} type="button" onClick={() => { toggleSkill(id); setPage(1); }} className={"rounded-full border px-2.5 py-1 text-xs " + (filters.skills.includes(id) ? "border-primary bg-primary/10 text-primary" : "border-border")}>{label}</button>
                    );
                  })}
                </div>
              </div>
              <Button variant="outline" className="w-full" onClick={() => { setFilters({ search: "", category: "", status: "", sort: "newest", minBudget: 0, maxBudget: 100000, skills: [] }); setPage(1); }}>Reset</Button>
            </div>
          </div>
        </aside>

        <div>
          {loading ? (
            <div className={"grid gap-5 " + (view === "grid" ? "md:grid-cols-2 xl:grid-cols-3" : "")}>
              {Array.from({ length: 6 }).map((_, i) => <Skeleton key={i} className="h-56 rounded-2xl" />)}
            </div>
          ) : items.length === 0 ? (
            <EmptyState icon={Briefcase} title="No jobs found" description="Try widening your filters to see more results." />
          ) : (
            <>
              <div className={"grid gap-5 " + (view === "grid" ? "md:grid-cols-2 xl:grid-cols-3" : "")}>
                {items.map((j) => <JobCard key={j.id} job={j} view={view} />)}
              </div>
              <div className="mt-8"><Pagination page={page} pageSize={9} total={total} onPage={setPage} /></div>
            </>
          )}
        </div>
      </div>
    </div>
  );
}
