import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import toast from "react-hot-toast";
import { AuthShell, Field, Input, Button } from "@/components/ui/FormKit";
import { useAuth } from "@/context/AuthContext";

const schema = z.object({
  email: z.string().trim().email("Enter a valid email"),
  password: z.string().min(6, "Password must be at least 6 characters"),
});

export const Route = createFileRoute("/_public/login")({
  head: () => ({ meta: [{ title: "Sign in — Workly" }, { name: "description", content: "Sign in to your Workly account." }] }),
  component: LoginPage,
});

function LoginPage() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(schema), defaultValues: { email: "", password: "" } });

  const onSubmit = async (data) => {
    try {
      const u = await login(data);
      toast.success(`Welcome back, ${u.username}`);
      navigate({ to: u.role === "Admin" ? "/admin" : "/dashboard" });
    } catch (e) {
      toast.error(e.message || "Sign in failed");
    }
  };

  return (
    <AuthShell title="Welcome back" subtitle="Sign in to continue to your dashboard." footer={<>Don't have an account? <Link to="/register" className="font-semibold text-primary hover:underline">Create one</Link></>}>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <Field label="Email" error={errors.email?.message}><Input type="email" placeholder="you@example.com" {...register("email")} /></Field>
        <Field label="Password" error={errors.password?.message}><Input type="password" placeholder="••••••••" {...register("password")} /></Field>
        <div className="flex items-center text-sm">
          <label className="inline-flex items-center gap-2 text-muted-foreground"><input type="checkbox" className="rounded" /> Remember me</label>
        </div>
        <Button className="w-full" disabled={isSubmitting}>{isSubmitting ? "Signing in..." : "Sign in"}</Button>
        <p className="rounded-md bg-muted p-3 text-xs text-muted-foreground">
          <strong>Demo:</strong> alex@studio.com / password123 (Freelancer), hello@acme.co / password123 (Client), admin@marketplace.io / password123 (Admin)
        </p>
      </form>
    </AuthShell>
  );
}
