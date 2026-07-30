import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect, useState } from "react";
import { jobService, staticData } from "@/services";
import { Field, Input, Textarea, Button, MultiSelect } from "@/components/ui/FormKit";
import { useAuth } from "@/context/AuthContext";
import toast from "react-hot-toast";

const schema = z.object({
  title: z.string().trim().min(6, "Title too short").max(120),
  description: z.string().trim().min(30, "Add more detail").max(4000),
  budget: z.coerce.number().positive(),
  deadline: z.string().min(1, "Deadline required"),
});

export const Route = createFileRoute("/dashboard/jobs/new")({ component: NewJob });

function NewJob() {
  const { user } = useAuth();
  const navigate = useNavigate();
  const [cats, setCats] = useState([]);
  const [allSkills, setAllSkills] = useState([]);
  const [allTags, setAllTags] = useState([]);
  const [categoryIds, setCategoryIds] = useState([]);
  const [selectedSkills, setSelectedSkills] = useState([]);
  const [selectedTags, setSelectedTags] = useState([]);
  const [touched, setTouched] = useState(false);
  useEffect(() => {
    staticData.categories().then(setCats);
    staticData.skills().then(setAllSkills);
    staticData.tags().then(setAllTags);
  }, []);

  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(schema) });
  const onSubmit = async (data) => {
    setTouched(true);
    if (!categoryIds.length) { toast.error("Select at least one category"); return; }
    await jobService.create({
      ...data,
      clientId: user.id,
      categoryId: categoryIds[0],
      categoryIds,
      categoryNames: cats.filter((c) => categoryIds.includes(c.id)).map((c) => c.name),
      requiredSkills: selectedSkills,
      tags: selectedTags,
    });
    toast.success("Job submitted for admin review");
    navigate({ to: "/dashboard/jobs" });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5 rounded-2xl border border-border bg-card p-6 shadow-soft">
      <h1 className="text-2xl font-bold">Post a job</h1>
      <Field label="Title" error={errors.title?.message}><Input placeholder="Build a React dashboard" {...register("title")} /></Field>
      <Field label="Description" error={errors.description?.message}><Textarea rows={6} {...register("description")} /></Field>
      <div className="grid gap-4 md:grid-cols-2">
        <Field label="Budget (USD)" error={errors.budget?.message}><Input type="number" {...register("budget")} /></Field>
        <Field label="Deadline" error={errors.deadline?.message}><Input type="date" {...register("deadline")} /></Field>
      </div>
      <Field label="Categories" error={touched && !categoryIds.length ? "Select at least one category" : undefined} hint="Select one or more">
        <MultiSelect options={cats} value={categoryIds} onChange={setCategoryIds} getKey={(c) => c.id} getLabel={(c) => c.name} />
      </Field>
      <Field label="Tags" hint="Select one or more">
        <MultiSelect options={allTags} value={selectedTags} onChange={setSelectedTags} getKey={(t) => t} getLabel={(t) => t} />
      </Field>
      <Field label="Required skills" hint="Select one or more">
        <MultiSelect options={allSkills} value={selectedSkills} onChange={setSelectedSkills} getKey={(s) => s} getLabel={(s) => s} />
      </Field>
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => navigate({ to: "/dashboard/jobs" })}>Cancel</Button>
        <Button disabled={isSubmitting}>Post job</Button>
      </div>
    </form>
  );
}
