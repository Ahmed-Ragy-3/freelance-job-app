import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { applicationService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Badge } from "@/components/common/Badge";
import { Button } from "@/components/ui/FormKit";
import { EmptyState } from "@/components/common/States";
import { formatMoney, timeAgo } from "@/utils/format";
import { Briefcase, Eye } from "lucide-react";
import toast from "react-hot-toast";

export const Route = createFileRoute("/dashboard/applications")({ component: Applications });

function Applications() {
  const { user } = useAuth();
  const [items, setItems] = useState([]);
  const [tab, setTab] = useState("all");
  useEffect(() => { if (user) applicationService.list({ freelancerId: user.id }).then(setItems); }, [user]);

  const withdraw = async (jobId) => {
    if (!confirm("Withdraw this application?")) return;
    try {
      await applicationService.withdraw(jobId);
      setItems((prev) => prev.filter((a) => String(a.jobId) !== String(jobId)));
      toast.success("Application withdrawn");
    } catch (err) {
      toast.error(err?.message || "Could not withdraw application");
    }
  };
  const canWithdraw = (status) => ["In_Progress", "Draft", "Pending", "Submitted"].includes(status);
  const filtered = tab === "all" ? items : items.filter((a) => a.status.toLowerCase().replace("_", " ") === tab || a.status.toLowerCase() === tab);

  return (
    <div className="space-y-5">
      <h1 className="text-2xl font-bold">Applications</h1>
      <div className="flex flex-wrap gap-2">
        {["all","pending","shortlisted","accepted","rejected"].map((t) => (
          <button key={t} onClick={() => setTab(t)} className={"rounded-full border px-3 py-1.5 text-sm capitalize " + (tab === t ? "border-primary bg-primary/10 text-primary" : "border-border")}>{t}</button>
        ))}
      </div>
      {filtered.length === 0 ? <EmptyState icon={Briefcase} title="No applications" description="Apply to jobs to see them here." /> : (
        <div className="space-y-3">
          {filtered.map((a) => (
            <div key={a.jobId ?? a.id} className="rounded-2xl border border-border bg-card p-5 shadow-soft">
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div className="min-w-0">
                  <Link to="/jobs/$id" params={{ id: a.jobId }} className="font-semibold hover:text-primary">{a.job?.title || a.jobTitle || "Job removed"}</Link>
                  <div className="mt-1 flex flex-wrap items-center gap-3 text-xs text-muted-foreground">
                    <span>Bid: <strong className="text-foreground">{formatMoney(a.bid)}</strong></span>
                    <span>Timeline: {a.timeline}</span>
                    <span>Applied {timeAgo(a.createdAt)}</span>
                  </div>
                </div>
                <Badge variant={a.status === "Accepted" ? "success" : a.status === "Rejected" ? "danger" : a.status === "Shortlisted" ? "info" : "warning"}>{a.status}</Badge>
              </div>
              <p className="mt-3 line-clamp-2 text-sm text-muted-foreground">{a.coverLetter}</p>
              <div className="mt-4 flex justify-end gap-2">
                <Link to="/jobs/$id" params={{ id: a.jobId }}><Button variant="outline" size="sm"><Eye size={13} />View job</Button></Link>
                {canWithdraw(a.status) && (
                  <Button variant="danger" size="sm" onClick={() => withdraw(a.jobId)}>Withdraw</Button>
                )}
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
}
