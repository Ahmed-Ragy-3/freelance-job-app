import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { reviewService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Rating } from "@/components/common/Rating";
import { Avatar } from "@/components/common/Avatar";
import { Badge } from "@/components/common/Badge";
import { Button } from "@/components/ui/FormKit";
import { EmptyState, Skeleton, ErrorState } from "@/components/common/States";
import { ReviewDialog } from "@/components/reviews/ReviewDialog";
import { formatDate } from "@/utils/format";
import { Star, CheckCircle2 } from "lucide-react";

export const Route = createFileRoute("/dashboard/reviews")({ component: Reviews });

function Reviews() {
  const { user, isClient } = useAuth();
  const [items, setItems] = useState([]);
  const [reviewable, setReviewable] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [target, setTarget] = useState(null);

  const load = async () => {
    if (!user) return;
    setLoading(true);
    try {
      const [list, jobs] = await Promise.all([
        isClient ? reviewService.list({ from: user.id }) : reviewService.list({ to: user.id }),
        isClient ? reviewService.reviewableJobs(user.id) : Promise.resolve([]),
      ]);
      setItems(list);
      setReviewable(jobs);
      setError(null);
    } catch (e) {
      setError(e);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); /* eslint-disable-next-line react-hooks/exhaustive-deps */ }, [user, isClient]);

  const pending = reviewable.filter((r) => !r.review);

  return (
    <div className="space-y-5">
      <h1 className="text-2xl font-bold">{isClient ? "Reviews given" : "Reviews"}</h1>

      {loading ? (
        <div className="space-y-3"><Skeleton className="h-24" /><Skeleton className="h-24" /></div>
      ) : error ? (
        <ErrorState description={error.message} />
      ) : (
        <>
          {isClient && (
            <div className="rounded-2xl border border-border bg-card p-5 shadow-soft">
              <h2 className="text-sm font-semibold">Completed jobs awaiting your review</h2>
              {pending.length === 0 ? (
                <p className="mt-3 text-sm text-muted-foreground">No completed jobs are waiting for a review.</p>
              ) : (
                <div className="mt-4 space-y-3">
                  {pending.map((r) => (
                    <div key={r.job.id} className="flex flex-wrap items-center justify-between gap-3 rounded-xl border border-border bg-background p-4">
                      <div className="min-w-0">
                        <div className="truncate text-sm font-medium">{r.job.title}</div>
                        <div className="mt-1 flex items-center gap-2 text-xs text-muted-foreground">
                          <Badge variant="success">Completed</Badge>
                          <span>{r.freelancerName}</span>
                        </div>
                      </div>
                      <Button size="sm" onClick={() => setTarget(r)}><Star size={13} />Leave review</Button>
                    </div>
                  ))}
                </div>
              )}
            </div>
          )}

          {items.length === 0 ? (
            <EmptyState icon={Star} title={isClient ? "No reviews given yet" : "No reviews yet"} description={isClient ? "Reviews you leave for freelancers appear here." : "Reviews from completed projects will appear here."} />
          ) : (
            <div className="space-y-3">
              {items.map((r) => (
                <div key={r.id} className="rounded-2xl border border-border bg-card p-5 shadow-soft">
                  <div className="flex flex-wrap items-center justify-between gap-3">
                    <div className="flex items-center gap-3">
                      <Avatar src={`https://i.pravatar.cc/200?u=${isClient ? r.to : r.from}`} name="User" size={40} />
                      <div>
                        <div className="text-sm font-semibold">
                          {isClient ? (r.targetName || "Freelancer") : (r.author?.username || "Client")}
                        </div>
                        <Rating value={r.rating} />
                      </div>
                    </div>
                    <div className="text-xs text-muted-foreground">{formatDate(r.createdAt)}</div>
                  </div>
                  {r.job && (
                    <div className="mt-3 inline-flex items-center gap-1 text-xs text-muted-foreground">
                      <CheckCircle2 size={12} className="text-primary" />
                      <Link to="/jobs/$id" params={{ id: r.job.id }} className="hover:text-primary">{r.job.title}</Link>
                    </div>
                  )}
                  <p className="mt-3 text-sm text-foreground">{r.text}</p>
                </div>
              ))}
            </div>
          )}
        </>
      )}

      <ReviewDialog
        open={!!target}
        onClose={() => setTarget(null)}
        job={target?.job}
        freelancerName={target?.freelancerName}
        clientId={user?.id}
        onSubmitted={load}
      />
    </div>
  );
}
