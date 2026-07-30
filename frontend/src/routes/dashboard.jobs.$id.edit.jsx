import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect, useState } from "react";
import { jobService, staticData } from "@/services";
import { Field, Input, Textarea, Select, Button, MultiSelect } from "@/components/ui/FormKit";
import toast from "react-hot-toast";

const schema = z.object({
  title: z.string().trim().min(6).max(120),
  description: z.string().trim().min(30).max(4000),
  budget: z.coerce.number().positive(),
  deadline: z.string(),
  status: z.enum(["Open","In Progress","Closed"]),
});

export const Route = createFileRoute("/dashboard/jobs/$id/edit")({ component: EditJob });

function EditJob() {
  const { id } = Route.useParams();
  const navigate = useNavigate();
  const [cats, setCats] = useState([]);
  const [allSkills, setAllSkills] = useState([]);
  const [allTags, setAllTags] = useState([]);
  const [categoryIds, setCategoryIds] = useState([]);
  const [selectedSkills, setSelectedSkills] = useState([]);
  const [selectedTags, setSelectedTags] = useState([]);
  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(schema) });

  useEffect(() => {
    staticData.categories().then(setCats);
    staticData.skills().then(setAllSkills);
    staticData.tags().then(setAllTags);
  }, []);

  useEffect(() => {
    jobService.get(id).then((j) => {
      if (!j) return;
      reset({ ...j, deadline: j?.deadline?.slice(0, 10) });
      setCategoryIds(j.categoryIds || (j.categoryId ? [j.categoryId] : []));
      setSelectedSkills(j.requiredSkills || []);
      setSelectedTags(j.tags || []);
    });
  }, [id, reset]);

  const onSubmit = async (data) => {
    if (!categoryIds.length) { toast.error("Select at least one category"); return; }
    await jobService.update(id, {
      ...data,
      categoryId: categoryIds[0],
      categoryIds,
      categoryNames: cats.filter((c) => categoryIds.includes(c.id)).map((c) => c.name),
      requiredSkills: selectedSkills,
      tags: selectedTags,
    });
    toast.success("Job updated");
    navigate({ to: "/dashboard/jobs" });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5 rounded-2xl border border-border bg-card p-6 shadow-soft">
      <h1 className="text-2xl font-bold">Edit job</h1>
      <Field label="Title" error={errors.title?.message}><Input {...register("title")} /></Field>
      <Field label="Description" error={errors.description?.message}><Textarea rows={6} {...register("description")} /></Field>
      <div className="grid gap-4 md:grid-cols-3">
        <Field label="Budget" error={errors.budget?.message}><Input type="number" {...register("budget")} /></Field>
        <Field label="Deadline" error={errors.deadline?.message}><Input type="date" {...register("deadline")} /></Field>
        <Field label="Status" error={errors.status?.message}><Select {...register("status")}><option>Open</option><option>In Progress</option><option>Closed</option></Select></Field>
      </div>
      <Field label="Categories" hint="Select one or more">
        <MultiSelect options={cats} value={categoryIds} onChange={setCategoryIds} getKey={(c) => c.id} getLabel={(c) => c.name} />
      </Field>
      <Field label="Tags" hint="Select one or more">
        <MultiSelect options={allTags} value={selectedTags} onChange={setSelectedTags} getKey={(t) => (typeof t === "object" ? t.id : t)} getLabel={(t) => (typeof t === "object" ? t.name : t)} />
      </Field>
      <Field label="Required skills" hint="Select one or more">
        <MultiSelect options={allSkills} value={selectedSkills} onChange={setSelectedSkills} getKey={(s) => (typeof s === "object" ? s.id : s)} getLabel={(s) => (typeof s === "object" ? s.name : s)} />
      </Field>
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => navigate({ to: "/dashboard/jobs" })}>Cancel</Button>
        <Button disabled={isSubmitting}>Save</Button>
      </div>
    </form>
  );
}
