import { createFileRoute } from "@tanstack/react-router";
import { useNotificationsContext } from "@/context/NotificationContext";
import { Button } from "@/components/ui/FormKit";
import { EmptyState } from "@/components/common/States";
import { timeAgo } from "@/utils/format";
import { Bell, Trash2, CheckCheck } from "lucide-react";

export const Route = createFileRoute("/dashboard/notifications")({ component: Notifications });

function Notifications() {
  const { notifications, unreadCount, markRead, markAllRead, deleteNotification } = useNotificationsContext();

  return (
    <div className="space-y-5">
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold">Notifications</h1>
          <p className="text-sm text-muted-foreground">
            {unreadCount > 0 ? `You have ${unreadCount} unread notification${unreadCount > 1 ? "s" : ""}` : "All notifications read"}
          </p>
        </div>
        {unreadCount > 0 && (
          <Button variant="outline" size="sm" onClick={markAllRead} className="gap-1.5">
            <CheckCheck size={16} /> Mark all read
          </Button>
        )}
      </div>

      {notifications.length === 0 ? (
        <EmptyState icon={Bell} title="You're all caught up" description="No notifications found." />
      ) : (
        <div className="divide-y divide-border rounded-2xl border border-border bg-card shadow-soft overflow-hidden">
          {notifications.map((n) => {
            const isUnread = !n.read && !n.isRead;
            return (
              <div
                key={n.id}
                onClick={() => { if (isUnread) markRead(n.id); }}
                className={`flex items-start justify-between gap-4 p-4 transition-colors hover:bg-accent/40 ${isUnread ? "bg-primary/5 border-l-4 border-l-primary" : ""}`}
              >
                <div className="flex-1">
                  <div className="flex items-center gap-2">
                    <p className="text-sm font-semibold">{n.title}</p>
                    {isUnread && <span className="h-2 w-2 rounded-full bg-primary" />}
                  </div>
                  <p className="mt-1 text-sm text-muted-foreground">{n.message || n.body}</p>
                  <p className="mt-2 text-xs uppercase tracking-wide text-muted-foreground">{timeAgo(n.createdAt)}</p>
                </div>
                <div className="flex items-center gap-2">
                  <button
                    onClick={(e) => { e.stopPropagation(); deleteNotification(n.id); }}
                    className="p-1.5 text-muted-foreground hover:text-red-500 rounded-md hover:bg-accent"
                    title="Delete notification"
                  >
                    <Trash2 size={16} />
                  </button>
                </div>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
