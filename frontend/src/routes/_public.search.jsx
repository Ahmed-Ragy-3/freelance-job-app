import { createFileRoute, useNavigate, Link } from "@tanstack/react-router";
import { useState, useEffect } from "react";
import { jobService, userService } from "@/services";
import { JobCard } from "@/components/jobs/JobCard";
import { FreelancerCard } from "@/components/profile/FreelancerCard";
import { ClientCard } from "@/components/profile/ClientCard";
import { Skeleton, EmptyState } from "@/components/common/States";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { Input, Button } from "@/components/ui/FormKit";
import { Search as SearchIcon } from "lucide-react";

export const Route = createFileRoute("/_public/search")({
  validateSearch: (s) => ({ q: typeof s.q === "string" ? s.q : "", type: s.type === "freelancers" || s.type === "clients" ? s.type : "jobs" }),
  head: ({ search }) => ({ meta: [{ title: `Search${search.q ? ` — "${search.q}"` : ""} — Workly` }, { name: "description", content: "Search jobs, freelancers, and companies." }] }),
  component: SearchPage,
});

function Tab({ active, onClick, children }) {
  return <button onClick={onClick} className={`rounded-full px-4 py-1.5 text-sm font-medium ${active ? "bg-primary text-primary-foreground" : "border border-border bg-card hover:bg-accent"}`}>{children}</button>;
}

function SearchPage() {
  const { q, type } = Route.useSearch();
  const navigate = useNavigate();
  const [input, setInput] = useState(q);
  const [jobs, setJobs] = useState(null);
  const [people, setPeople] = useState(null);

  useEffect(() => { setInput(q); }, [q]);
  useEffect(() => {
    setJobs(null); setPeople(null);
    jobService.list({ search: q, pageSize: 12 }).then((r) => setJobs(r.items));
    userService.search(q).then(setPeople);
  }, [q]);

  const setSearch = (patch) => navigate({ to: "/search", search: { q, type, ...patch } });
  const onSubmit = (e) => { e.preventDefault(); setSearch({ q: input }); };

  return (
    <div className="mx-auto max-w-6xl px-4 py-8 sm:px-6">
      <Breadcrumbs />
      <h1 className="mt-4 text-3xl font-bold tracking-tight">Search</h1>
      <form onSubmit={onSubmit} className="mt-4 flex flex-wrap gap-2">
        <div className="relative flex-1 min-w-[240px]">
          <SearchIcon size={16} className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
          <Input value={input} onChange={(e) => setInput(e.target.value)} placeholder="Search jobs, skills, companies..." className="pl-9" />
        </div>
        <Button>Search</Button>
      </form>

      <div className="mt-6 flex flex-wrap gap-2">
        <Tab active={type === "jobs"} onClick={() => setSearch({ type: "jobs" })}>Jobs {jobs && `(${jobs.length})`}</Tab>
        <Tab active={type === "freelancers"} onClick={() => setSearch({ type: "freelancers" })}>Freelancers {people && `(${people.freelancers.length})`}</Tab>
        <Tab active={type === "clients"} onClick={() => setSearch({ type: "clients" })}>Companies {people && `(${people.clients.length})`}</Tab>
      </div>

      <div className="mt-6">
        {type === "jobs" && (
          jobs === null ? <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">{Array.from({ length: 6 }).map((_, i) => <Skeleton key={i} className="h-56" />)}</div>
          : jobs.length === 0 ? <EmptyState icon={SearchIcon} title="No jobs matched" description="Try a different keyword." action={<Link to="/jobs" className="rounded-md bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground">Browse all jobs</Link>} />
          : <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">{jobs.map((j) => <JobCard key={j.id} job={j} />)}</div>
        )}
        {type === "freelancers" && (
          !people ? <Skeleton className="h-40" />
          : people.freelancers.length === 0 ? <EmptyState icon={SearchIcon} title="No freelancers matched" />
          : <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">{people.freelancers.map((f) => <FreelancerCard key={f.userId} f={f} />)}</div>
        )}
        {type === "clients" && (
          !people ? <Skeleton className="h-40" />
          : people.clients.length === 0 ? <EmptyState icon={SearchIcon} title="No companies matched" />
          : <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">{people.clients.map((c) => <ClientCard key={c.userId} c={{ ...c, rating: c.rating || 4.8, reviews: c.reviews || 20, openJobs: c.openJobs || 0, responseTime: c.responseTime || "under 6 hours" }} />)}</div>
        )}
      </div>
    </div>
  );
}
