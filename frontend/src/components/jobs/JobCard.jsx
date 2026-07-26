import { Link } from "@tanstack/react-router";
import { Badge } from "@/components/common/Badge";
import { formatMoney, timeAgo, daysUntil } from "@/utils/format";
import { Bookmark, Clock, Users, MapPin } from "lucide-react";
import { motion } from "framer-motion";

export function JobCard({ job, view = "grid", onBookmark, bookmarked }) {
  const statusVariant = job.status === "Open" ? "success" : job.status === "In Progress" ? "info" : "default";
  return (
    <motion.article
      whileHover={{ y: -3 }}
      className="group flex h-full flex-col rounded-2xl border border-border bg-card p-5 shadow-soft transition-shadow hover:shadow-elevated"
    >
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0 flex-1">
          <div className="flex flex-wrap items-center gap-2">
            <Badge variant={statusVariant}>{job.status}</Badge>
            {job.tags?.slice(0, 2).map((t) => <Badge key={t} variant="outline">{t}</Badge>)}
          </div>
          <Link to="/jobs/$id" params={{ id: job.id }} className="mt-3 block">
            <h3 className="line-clamp-2 text-base font-semibold text-foreground group-hover:text-primary">{job.title}</h3>
          </Link>
        </div>
        {onBookmark && (
          <button onClick={(e) => { e.preventDefault(); onBookmark(job.id); }} className="rounded-full p-2 text-muted-foreground hover:bg-accent">
            <Bookmark size={18} className={bookmarked ? "fill-primary text-primary" : ""} />
          </button>
        )}
      </div>
      <p className="mt-2 line-clamp-2 text-sm text-muted-foreground">{job.description}</p>
      <div className="mt-4 flex flex-wrap gap-1.5">
        {job.requiredSkills?.slice(0, 4).map((s) => <Badge key={s} variant="primary">{s}</Badge>)}
      </div>
      <div className="mt-5 flex flex-wrap items-center justify-between gap-3 border-t border-border pt-4">
        <div className="text-lg font-bold text-foreground">{formatMoney(job.budget)}</div>
        <div className="flex items-center gap-3 text-xs text-muted-foreground">
          <span className="inline-flex items-center gap-1"><Clock size={12} />{daysUntil(job.deadline)}d left</span>
          <span className="inline-flex items-center gap-1"><Users size={12} />{job.proposals}</span>
          <span>{timeAgo(job.createdAt)}</span>
        </div>
      </div>
    </motion.article>
  );
}
