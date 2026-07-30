import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { adminService } from "@/services";
import { Badge } from "@/components/common/Badge";
import { Button } from "@/components/ui/FormKit";
import { formatMoney, timeAgo } from "@/utils/format";
import toast from "react-hot-toast";

export const Route = createFileRoute("/admin/jobs")({ component: AdminJobs });

const variantFor = (s) =>
  s === "Open" ? "success" : s === "Pending" ? "warning" : s === "Rejected" ? "danger" : "default";

function AdminJobs() {
  const [items, setItems] = useState([]);
  const [filter, setFilter] = useState("all");
  const load = () => adminService.jobs().then(setItems);
  useEffect(() => { load(); }, []);

  const approve = async (id) => { await adminService.approveJob(id); toast.success("Job approved"); load(); };
  const reject = async (id) => { await adminService.rejectJob(id); toast.success("Job rejected"); load(); };
  const remove = async (id) => { if (!confirm("Delete this job?")) return; await adminService.deleteJob(id); setItems(items.filter((j) => j.id !== id)); toast.success("Deleted"); };

  const filters = ["all", "Pending", "Open", "Closed", "Rejected"];
  const shown = filter === "all" ? items : items.filter((j) => j.status === filter);
  const pendingCount = items.filter((j) => j.status === "Pending").length;

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <div>
          <h1 className="text-2xl font-bold">Jobs</h1>
          {pendingCount > 0 && <p className="text-sm text-muted-foreground">{pendingCount} pending job{pendingCount === 1 ? "" : "s"} awaiting review.</p>}
        </div>
        <div className="flex flex-wrap gap-1.5">
          {filters.map((f) => (
            <button key={f} onClick={() => setFilter(f)} className={"rounded-full border px-3 py-1 text-xs font-medium " + (filter === f ? "border-primary bg-primary/10 text-primary" : "border-border text-muted-foreground hover:bg-accent")}>
              {f === "all" ? "All" : f}
            </button>
          ))}
        </div>
      </div>
      <div className="overflow-hidden rounded-2xl border border-border bg-card shadow-soft">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="bg-muted/50 text-left text-xs uppercase tracking-wider text-muted-foreground">
              <tr><th className="p-3">Title</th><th className="p-3">Budget</th><th className="p-3">Status</th><th className="p-3">Proposals</th><th className="p-3">Posted</th><th className="p-3 text-right">Actions</th></tr>
            </thead>
            <tbody>
              {shown.map((j) => (
                <tr key={j.id} className="border-t border-border">
                  <td className="p-3 font-medium">{j.title}</td>
                  <td className="p-3">{formatMoney(j.budget)}</td>
                  <td className="p-3"><Badge variant={variantFor(j.status)}>{j.status}</Badge></td>
                  <td className="p-3">{j.proposals}</td>
                  <td className="p-3 text-muted-foreground">{timeAgo(j.createdAt)}</td>
                  <td className="p-3">
                    <div className="flex justify-end gap-2">
                      {j.status === "Pending" && (
                        <>
                          <Button size="sm" onClick={() => approve(j.id)}>Approve</Button>
                          <Button size="sm" variant="outline" onClick={() => reject(j.id)}>Reject</Button>
                        </>
                      )}
                      <Button size="sm" variant="danger" onClick={() => remove(j.id)}>Delete</Button>
                    </div>
                  </td>
                </tr>
              ))}
              {shown.length === 0 && (
                <tr><td colSpan={6} className="p-6 text-center text-sm text-muted-foreground">No jobs match this filter.</td></tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
