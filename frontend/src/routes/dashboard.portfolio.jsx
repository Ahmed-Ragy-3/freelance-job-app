import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { profileService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Badge } from "@/components/common/Badge";
import { EmptyState } from "@/components/common/States";
import { FileText, ExternalLink } from "lucide-react";

export const Route = createFileRoute("/dashboard/portfolio")({ component: Portfolio });

function Portfolio() {
  const { user } = useAuth();
  const [items, setItems] = useState([]);
  useEffect(() => { if (user) profileService.portfolio(user.id).then(setItems); }, [user]);
  return (
    <div className="space-y-5">
      <h1 className="text-2xl font-bold">Portfolio</h1>
      {items.length === 0 ? <EmptyState icon={FileText} title="No portfolio items" description="Add your best work to attract clients." /> : (
        <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">
          {items.map((p) => (
            <a key={p.id} href={p.url} className="group overflow-hidden rounded-2xl border border-border bg-card shadow-soft transition-shadow hover:shadow-elevated">
              <div className="aspect-video w-full bg-cover bg-center" style={{ backgroundImage: `url(${p.image})` }} />
              <div className="p-4">
                <h3 className="font-semibold group-hover:text-primary">{p.title}</h3>
                <div className="mt-2 flex flex-wrap gap-1.5">{p.tags.map((t) => <Badge key={t} variant="primary">{t}</Badge>)}</div>
                <div className="mt-3 inline-flex items-center gap-1 text-xs text-primary">View project <ExternalLink size={12} /></div>
              </div>
            </a>
          ))}
        </div>
      )}
    </div>
  );
}
