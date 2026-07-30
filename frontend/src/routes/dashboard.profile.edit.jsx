import { createFileRoute, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useEffect, useState } from "react";
import { profileService, staticData } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Field, Input, Textarea, Button, MultiSelect } from "@/components/ui/FormKit";
import toast from "react-hot-toast";

const freelancerSchema = z.object({
  name: z.string().trim().min(3, "Name must be at least 3 characters").max(80),
  bio: z.string().trim().max(500).optional().or(z.literal("")),
  link: z.string().trim().max(200).optional().or(z.literal("")),
});

const clientSchema = z.object({
  companyName: z.string().trim().min(2, "Company name is required").max(120),
  companyDetails: z.string().trim().max(500).optional().or(z.literal("")),
});

export const Route = createFileRoute("/dashboard/profile/edit")({
  component: EditProfile,
});

function EditProfile() {
  const { user, isFreelancer, isClient, refreshUser } = useAuth();
  const navigate = useNavigate();

  if (!user) {
    return <p className="text-sm text-muted-foreground">Loading profile…</p>;
  }

  if (isFreelancer) {
    return <FreelancerEditForm user={user} refreshUser={refreshUser} navigate={navigate} />;
  }

  if (isClient) {
    return <ClientEditForm user={user} navigate={navigate} />;
  }

  return (
    <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
      <h1 className="text-2xl font-bold">Edit profile</h1>
      <p className="mt-2 text-sm text-muted-foreground">Profile editing is not available for your account type.</p>
      <div className="mt-4">
        <Button type="button" variant="outline" onClick={() => navigate({ to: "/dashboard/profile" })}>
          Back to profile
        </Button>
      </div>
    </div>
  );
}

function FreelancerEditForm({ user, refreshUser, navigate }) {
  const [skillOptions, setSkillOptions] = useState([]);
  const [selectedSkills, setSelectedSkills] = useState([]);
  const [loading, setLoading] = useState(true);
  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm({
    resolver: zodResolver(freelancerSchema),
    defaultValues: { name: "", bio: "", link: "" },
  });

  useEffect(() => {
    let cancelled = false;
    setLoading(true);

    Promise.all([
      staticData.skills().catch(() => []),
      profileService.getFreelancer(user.id).catch(() => null),
    ])
      .then(([skills, profile]) => {
        if (cancelled) return;
        setSkillOptions(skills);
        reset({
          name: profile?.name || profile?.username || user.username || "",
          bio: profile?.bio || "",
          link: profile?.link || "",
        });
        const ids = (profile?.skills || [])
          .map((s) => (typeof s === "object" ? s.skillId ?? s.id : Number(s)))
          .filter((id) => id != null && !Number.isNaN(Number(id)))
          .map(Number);
        setSelectedSkills(ids);
      })
      .catch((err) => {
        if (cancelled) return;
        toast.error(err?.message || "Failed to load profile");
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => { cancelled = true; };
  }, [user, reset]);

  const onSubmit = async (data) => {
    try {
      await profileService.updateUser({
        username: data.name,
        imageUrl: user.imageUrl,
      });
      await profileService.updateFreelancer(user.id, {
        bio: data.bio || "",
        link: data.link || "",
        skills: selectedSkills.map((id) => ({ skillId: id, experienceLevel: 1 })),
      });
      await refreshUser();
      toast.success("Profile updated");
      navigate({ to: "/dashboard/profile" });
    } catch (err) {
      toast.error(err?.message || "Failed to update profile");
    }
  };

  if (loading) {
    return <p className="text-sm text-muted-foreground">Loading profile…</p>;
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5 rounded-2xl border border-border bg-card p-6 shadow-soft">
      <h1 className="text-2xl font-bold">Edit profile</h1>
      <Field label="Display name" error={errors.name?.message}>
        <Input {...register("name")} autoComplete="name" />
      </Field>
      <Field label="Bio" error={errors.bio?.message}>
        <Textarea rows={4} placeholder="Tell clients about your experience…" {...register("bio")} />
      </Field>
      <Field label="Portfolio link" error={errors.link?.message}>
        <Input type="url" placeholder="https://…" {...register("link")} />
      </Field>
      <Field label="Skills" hint="Select the skills you offer">
        <MultiSelect
          options={skillOptions}
          value={selectedSkills}
          onChange={setSelectedSkills}
          getKey={(s) => s.id}
          getLabel={(s) => s.name}
          empty="No skills available"
        />
      </Field>
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => navigate({ to: "/dashboard/profile" })}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Saving…" : "Save changes"}
        </Button>
      </div>
    </form>
  );
}

function ClientEditForm({ user, navigate }) {
  const [loading, setLoading] = useState(true);
  const { register, handleSubmit, reset, formState: { errors, isSubmitting } } = useForm({
    resolver: zodResolver(clientSchema),
    defaultValues: { companyName: "", companyDetails: "" },
  });

  useEffect(() => {
    let cancelled = false;
    setLoading(true);

    profileService.getClient(user.id)
      .then((profile) => {
        if (cancelled) return;
        reset({
          companyName: profile?.companyName || "",
          companyDetails: profile?.companyDetails || "",
        });
      })
      .catch((err) => {
        if (cancelled) return;
        toast.error(err?.message || "Failed to load profile");
      })
      .finally(() => {
        if (!cancelled) setLoading(false);
      });

    return () => { cancelled = true; };
  }, [user, reset]);

  const onSubmit = async (data) => {
    try {
      await profileService.updateClient(user.id, {
        companyName: data.companyName,
        companyDetails: data.companyDetails || "",
        logo: user.imageUrl,
      });
      toast.success("Profile updated");
      navigate({ to: "/dashboard/profile" });
    } catch (err) {
      toast.error(err?.message || "Failed to update profile");
    }
  };

  if (loading) {
    return <p className="text-sm text-muted-foreground">Loading profile…</p>;
  }

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="space-y-5 rounded-2xl border border-border bg-card p-6 shadow-soft">
      <h1 className="text-2xl font-bold">Edit profile</h1>
      <Field label="Company name" error={errors.companyName?.message}>
        <Input {...register("companyName")} autoComplete="organization" />
      </Field>
      <Field label="About the company" error={errors.companyDetails?.message}>
        <Textarea rows={4} placeholder="Describe your company…" {...register("companyDetails")} />
      </Field>
      <div className="flex justify-end gap-2">
        <Button type="button" variant="outline" onClick={() => navigate({ to: "/dashboard/profile" })}>
          Cancel
        </Button>
        <Button type="submit" disabled={isSubmitting}>
          {isSubmitting ? "Saving…" : "Save changes"}
        </Button>
      </div>
    </form>
  );
}
