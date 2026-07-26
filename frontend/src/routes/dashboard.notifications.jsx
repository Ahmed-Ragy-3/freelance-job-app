import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { notificationService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Button } from "@/components/ui/FormKit";
import { EmptyState } from "@/components/common/States";
import { timeAgo } from "@/utils/format";
import { Bell } from "lucide-react";

export const Route = createFileRoute("/dashboard/notifications")({ component: Notifications });

function Notifications() {
  const { user } = useAuth();
  const [items, setItems] = useState([]);
  useEffect(() => { if (user) notificationService.list(user.id).then(setItems); }, [user]);
  const markAll = async () => { await notificationService.markAllRead(user.id); notificationService.list(user.id).then(setItems); };
  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between"><h1 className="text-2xl font-bold">Notifications</h1><Button variant="outline" size="sm" onClick={markAll}>Mark all read</Button></div>
      {items.length === 0 ? <EmptyState icon={Bell} title="You're all caught up" /> : (
        <div className="divide-y divide-border rounded-2xl border border-border bg-card shadow-soft">
          {items.map((n) => (
            <div key={n.id} className={"flex items-start justify-between gap-3 p-4 " + (!n.read ? "bg-primary/5" : "")}>
              <div>
                <p className="text-sm font-medium">{n.title}</p>
                <p className="mt-0.5 text-sm text-muted-foreground">{n.body}</p>
                <p className="mt-1 text-xs uppercase tracking-wide text-muted-foreground">{timeAgo(n.createdAt)}</p>
              </div>
              {!n.read && <span className="mt-2 h-2 w-2 shrink-0 rounded-full bg-primary" />}
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
