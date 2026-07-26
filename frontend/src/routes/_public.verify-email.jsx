import { createFileRoute, Link } from "@tanstack/react-router";
import { AuthShell, Button } from "@/components/ui/FormKit";
import { CheckCircle2 } from "lucide-react";

export const Route = createFileRoute("/_public/verify-email")({
  head: () => ({ meta: [{ title: "Verify email — Workly" }, { name: "description", content: "Verify your Workly email address." }] }),
  component: () => (
    <AuthShell title="Check your inbox" subtitle="We sent a verification link to your email." footer={<Link to="/login" className="font-semibold text-primary hover:underline">Back to sign in</Link>}>
      <div className="flex flex-col items-center rounded-xl border border-border bg-card p-8 text-center">
        <div className="flex h-12 w-12 items-center justify-center rounded-full bg-primary/10 text-primary"><CheckCircle2 size={22} /></div>
        <p className="mt-4 text-sm text-muted-foreground">Click the link in the email to activate your account. Didn't get it?</p>
        <Button variant="outline" className="mt-4">Resend email</Button>
      </div>
    </AuthShell>
  ),
});
