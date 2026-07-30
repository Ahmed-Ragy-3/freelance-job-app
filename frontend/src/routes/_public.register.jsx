import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import toast from "react-hot-toast";
import { useState } from "react";
import { AuthShell, Field, Input, Textarea, Button } from "@/components/ui/FormKit";
import { useAuth } from "@/context/AuthContext";
import { Briefcase, User as UserIcon } from "lucide-react";

const baseSchema = {
  username: z.string().trim().min(3, "Min 3 characters").max(40),
  email: z.string().trim().email("Enter a valid email"),
  password: z.string().min(8, "Min 8 characters").regex(/[A-Z]/, "Include an uppercase letter").regex(/[0-9]/, "Include a number"),
};
const freelancerSchema = z.object({ ...baseSchema, role: z.literal("Freelancer"), bio: z.string().trim().min(20, "Tell us more (20+ chars)").max(400) });
const clientSchema = z.object({ ...baseSchema, role: z.literal("Client"), companyName: z.string().trim().min(2, "Company name required").max(80) });

export const Route = createFileRoute("/_public/register")({
  head: () => ({ meta: [{ title: "Create account — Workly" }, { name: "description", content: "Join Workly as a freelancer or client." }] }),
  component: RegisterPage,
});

function RegisterPage() {
  const [role, setRole] = useState("Freelancer");
  const { register: signUp } = useAuth();
  const navigate = useNavigate();
  const schema = role === "Freelancer" ? freelancerSchema : clientSchema;
  const { register, handleSubmit, formState: { errors, isSubmitting }, reset } = useForm({ resolver: zodResolver(schema), defaultValues: { role } });

  const onSubmit = async (data) => {
    try {
      const u = await signUp(data);
      toast.success(`Welcome, ${u.username}!`);
      navigate({ to: "/dashboard" });
    } catch (e) { toast.error(e.message || "Registration failed"); }
  };

  const switchRole = (r) => { setRole(r); reset({ role: r }); };

  return (
    <AuthShell title="Create your account" subtitle="Join thousands of teams and freelancers." footer={<>Already have an account? <Link to="/login" className="font-semibold text-primary hover:underline">Sign in</Link></>}>
      <div className="mb-6 grid grid-cols-2 gap-3">
        {[
          { r: "Freelancer", icon: UserIcon, desc: "Find work" },
          { r: "Client", icon: Briefcase, desc: "Hire talent" },
        ].map(({ r, icon: Icon, desc }) => (
          <button key={r} type="button" onClick={() => switchRole(r)}
            className={`rounded-xl border p-4 text-left transition-colors ${role === r ? "border-primary bg-primary/5" : "border-border hover:border-primary/50"}`}>
            <Icon size={18} className={role === r ? "text-primary" : "text-muted-foreground"} />
            <div className="mt-2 text-sm font-semibold">{r}</div>
            <div className="text-xs text-muted-foreground">{desc}</div>
          </button>
        ))}
      </div>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <input type="hidden" value={role} {...register("role")} />
        <Field label="Username" error={errors.username?.message}><Input placeholder="janedoe" {...register("username")} /></Field>
        <Field label="Email" error={errors.email?.message}><Input type="email" placeholder="you@example.com" {...register("email")} /></Field>
        <Field label="Password" error={errors.password?.message} hint="Min 8 chars, one uppercase and one number"><Input type="password" placeholder="••••••••" {...register("password")} /></Field>
        {role === "Freelancer" ? (
          <Field label="Short bio" error={errors.bio?.message}><Textarea rows={3} placeholder="Full-stack engineer with 5 years experience..." {...register("bio")} /></Field>
        ) : (
          <Field label="Company name" error={errors.companyName?.message}><Input placeholder="Acme Inc." {...register("companyName")} /></Field>
        )}
        <Button className="w-full" disabled={isSubmitting}>{isSubmitting ? "Creating..." : "Create account"}</Button>
      </form>
    </AuthShell>
  );
}
