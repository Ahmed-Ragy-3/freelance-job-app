import { createFileRoute, Link } from "@tanstack/react-router";
import { profileService, reviewService } from "@/services";
import { useAsync } from "@/hooks/useAsync";
import { Avatar } from "@/components/common/Avatar";
import { Rating } from "@/components/common/Rating";
import { Badge } from "@/components/common/Badge";
import { Button } from "@/components/ui/FormKit";
import { Skeleton, EmptyState, ErrorState } from "@/components/common/States";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { formatMoney, formatDate } from "@/utils/format";
import { MapPin, Briefcase, ExternalLink, Star } from "lucide-react";
import { usePortfolio } from "@/hooks/useProfile";

export const Route = createFileRoute("/_public/freelancers/$id")({
  head: ({ params }) => ({ meta: [{ title: `Freelancer profile — Workly` }, { name: "description", content: `View freelancer ${params.id}'s profile, portfolio, and reviews.` }] }),
  component: FreelancerProfile,
});

function FreelancerProfile() {
  const { id } = Route.useParams();
  const { data: f, loading, error } = useAsync(() => profileService.getFreelancer(id), [id]);
  const { data: portfolio } = usePortfolio(id);
  const { data: reviews } = useAsync(() => reviewService.list({ to: id }), [id]);

  if (loading) return <div className="mx-auto max-w-5xl px-4 py-10"><Skeleton className="h-64" /></div>;
  if (error) return <div className="mx-auto max-w-5xl px-4 py-10"><ErrorState description={error.message} /></div>;
  if (!f) return <div className="mx-auto max-w-5xl px-4 py-10"><EmptyState title="Freelancer not found" /></div>;

  return (
    <div className="mx-auto max-w-6xl px-4 py-8 sm:px-6">
      <Breadcrumbs />
      <div className="mt-4 grid gap-6 lg:grid-cols-[1fr_320px]">
        <div className="space-y-6">
          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <div className="flex flex-wrap items-start gap-4">
              <Avatar src={f.imageUrl} name={f.name} size={80} />
              <div className="min-w-0 flex-1">
                <h1 className="text-2xl font-bold">{f.name}</h1>
                <p className="text-sm text-muted-foreground">{f.title}</p>
                <div className="mt-2 flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
                  <Rating value={f.rating} />
                  <span className="inline-flex items-center gap-1"><MapPin size={11} />{f.location}</span>
                  <span className="inline-flex items-center gap-1"><Briefcase size={11} />{f.completed} projects</span>
                </div>
              </div>
            </div>
            <p className="mt-5 text-sm text-muted-foreground">{f.bio}</p>
            <div className="mt-5 flex flex-wrap gap-1.5">{f.skills?.map((s) => <Badge key={s} variant="primary">{s}</Badge>)}</div>
          </div>

          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <h2 className="text-lg font-semibold">Portfolio</h2>
            {!portfolio || portfolio.length === 0 ? (
              <p className="mt-3 text-sm text-muted-foreground">No portfolio items yet.</p>
            ) : (
              <div className="mt-4 grid gap-4 sm:grid-cols-2">
                {portfolio.map((p) => (
                  <a key={p.id} href={p.url} className="group overflow-hidden rounded-xl border border-border bg-background">
                    <div className="aspect-video w-full bg-cover bg-center" style={{ backgroundImage: `url(${p.image})` }} />
                    <div className="p-4">
                      <h3 className="font-semibold group-hover:text-primary">{p.title}</h3>
                      <div className="mt-2 flex flex-wrap gap-1.5">{p.tags.map((t) => <Badge key={t} variant="outline">{t}</Badge>)}</div>
                    </div>
                  </a>
                ))}
              </div>
            )}
          </div>

          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <h2 className="text-lg font-semibold">Reviews ({f.reviews})</h2>
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
            <div className="text-sm text-muted-foreground">Hourly rate</div>
            <div className="mt-1 text-3xl font-bold">{formatMoney(f.averageRate)}<span className="text-base font-normal text-muted-foreground">/hr</span></div>
            <div className="mt-5 space-y-2">
              <Button className="w-full">Invite to job</Button>
              <Button variant="outline" className="w-full">Message</Button>
            </div>
          </div>
          <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
            <h3 className="text-sm font-semibold">Details</h3>
            <ul className="mt-4 space-y-2 text-sm text-muted-foreground">
              <li className="flex justify-between"><span>Completed jobs</span><span className="font-medium text-foreground">{f.completed}</span></li>
              <li className="flex justify-between"><span>Reviews</span><span className="font-medium text-foreground">{f.reviews}</span></li>
              <li className="flex justify-between"><span>Rating</span><span className="font-medium text-foreground">{f.rating}</span></li>
              <li className="flex justify-between"><span>Website</span><a href="#" className="inline-flex items-center gap-1 font-medium text-primary hover:underline">{f.link} <ExternalLink size={11} /></a></li>
            </ul>
          </div>
          <Link to="/jobs" className="block text-center text-sm text-muted-foreground hover:text-primary">← Back to jobs</Link>
        </aside>
      </div>
    </div>
  );
}
