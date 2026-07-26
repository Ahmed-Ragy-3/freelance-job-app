import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { reviewService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Rating } from "@/components/common/Rating";
import { Avatar } from "@/components/common/Avatar";
import { EmptyState } from "@/components/common/States";
import { formatDate } from "@/utils/format";
import { Star } from "lucide-react";

export const Route = createFileRoute("/dashboard/reviews")({ component: Reviews });

function Reviews() {
  const { user } = useAuth();
  const [items, setItems] = useState([]);
  useEffect(() => { if (user) reviewService.list({ userId: user.id }).then(setItems); }, [user]);
  return (
    <div className="space-y-5">
      <h1 className="text-2xl font-bold">Reviews</h1>
      {items.length === 0 ? <EmptyState icon={Star} title="No reviews yet" description="Reviews from completed projects will appear here." /> : (
        <div className="space-y-3">
          {items.map((r) => (
            <div key={r.id} className="rounded-2xl border border-border bg-card p-5 shadow-soft">
              <div className="flex items-center justify-between">
                <div className="flex items-center gap-3">
                  <Avatar src={`https://i.pravatar.cc/200?u=${r.from}`} name="User" size={40} />
                  <div><div className="text-sm font-semibold">Anonymous</div><Rating value={r.rating} /></div>
                </div>
                <div className="text-xs text-muted-foreground">{formatDate(r.createdAt)}</div>
              </div>
              <p className="mt-3 text-sm text-foreground">{r.text}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
