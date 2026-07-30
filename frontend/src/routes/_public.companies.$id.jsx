import { createFileRoute, Link } from "@tanstack/react-router";
import { profileService, jobService, reviewService } from "@/services";
import { useAsync } from "@/hooks/useAsync";
import { Avatar } from "@/components/common/Avatar";
import { Rating } from "@/components/common/Rating";
import { Badge } from "@/components/common/Badge";
import { Skeleton, EmptyState, ErrorState } from "@/components/common/States";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { JobCard } from "@/components/jobs/JobCard";
import { formatDate } from "@/utils/format";
import { MapPin, Clock, Briefcase, Star } from "lucide-react";

export const Route = createFileRoute("/_public/companies/$id")({
  head: ({ params }) => ({ meta: [{ title: `Company profile — Workly` }, { name: "description", content: `View company ${params.id} and their open jobs.` }] }),
  component: ClientProfile,
});

function ClientProfile() {
  const { id } = Route.useParams();
  const { data: c, loading, error } = useAsync(() => profileService.getClient(id), [id]);
  const { data: jobs } = useAsync(() => jobService.byClient(id), [id]);
  const { data: reviews } = useAsync(() => reviewService.list({ userId: id }), [id]);

  if (loading) return <div className="mx-auto max-w-5xl px-4 py-10"><Skeleton className="h-64" /></div>;
  if (error) return <div className="mx-auto max-w-5xl px-4 py-10"><ErrorState description={error.message} /></div>;
  if (!c) return <div className="mx-auto max-w-5xl px-4 py-10"><EmptyState title="Company not found" /></div>;

  const open = (jobs || []).filter((j) => j.status === "Open");

  return (
    <div className="mx-auto max-w-6xl px-4 py-8 sm:px-6">
      <Breadcrumbs />
      <div className="mt-4 grid gap-6 lg:grid-cols-[1fr_320px]">
        <div className="space-y-6">
          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <div className="flex flex-wrap items-start gap-4">
              <Avatar src={c.logo} name={c.companyName} size={80} />
              <div className="min-w-0 flex-1">
                <h1 className="text-2xl font-bold">{c.companyName}</h1>
                <p className="text-sm text-muted-foreground">{c.location}</p>
                <div className="mt-2 flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
                  <Rating value={c.rating} />
                  <span>{c.reviews} reviews</span>
                  <span className="inline-flex items-center gap-1"><Clock size={11} />Replies {c.responseTime}</span>
                </div>
              </div>
            </div>
            <p className="mt-5 text-sm text-muted-foreground">{c.companyDetails}</p>
          </div>

          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <h2 className="text-lg font-semibold">Open jobs ({open.length})</h2>
            {open.length === 0 ? (
              <p className="mt-3 text-sm text-muted-foreground">No open jobs right now.</p>
            ) : (
              <div className="mt-4 grid gap-4 md:grid-cols-2">{open.map((j) => <JobCard key={j.id} job={j} />)}</div>
            )}
          </div>

          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <h2 className="text-lg font-semibold">Reviews from freelancers</h2>
            {!reviews || reviews.length === 0 ? (
              <EmptyState icon={Star} title="No reviews yet" />
            ) : (
              <div className="mt-4 space-y-3">
                {reviews.map((r) => (
                  <div key={r.id} className="rounded-xl border border-border bg-background p-4">
                    <div className="flex items-center justify-between">
                      <Rating value={r.rating} />
                      <span className="text-xs text-muted-foreground">{formatDate(r.createdAt)}</span>
                    </div>
                    <p className="mt-2 text-sm">{r.text}</p>
                  </div>
                ))}
              </div>
            )}
          </div>
        </div>

        <aside className="space-y-4">
          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <h3 className="text-sm font-semibold">Company stats</h3>
            <ul className="mt-4 space-y-2 text-sm text-muted-foreground">
              <li className="flex justify-between"><span>Jobs posted</span><span className="font-medium text-foreground">{c.jobsPosted}</span></li>
              <li className="flex justify-between"><span>Hire rate</span><span className="font-medium text-foreground">{c.hireRate}%</span></li>
              <li className="flex justify-between"><span>Open jobs</span><span className="font-medium text-foreground">{c.openJobs}</span></li>
              <li className="flex justify-between"><span>Response time</span><span className="font-medium text-foreground">{c.responseTime}</span></li>
              <li className="flex justify-between"><span>Member since</span><span className="font-medium text-foreground">{c.memberSince}</span></li>
              <li className="flex justify-between"><span>Location</span><span className="font-medium text-foreground inline-flex items-center gap-1"><MapPin size={11} />{c.location}</span></li>
            </ul>
          </div>
          <Link to="/jobs" className="block text-center text-sm text-muted-foreground hover:text-primary"><Briefcase size={12} className="mr-1 inline" />Browse all jobs</Link>
        </aside>
      </div>
    </div>
  );
}
