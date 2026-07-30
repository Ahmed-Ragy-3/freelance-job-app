import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { applicationService, jobService } from "@/services";
import { Avatar } from "@/components/common/Avatar";
import { Badge } from "@/components/common/Badge";
import { Button } from "@/components/ui/FormKit";
import { EmptyState } from "@/components/common/States";
import { formatMoney, timeAgo } from "@/utils/format";
import { Users } from "lucide-react";
import toast from "react-hot-toast";

export const Route = createFileRoute("/dashboard/jobs/$id/applications")({ component: JobApps });

function JobApps() {
  const { id } = Route.useParams();
  const [job, setJob] = useState(null);
  const [apps, setApps] = useState([]);
  useEffect(() => {
    jobService.get(id).then(setJob);
    applicationService.list({ jobId: id }).then(setApps);
  }, [id]);
  const setStatus = async (aid, status) => { await applicationService.updateStatus(aid, status); setApps((a) => a.map((x) => x.id === aid ? { ...x, status } : x)); toast.success(`Marked as ${status}`); };

  return (
    <div className="space-y-5">
      <div>
        <Link to="/dashboard/jobs" className="text-sm text-muted-foreground hover:text-foreground">← Back to jobs</Link>
        <h1 className="mt-2 text-2xl font-bold">{job?.title || "Job"}</h1>
        <p className="text-sm text-muted-foreground">{apps.length} applications</p>
      </div>
      {apps.length === 0 ? <EmptyState icon={Users} title="No applications yet" /> : (
        <div className="space-y-3">
          {apps.map((a) => (
            <div key={a.id} className="rounded-2xl border border-border bg-card p-5 shadow-soft">
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div className="flex items-center gap-3">
                  <Avatar src={`https://i.pravatar.cc/200?u=${a.freelancerId}`} name={a.freelancerId} size={44} />
                  <div>
                    <div className="font-semibold">Freelancer {a.freelancerId}</div>
                    <div className="text-xs text-muted-foreground">Bid {formatMoney(a.bid)} · {a.timeline} · Applied {timeAgo(a.createdAt)}</div>
                  </div>
                </div>
                <Badge variant={a.status === "Accepted" ? "success" : a.status === "Rejected" ? "danger" : a.status === "Shortlisted" ? "info" : "warning"}>{a.status}</Badge>
              </div>
              <p className="mt-3 text-sm text-muted-foreground">{a.coverLetter}</p>
              <div className="mt-4 flex flex-wrap justify-end gap-2">
                <Button size="sm" variant="outline" onClick={() => setStatus(a.id, "Rejected")}>Reject</Button>
                <Button size="sm" onClick={() => setStatus(a.id, "Accepted")}>Accept</Button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
