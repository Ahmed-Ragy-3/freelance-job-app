import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect } from "react";
import { profileService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Field, Input, Textarea, Button } from "@/components/ui/FormKit";
import toast from "react-hot-toast";

const schema = z.object({
  name: z.string().trim().min(2).max(80).optional(),
  title: z.string().trim().max(120).optional(),
  bio: z.string().trim().max(500).optional(),
  averageRate: z.coerce.number().min(0).optional(),
  link: z.string().trim().max(200).optional(),
  companyName: z.string().trim().max(120).optional(),
  companyDetails: z.string().trim().max(500).optional(),
});

export const Route = createFileRoute("/dashboard/profile/edit")({
  component: EditProfile,
});

function EditProfile() {
  const { user, isFreelancer } = useAuth();
  const navigate = useNavigate();
  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(schema) });

  useEffect(() => {
    if (!user) return;
    (isFreelancer ? profileService.getFreelancer(user.id) : profileService.getClient(user.id)).then((p) => reset(p || {}));
  }, [user, isFreelancer, reset]);

  const onSubmit = async (data) => {
    if (isFreelancer) await profileService.updateFreelancer(user.id, data);
    else await profileService.updateClient(user.id, data);
    toast.success("Profile updated");
    navigate({ to: "/dashboard/profile" });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5 rounded-2xl border border-border bg-card p-6 shadow-soft">
      <h1 className="text-2xl font-bold">Edit profile</h1>
      {isFreelancer ? (
        <>
          <div className="grid gap-4 md:grid-cols-2">
            <Field label="Full name" error={errors.name?.message}><Input {...register("name")} /></Field>
            <Field label="Title" error={errors.title?.message}><Input {...register("title")} /></Field>
          </div>
          <Field label="Bio" error={errors.bio?.message}><Textarea rows={4} {...register("bio")} /></Field>
          <div className="grid gap-4 md:grid-cols-2">
            <Field label="Hourly rate (USD)" error={errors.averageRate?.message}><Input type="number" {...register("averageRate")} /></Field>
            <Field label="Portfolio link" error={errors.link?.message}><Input {...register("link")} /></Field>
          </div>
        </>
      ) : (
        <>
          <Field label="Company name" error={errors.companyName?.message}><Input {...register("companyName")} /></Field>
          <Field label="About the company" error={errors.companyDetails?.message}><Textarea rows={4} {...register("companyDetails")} /></Field>
        </>
      )}
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => navigate({ to: "/dashboard/profile" })}>Cancel</Button>
        <Button disabled={isSubmitting}>Save changes</Button>
      </div>
    </form>
  );
}
