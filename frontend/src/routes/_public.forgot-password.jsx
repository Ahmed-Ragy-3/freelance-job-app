import { createFileRoute, Link } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { AuthShell, Field, Input, Button } from "@/components/ui/FormKit";
import { authService } from "@/services";
import toast from "react-hot-toast";

const schema = z.object({ email: z.string().trim().email("Enter a valid email") });

export const Route = createFileRoute("/_public/forgot-password")({
  head: () => ({ meta: [{ title: "Forgot password — Workly" }, { name: "description", content: "Reset your Workly password." }] }),
  component: ForgotPage,
});

function ForgotPage() {
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(schema) });
  const onSubmit = async ({ email }) => {
    await authService.forgotPassword(email);
    toast.success("Password reset link sent");
  };
  return (
    <AuthShell title="Forgot password?" subtitle="We'll email you a link to reset it." footer={<>Remember it? <Link to="/login" className="font-semibold text-primary hover:underline">Sign in</Link></>}>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <Field label="Email" error={errors.email?.message}><Input type="email" placeholder="you@example.com" {...register("email")} /></Field>
        <Button className="w-full" disabled={isSubmitting}>{isSubmitting ? "Sending..." : "Send reset link"}</Button>
      </form>
    </AuthShell>
  );
}
