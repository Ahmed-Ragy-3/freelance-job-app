import { Avatar } from "@/components/common/Avatar";
import { Rating } from "@/components/common/Rating";
import { Badge } from "@/components/common/Badge";
import { formatMoney } from "@/utils/format";
import { MapPin, Briefcase } from "lucide-react";
import { motion } from "framer-motion";

export function FreelancerCard({ f }) {
  return (
    <motion.div whileHover={{ y: -3 }} className="rounded-2xl border border-border bg-card p-5 shadow-soft transition-shadow hover:shadow-elevated">
      <div className="flex items-start gap-4">
        <Avatar src={`https://i.pravatar.cc/200?u=${f.userId}`} name={f.name} size={56} />
        <div className="min-w-0 flex-1">
          <h3 className="truncate font-semibold text-foreground">{f.name}</h3>
          <p className="truncate text-sm text-muted-foreground">{f.title}</p>
          <div className="mt-1 flex items-center gap-3 text-xs text-muted-foreground">
            <Rating value={f.rating} />
            <span className="inline-flex items-center gap-1"><MapPin size={11} />{f.location}</span>
          </div>
        </div>
      </div>
      <p className="mt-3 line-clamp-2 text-sm text-muted-foreground">{f.bio}</p>
      <div className="mt-3 flex flex-wrap gap-1.5">
        {f.skills.slice(0, 3).map((s) => <Badge key={s} variant="primary">{s}</Badge>)}
      </div>
      <div className="mt-4 flex items-center justify-between border-t border-border pt-3 text-sm">
        <span className="inline-flex items-center gap-1 text-muted-foreground"><Briefcase size={13} />{f.completed} jobs</span>
        <span className="font-semibold text-foreground">{formatMoney(f.averageRate)}/hr</span>
      </div>
    </motion.div>
  );
}
