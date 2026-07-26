import { Link } from "@tanstack/react-router";
import { Avatar } from "@/components/common/Avatar";
import { Rating } from "@/components/common/Rating";
import { MapPin, Briefcase, Clock } from "lucide-react";
import { motion } from "framer-motion";

export function ClientCard({ c }) {
  return (
    <motion.div whileHover={{ y: -3 }} className="rounded-2xl border border-border bg-card p-5 shadow-soft transition-shadow hover:shadow-elevated">
      <div className="flex items-start gap-4">
        <Avatar src={c.logo} name={c.companyName} size={56} />
        <div className="min-w-0 flex-1">
          <Link to="/companies/$id" params={{ id: c.userId }} className="truncate font-semibold text-foreground hover:text-primary">
            {c.companyName}
          </Link>
          <p className="truncate text-sm text-muted-foreground">{c.location}</p>
          <div className="mt-1 flex items-center gap-3 text-xs text-muted-foreground">
            <Rating value={c.rating} />
            <span>{c.reviews} reviews</span>
          </div>
        </div>
      </div>
      <p className="mt-3 line-clamp-2 text-sm text-muted-foreground">{c.companyDetails}</p>
      <div className="mt-4 flex items-center justify-between border-t border-border pt-3 text-sm text-muted-foreground">
        <span className="inline-flex items-center gap-1"><Briefcase size={13} />{c.openJobs} open</span>
        <span className="inline-flex items-center gap-1"><Clock size={13} />Replies {c.responseTime}</span>
      </div>
    </motion.div>
  );
}
