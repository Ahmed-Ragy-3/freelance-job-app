import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect, useState } from "react";
import { jobService, staticData } from "@/services";
import { Field, Input, Textarea, Select, Button } from "@/components/ui/FormKit";
import { useAuth } from "@/context/AuthContext";
import toast from "react-hot-toast";

const schema = z.object({
  title: z.string().trim().min(6, "Title too short").max(120),
  description: z.string().trim().min(30, "Add more detail").max(4000),
  budget: z.coerce.number().positive(),
  deadline: z.string().min(1, "Deadline required"),
  categoryId: z.string().min(1, "Category required"),
});

export const Route = createFileRoute("/dashboard/jobs/new")({ component: NewJob });

function NewJob() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [cats, setCats] = useState([]);
  const [selectedSkills, setSelectedSkills] = useState([]);
  const [allSkills, setAllSkills] = useState([]);
  useEffect(() => { staticData.categories().then(setCats); staticData.skills().then(setAllSkills); }, []);

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(schema) });
  const onSubmit = async (data) => {
    await jobService.create({ ...data, clientId: user.id, requiredSkills: selectedSkills, tags: ["Fixed price"] });
    toast.success("Job submitted for admin review");
    navigate({ to: "/dashboard/jobs" });
  };
  const toggle = (s) => setSelectedSkills((cur) => cur.includes(s) ? cur.filter((x) => x !== s) : [...cur, s]);

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5 rounded-2xl border border-border bg-card p-6 shadow-soft">
      <h1 className="text-2xl font-bold">Post a job</h1>
      <Field label="Title" error={errors.title?.message}><Input placeholder="Build a React dashboard" {...register("title")} /></Field>
      <Field label="Description" error={errors.description?.message}><Textarea rows={6} {...register("description")} /></Field>
      <div className="grid gap-4 md:grid-cols-3">
        <Field label="Budget (USD)" error={errors.budget?.message}><Input type="number" {...register("budget")} /></Field>
        <Field label="Deadline" error={errors.deadline?.message}><Input type="date" {...register("deadline")} /></Field>
        <Field label="Category" error={errors.categoryId?.message}>
          <Select {...register("categoryId")}>
            <option value="">Select…</option>
            {cats.map((c) => <option key={c.id} value={c.id}>{c.name}</option>)}
          </Select>
        </Field>
      </div>
      <div>
        <label className="mb-1.5 block text-sm font-medium">Required skills</label>
        <div className="flex flex-wrap gap-1.5">
          {allSkills.map((s) => <button type="button" key={s} onClick={() => toggle(s)} className={"rounded-full border px-2.5 py-1 text-xs " + (selectedSkills.includes(s) ? "border-primary bg-primary/10 text-primary" : "border-border")}>{s}</button>)}
        </div>
      </div>
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => navigate({ to: "/dashboard/jobs" })}>Cancel</Button>
        <Button disabled={isSubmitting}>Post job</Button>
      </div>
    </form>
  );
}
