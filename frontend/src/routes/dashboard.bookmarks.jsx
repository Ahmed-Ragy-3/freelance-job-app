import { createFileRoute, Link } from "@tanstack/react-router";
import { bookmarkService } from "@/services";
import { JobCard } from "@/components/jobs/JobCard";
import { EmptyState, Skeleton, ErrorState } from "@/components/common/States";
import { Bookmark } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { useBookmarks } from "@/hooks/useBookmarks";

export const Route = createFileRoute("/dashboard/bookmarks")({
  head: () => ({ meta: [{ title: "Bookmarks — Workly" }, { name: "description", content: "Jobs you've saved for later." }] }),
  component: Bookmarks,
});

function Bookmarks() {
  const { user } = useAuth();
  const { data: items, loading, error, refetch } = useBookmarks(user?.id);
  const onToggle = async (jobId) => { await bookmarkService.toggle(user.id, jobId); refetch(); };

  return (
    <div className="space-y-5">
      <div>
        <h1 className="text-2xl font-bold">Bookmarks</h1>
        <p className="text-sm text-muted-foreground">Jobs you've saved for later.</p>
      </div>
      {loading ? (
        <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">{Array.from({ length: 3 }).map((_, i) => <Skeleton key={i} className="h-56" />)}</div>
      ) : error ? (
        <ErrorState description={error.message} onRetry={refetch} />
      ) : !items || items.length === 0 ? (
        <EmptyState icon={Bookmark} title="No bookmarks yet" description="Save jobs to revisit them later." action={<Link to="/jobs" className="rounded-md bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground">Browse jobs</Link>} />
      ) : (
        <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">
          {items.map((b) => b.job && <JobCard key={b.id} job={b.job} onBookmark={onToggle} bookmarked />)}
        </div>
      )}
    </div>
  );
}
