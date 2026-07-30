import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { jobService, reviewService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Badge } from "@/components/common/Badge";
import { Button } from "@/components/ui/FormKit";
import { EmptyState } from "@/components/common/States";
import { Pagination } from "@/components/common/Pagination";
import { formatMoney, timeAgo } from "@/utils/format";
import { Briefcase, Edit, Trash2, Users, PlusCircle, Star, CheckCircle2 } from "lucide-react";
import { ReviewDialog } from "@/components/reviews/ReviewDialog";
import toast from "react-hot-toast";

export const Route = createFileRoute("/dashboard/jobs/")({ component: ManageJobs });

function ManageJobs() {
  const { user } = useAuth();
  const [items, setItems] = useState([]);
  const [q, setQ] = useState("");
  const [status, setStatus] = useState("");
  const [page, setPage] = useState(1);
  const pageSize = 8;
  const [reviewable, setReviewable] = useState([]);
  const [target, setTarget] = useState(null);

  const load = async () => {
    if (!user) return;
    const [jobs, rev] = await Promise.all([jobService.byClient(user.id), reviewService.reviewableJobs(user.id)]);
    setItems(jobs);
    setReviewable(rev);
  };
  useEffect(() => { load(); /* eslint-disable-next-line react-hooks/exhaustive-deps */ }, [user]);

  const reviewInfoFor = (jobId) => reviewable.find((r) => r.job.id === jobId);
  const markComplete = async (job) => {
    await jobService.complete(job.id);
    toast.success("Job marked as completed");
    load();
  };

  const filtered = items.filter((j) => (q ? j.title.toLowerCase().includes(q.toLowerCase()) : true)).filter((j) => (status ? j.status === status : true));
  const pageItems = filtered.slice((page - 1) * pageSize, page * pageSize);

  const remove = async (id) => { if (!confirm("Delete this job?")) return; await jobService.remove(id); setItems(items.filter((x) => x.id !== id)); toast.success("Deleted"); };

  return (
    <div className="space-y-5">
      <div className="flex flex-wrap items-center justify-between gap-3">
        <h1 className="text-2xl font-bold">Manage jobs</h1>
        <Link to="/dashboard/jobs/new"><Button><PlusCircle size={14} /> Post a job</Button></Link>
      </div>
      <div className="flex flex-wrap gap-3">
        <input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Search title..." className="rounded-lg border border-border bg-card px-3 py-2 text-sm outline-none focus:border-primary" />
        <select value={status} onChange={(e) => setStatus(e.target.value)} className="rounded-lg border border-border bg-card px-3 py-2 text-sm outline-none focus:border-primary">
          <option value="">All status</option><option>Open</option><option>In Progress</option><option>Completed</option><option>Closed</option>
        </select>
      </div>
      {filtered.length === 0 ? <EmptyState icon={Briefcase} title="No jobs yet" action={<Link to="/dashboard/jobs/new" className="rounded-md bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground">Post your first job</Link>} /> : (
        <>
          <div className="overflow-hidden rounded-2xl border border-border bg-card shadow-soft">
            <div className="overflow-x-auto">
              <table className="w-full text-sm">
                <thead className="bg-muted/50 text-left text-xs uppercase tracking-wider text-muted-foreground">
                  <tr><th className="p-3">Title</th><th className="p-3">Budget</th><th className="p-3">Status</th><th className="p-3">Proposals</th><th className="p-3">Posted</th><th className="p-3 text-right">Actions</th></tr>
                </thead>
                <tbody>
                  {pageItems.map((j) => (
                    <tr key={j.id} className="border-t border-border">
                      <td className="p-3 font-medium">{j.title}</td>
                      <td className="p-3">{formatMoney(j.budget)}</td>
                      <td className="p-3"><Badge variant={j.status === "Open" ? "success" : j.status === "In Progress" ? "info" : j.status === "Completed" ? "primary" : "default"}>{j.status}</Badge></td>
                      <td className="p-3">{j.proposals}</td>
                      <td className="p-3 text-muted-foreground">{timeAgo(j.createdAt)}</td>
                      <td className="p-3">
                        <div className="flex justify-end gap-2">
                          {j.status === "In Progress" && (
                            <Button size="sm" variant="outline" onClick={() => markComplete(j)}><CheckCircle2 size={13} />Complete</Button>
                          )}
                          {j.status === "Completed" && reviewInfoFor(j.id) && (
                            reviewInfoFor(j.id).review ? (
                              <Button size="sm" variant="outline" disabled title="You already reviewed this job"><Star size={13} />Reviewed</Button>
                            ) : (
                              <Button size="sm" onClick={() => setTarget(reviewInfoFor(j.id))}><Star size={13} />Leave review</Button>
                            )
                          )}
                          <Link to="/dashboard/jobs/$id/applications" params={{ id: j.id }}><Button size="sm" variant="outline"><Users size={13} />Apps</Button></Link>
                          <Link to="/dashboard/jobs/$id/edit" params={{ id: j.id }}><Button size="sm" variant="outline"><Edit size={13} /></Button></Link>
                          <Button size="sm" variant="danger" onClick={() => remove(j.id)}><Trash2 size={13} /></Button>
                        </div>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
          <Pagination page={page} pageSize={pageSize} total={filtered.length} onPage={setPage} />
        </>
      )}
      <ReviewDialog
        open={!!target}
        onClose={() => setTarget(null)}
        job={target?.job}
        freelancerName={target?.freelancerName}
        clientId={user?.id}
        onSubmitted={load}
      />
    </div>
  );
}
