import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { useState, useEffect } from "react";
import { jobService, bookmarkService, staticData, profileService } from "@/services";
import { Badge } from "@/components/common/Badge";
import { Avatar } from "@/components/common/Avatar";
import { Rating } from "@/components/common/Rating";
import { Button } from "@/components/ui/FormKit";
import { Skeleton, ErrorState, EmptyState } from "@/components/common/States";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { formatMoney, formatDate, daysUntil, timeAgo } from "@/utils/format";
import { Bookmark, Share2, Clock, Users, MapPin, Calendar, Paperclip, Layers, GraduationCap, Zap, TrendingUp } from "lucide-react";
import { JobCard } from "@/components/jobs/JobCard";
import { useAuth } from "@/context/AuthContext";
import { useJob, useSimilarJobs } from "@/hooks/useJobs";
import { useAsync } from "@/hooks/useAsync";
import toast from "react-hot-toast";

export const Route = createFileRoute("/_public/jobs/$id")({
  head: ({ params }) => ({ meta: [{ title: `Job ${params.id} — Workly` }, { name: "description", content: "View job details and apply." }] }),
  component: JobDetail,
});

function JobDetail() {
  const { id } = Route.useParams();
  const { data: job, loading, error } = useJob(id);
  const { data: similar } = useSimilarJobs(id);
  const { data: cats } = useAsync(() => staticData.categories(), []);
  const { data: client } = useAsync(() => (job?.clientId ? profileService.getClient(job.clientId) : Promise.resolve(null)), [job?.clientId]);
  const [bookmarked, setBookmarked] = useState(false);
  const { user, isAuthenticated, isFreelancer, role } = useAuth();
  const navigate = useNavigate();

  useEffect(() => { if (user && id) bookmarkService.isBookmarked(user.id, id).then(setBookmarked); }, [user, id]);

  const onBookmark = async () => {
    if (!isAuthenticated) { toast.error("Sign in to bookmark"); return; }
    const { bookmarked } = await bookmarkService.toggle(user.id, id);
    setBookmarked(bookmarked);
    toast.success(bookmarked ? "Bookmarked" : "Removed bookmark");
  };
  const share = () => { navigator.clipboard.writeText(window.location.href); toast.success("Link copied"); };

  if (loading) return <div className="mx-auto max-w-5xl px-4 py-10"><Skeleton className="h-80" /></div>;
  if (error) return <div className="mx-auto max-w-5xl px-4 py-10"><ErrorState description={error.message} /></div>;
  if (!job) return <div className="mx-auto max-w-5xl px-4 py-10"><EmptyState title="Job not found" /></div>;
  const cat = (cats || []).find((c) => c.id === job.categoryId);

  return (
    <div className="mx-auto max-w-6xl px-4 py-8 sm:px-6">
      <Breadcrumbs />
      <div className="mt-4 grid gap-6 lg:grid-cols-[1fr_320px]">
        <div>
          <div className="overflow-hidden rounded-2xl border border-border bg-card shadow-soft">
            <div className="h-40 w-full bg-cover bg-center" style={{ backgroundImage: `url(${job.cover})` }} />
            <div className="p-6">
              <div className="flex flex-wrap items-center gap-2">
                <Badge variant={job.status === "Open" ? "success" : "info"}>{job.status}</Badge>
                {cat && <Badge variant="outline">{cat.name}</Badge>}
                {job.tags?.map((t) => <Badge key={t} variant="primary">{t}</Badge>)}
              </div>
              <h1 className="mt-3 text-3xl font-bold tracking-tight">{job.title}</h1>
              <div className="mt-3 flex flex-wrap items-center gap-4 text-sm text-muted-foreground">
                <span className="inline-flex items-center gap-1"><Calendar size={14} /> Posted {timeAgo(job.createdAt)}</span>
                <span className="inline-flex items-center gap-1"><Clock size={14} /> Due {formatDate(job.deadline)}</span>
                <span className="inline-flex items-center gap-1"><Users size={14} /> {job.proposals} proposals</span>
              </div>


              <div className="mt-6">
                <h2 className="text-lg font-semibold">About the project</h2>
                <p className="mt-2 leading-relaxed text-muted-foreground">{job.description}</p>
              </div>
              <div className="mt-6">
                <h3 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Required skills</h3>
                <div className="mt-3 flex flex-wrap gap-1.5">{job.requiredSkills?.map((s) => <Badge key={s} variant="primary">{s}</Badge>)}</div>
              </div>
              {job.attachments?.length ? (
                <div className="mt-6">
                  <h3 className="text-sm font-semibold uppercase tracking-wider text-muted-foreground">Attachments</h3>
                  <ul className="mt-2 space-y-1">{job.attachments.map((a, i) => <li key={i} className="inline-flex items-center gap-2 text-sm text-primary hover:underline"><Paperclip size={14} />{a}</li>)}</ul>
                </div>
              ) : null}
            </div>
          </div>

          <div className="mt-6 rounded-2xl border border-border bg-card p-6 shadow-soft">
            <h2 className="text-lg font-semibold">Similar jobs</h2>
            <div className="mt-4 grid gap-4 md:grid-cols-2">
              {!similar || similar.length === 0 ? <p className="text-sm text-muted-foreground">No similar jobs yet.</p> : similar.map((j) => <JobCard key={j.id} job={j} />)}
            </div>
          </div>
        </div>

        <aside className="space-y-4">
          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <div className="text-sm text-muted-foreground">{job.projectType === "Hourly" ? "Hourly rate" : "Fixed budget"}</div>
            <div className="mt-1 text-3xl font-bold">{formatMoney(job.budget)}</div>
            <div className="mt-1 text-xs text-muted-foreground">{daysUntil(job.deadline)} days to apply</div>
            <div className="mt-5 space-y-2">
              {!isAuthenticated ? (
                <Button className="w-full" onClick={() => navigate({ to: "/login" })}>Sign in to apply</Button>
              ) : isFreelancer ? (
                <Button className="w-full" onClick={() => navigate({ to: "/jobs/$id/apply", params: { id } })}>Apply now</Button>
              ) : (
                <Button className="w-full" disabled title="Only freelancers can apply">Only freelancers can apply</Button>
              )}
              <div className="grid grid-cols-2 gap-2">
                <Button variant="outline" onClick={onBookmark}><Bookmark size={14} className={bookmarked ? "fill-primary text-primary" : ""} />{bookmarked ? "Saved" : "Save"}</Button>
                <Button variant="outline" onClick={share}><Share2 size={14} />Share</Button>
              </div>
            </div>
          </div>
          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <h3 className="text-sm font-semibold">About the client</h3>
            <div className="mt-4 flex items-center gap-3">
              <Avatar src={client?.logo || `https://i.pravatar.cc/200?u=${job.clientId}`} name={client?.companyName || "Client"} size={48} />
              <div className="min-w-0">
                <Link to="/companies/$id" params={{ id: job.clientId }} className="block truncate font-semibold hover:text-primary">{client?.companyName || "Client"}</Link>
                <Rating value={client?.rating ?? 4.8} />
              </div>
            </div>
            <ul className="mt-4 space-y-2 text-sm text-muted-foreground">
              <li className="flex justify-between"><span className="inline-flex items-center gap-1"><TrendingUp size={12} />Jobs posted</span><span className="font-medium text-foreground">{client?.jobsPosted ?? "—"}</span></li>
              <li className="flex justify-between"><span>Hire rate</span><span className="font-medium text-foreground">{client?.hireRate ?? "—"}%</span></li>
              <li className="flex justify-between"><span className="inline-flex items-center gap-1"><Zap size={12} />Response time</span><span className="font-medium text-foreground">{client?.responseTime ?? "—"}</span></li>
              <li className="flex justify-between"><span>Member since</span><span className="font-medium text-foreground">{client?.memberSince ?? "—"}</span></li>
              <li className="flex justify-between"><span>Location</span><span className="font-medium text-foreground inline-flex items-center gap-1"><MapPin size={11} />{client?.location ?? "—"}</span></li>
            </ul>
            <Link to="/companies/$id" params={{ id: job.clientId }} className="mt-4 inline-block text-xs font-semibold text-primary hover:underline">View company profile →</Link>
          </div>
        </aside>
      </div>
    </div>
  );
}
