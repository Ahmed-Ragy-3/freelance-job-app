import { createFileRoute, Link, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { AuthShell, Field, Input, Button } from "@/components/ui/FormKit";
import toast from "react-hot-toast";
import { authService } from "@/services";

const schema = z.object({
  password: z.string().min(8, "Min 8 characters").regex(/[A-Z]/, "Include an uppercase letter").regex(/[0-9]/, "Include a number"),
  confirm: z.string(),
}).refine((d) => d.password === d.confirm, { message: "Passwords must match", path: ["confirm"] });

export const Route = createFileRoute("/_public/reset-password")({
  head: () => ({ meta: [{ title: "Reset password — Workly" }, { name: "description", content: "Choose a new password." }] }),
  component: ResetPage,
});

function ResetPage() {
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({ resolver: zodResolver(schema) });
  const navigate = useNavigate();
  const onSubmit = async (d) => { await authService.resetPassword({ token: "demo", password: d.password }); toast.success("Password updated"); navigate({ to: "/login" }); };
  return (
    <AuthShell title="Set a new password" footer={<Link to="/login" className="font-semibold text-primary hover:underline">Back to sign in</Link>}>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <Field label="New password" error={errors.password?.message}><Input type="password" {...register("password")} /></Field>
        <Field label="Confirm password" error={errors.confirm?.message}><Input type="password" {...register("confirm")} /></Field>
        <Button className="w-full" disabled={isSubmitting}>{isSubmitting ? "Updating..." : "Update password"}</Button>
      </form>
    </AuthShell>
  );
}
