import { createFileRoute, useNavigate, Link } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect, useState } from "react";
import { jobService, applicationService } from "@/services";
import { Field, Input, Textarea, Button } from "@/components/ui/FormKit";
import { Badge } from "@/components/common/Badge";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { Skeleton, ErrorState } from "@/components/common/States";
import { formatMoney, daysUntil } from "@/utils/format";
import { UploadCloud, X, CheckCircle2, Lock } from "lucide-react";
import { useAuth } from "@/context/AuthContext";
import { useApplicationForJob } from "@/hooks/useApplications";
import toast from "react-hot-toast";

const schema = z.object({
  coverLetter: z
    .string()
    .trim()
    .min(50, "Cover letter should be at least 50 characters")
    .max(2000, "Cover letter must be under 2000 characters"),
  bid: z.coerce.number().int("Bid must be a whole number").positive("Bid must be greater than 0"),
  timeline: z.coerce.number().int("Timeline must be a whole number of days").positive("Timeline must be at least 1 day").max(365, "Timeline cannot exceed 365 days"),
});

export const Route = createFileRoute("/_public/jobs/$id_/apply")({
  head: () => ({
    meta: [
      { title: "Apply — Workly" },
      { name: "description", content: "Submit, edit, or withdraw your job application." },
    ],
  }),
  component: ApplyPage,
});

function ApplyPage() {
  const { id } = Route.useParams();
  const { user, isAuthenticated, isFreelancer, loading: authLoading } = useAuth();
  const navigate = useNavigate();

  const [job, setJob] = useState(null);
  const [jobLoading, setJobLoading] = useState(true);
  const [jobError, setJobError] = useState(null);
  const [files, setFiles] = useState([]);
  const [submitError, setSubmitError] = useState(null);

  const { data: existing, loading: loadingApp, refetch } = useApplicationForJob(user?.id, id);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm({
    resolver: zodResolver(schema),
    defaultValues: { coverLetter: "", bid: undefined, timeline: 14 },
  });

  useEffect(() => {
    let alive = true;
    setJobLoading(true);
    jobService
      .get(id)
      .then((j) => { if (alive) { setJob(j); setJobLoading(false); } })
      .catch((e) => { if (alive) { setJobError(e); setJobLoading(false); } });
    return () => { alive = false; };
  }, [id]);

  useEffect(() => {
    if (existing) {
      reset({
        coverLetter: existing.coverLetter,
        bid: existing.bid,
        timeline: typeof existing.timeline === "number" ? existing.timeline : parseInt(existing.timeline, 10) || 14,
      });
    }
  }, [existing, reset]);

  // Auth + role gate
  if (authLoading) return <div className="mx-auto max-w-4xl px-4 py-10"><Skeleton className="h-80" /></div>;

  if (!isAuthenticated) {
    return (
      <div className="mx-auto max-w-lg px-4 py-16 text-center">
        <Lock size={32} className="mx-auto text-muted-foreground" />
        <h1 className="mt-4 text-2xl font-bold">Sign in to apply</h1>
        <p className="mt-2 text-sm text-muted-foreground">You need a freelancer account to apply for jobs.</p>
        <div className="mt-6 flex justify-center gap-2">
          <Button onClick={() => navigate({ to: "/login" })}>Sign in</Button>
          <Button variant="outline" onClick={() => navigate({ to: "/register" })}>Create account</Button>
        </div>
      </div>
    );
  }

  if (!isFreelancer) {
    return (
      <div className="mx-auto max-w-lg px-4 py-16 text-center">
        <Lock size={32} className="mx-auto text-muted-foreground" />
        <h1 className="mt-4 text-2xl font-bold">Freelancers only</h1>
        <p className="mt-2 text-sm text-muted-foreground">Only accounts with the Freelancer role can submit applications.</p>
        <div className="mt-6">
          <Link to="/jobs/$id" params={{ id }} className="text-sm font-semibold text-primary hover:underline">← Back to job</Link>
        </div>
      </div>
    );
  }

  const onSubmit = async (data) => {
    setSubmitError(null);
    try {
      if (existing) {
        if (existing.status !== "Submitted" && existing.status !== "Pending") {
          toast.error("This application can no longer be edited");
          return;
        }
        await applicationService.update(existing.id, {
          coverLetter: data.coverLetter,
          bid: parseInt(data.bid, 10),
          timeline: parseInt(data.timeline, 10),
        });
        toast.success("Application updated");
      } else {
        await applicationService.create({
          jobId: id,
          freelancerId: user.id,
          coverLetter: data.coverLetter,
          bid: parseInt(data.bid, 10),
          timeline: parseInt(data.timeline, 10),
        });
        toast.success("Application submitted");
      }
      navigate({ to: "/dashboard/applications" });
    } catch (err) {
      const msg = err?.message || "Something went wrong. Please try again.";
      setSubmitError(msg);
      toast.error(msg);
    }
  };

  const onWithdraw = async () => {
    if (!existing) return;
    if (!window.confirm("Withdraw your application? This cannot be undone.")) return;
    try {
      await applicationService.withdraw(existing.id);
      toast.success("Application withdrawn");
      refetch();
      reset({ coverLetter: "", bid: undefined, timeline: 14 });
    } catch (err) {
      toast.error(err?.message || "Could not withdraw application");
    }
  };

  const addFiles = (e) => {
    const list = Array.from(e.target.files || []);
    setFiles((prev) => [...prev, ...list.map((f) => ({ name: f.name, size: f.size }))]);
  };
  const removeFile = (i) => setFiles((prev) => prev.filter((_, idx) => idx !== i));

  const locked = existing && existing.status !== "Submitted" && existing.status !== "Pending";

  return (
    <div className="mx-auto max-w-4xl px-4 py-8 sm:px-6">
      <Breadcrumbs />
      <div className="mt-4 flex flex-wrap items-start justify-between gap-3">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">{existing ? "Your application" : "Apply for this job"}</h1>
          {existing && (
            <p className="mt-1 inline-flex items-center gap-2 text-sm text-muted-foreground">
              <CheckCircle2 size={14} className="text-primary" /> Submitted — status:{" "}
              <Badge
                variant={
                  existing.status === "Accepted"
                    ? "success"
                    : existing.status === "Rejected"
                    ? "danger"
                    : existing.status === "Shortlisted"
                    ? "info"
                    : "warning"
                }
              >
                {existing.status}
              </Badge>
            </p>
          )}
        </div>
        <Link to="/jobs/$id" params={{ id }} className="text-sm text-muted-foreground hover:text-foreground">
          ← Back to job
        </Link>
      </div>

      {jobLoading ? (
        <Skeleton className="mt-4 h-20" />
      ) : jobError ? (
        <div className="mt-4"><ErrorState description={jobError.message} /></div>
      ) : job ? (
        <div className="mt-4 rounded-2xl border border-border bg-card p-5">
          <div className="flex flex-wrap items-center gap-2">
            <Badge variant="success">{job.status}</Badge>
            <Badge variant="outline">Budget: {formatMoney(job.budget)}</Badge>
            <Badge variant="outline">{daysUntil(job.deadline)} days left</Badge>
          </div>
          <h2 className="mt-3 font-semibold">{job.title}</h2>
        </div>
      ) : null}

      {loadingApp ? (
        <Skeleton className="mt-6 h-80" />
      ) : (
        <form onSubmit={handleSubmit(onSubmit)} className="mt-6 space-y-5 rounded-2xl border border-border bg-card p-6 shadow-soft">
          <Field label="Cover letter" error={errors.coverLetter?.message} hint="Explain why you're a great fit (min 50 characters).">
            <Textarea rows={8} disabled={locked} {...register("coverLetter")} />
          </Field>
          <div className="grid gap-4 md:grid-cols-2">
            <Field label="Your bid (USD)" error={errors.bid?.message}>
              <Input type="number" min="1" step="1" disabled={locked} {...register("bid")} />
            </Field>
            <Field label="Timeline (days)" error={errors.timeline?.message} hint="Estimated number of days to complete.">
              <Input type="number" min="1" step="1" disabled={locked} {...register("timeline")} />
            </Field>
          </div>
          <Field label="Portfolio / attachments" hint="Optional. PDF, images, or ZIP up to 10MB.">
            <label className="flex cursor-pointer items-center justify-center gap-2 rounded-lg border border-dashed border-border bg-background px-4 py-8 text-sm text-muted-foreground hover:border-primary hover:text-foreground">
              <UploadCloud size={18} />
              <span>Click to upload files</span>
              <input type="file" multiple className="hidden" disabled={locked} onChange={addFiles} />
            </label>
            {files.length > 0 && (
              <ul className="mt-3 space-y-1">
                {files.map((f, i) => (
                  <li key={i} className="flex items-center justify-between rounded-md border border-border bg-background px-3 py-2 text-xs">
                    <span className="truncate">{f.name}</span>
                    <button type="button" onClick={() => removeFile(i)} className="text-muted-foreground hover:text-destructive">
                      <X size={14} />
                    </button>
                  </li>
                ))}
              </ul>
            )}
          </Field>

          {submitError && (
            <div className="rounded-lg border border-destructive/30 bg-destructive/10 px-3 py-2 text-sm text-destructive">
              {submitError}
            </div>
          )}

          <div className="flex flex-wrap justify-between gap-2 border-t border-border pt-4">
            <div>
              {existing && !locked && (
                <Button type="button" variant="danger" onClick={onWithdraw} disabled={isSubmitting}>
                  Withdraw application
                </Button>
              )}
            </div>
            <div className="flex gap-2">
              <Button type="button" variant="outline" onClick={() => navigate({ to: "/jobs/$id", params: { id } })}>
                Cancel
              </Button>
              <Button disabled={isSubmitting || locked}>
                {isSubmitting ? "Submitting…" : existing ? "Save changes" : "Submit application"}
              </Button>
            </div>
          </div>
          {locked && (
            <p className="text-xs text-muted-foreground">
              This application is {existing.status.toLowerCase()} and can no longer be edited.
            </p>
          )}
        </form>
      )}
    </div>
  );
}
