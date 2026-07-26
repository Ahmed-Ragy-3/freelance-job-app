import { createFileRoute } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { Field, Input, Button } from "@/components/ui/FormKit";
import { useTheme } from "@/context/ThemeContext";
import toast from "react-hot-toast";
import { Bell, Lock, Palette, Trash2 } from "lucide-react";

const accountSchema = z.object({
  username: z.string().trim().min(3).max(40),
  email: z.string().trim().email(),
});
const passwordSchema = z.object({
  current: z.string().min(1, "Required"),
  next: z.string().min(8, "Min 8 characters").regex(/[A-Z]/, "Add an uppercase letter").regex(/[0-9]/, "Add a number"),
  confirm: z.string(),
}).refine((d) => d.next === d.confirm, { message: "Passwords don't match", path: ["confirm"] });

export const Route = createFileRoute("/dashboard/settings")({
  head: () => ({ meta: [{ title: "Settings — Workly" }, { name: "description", content: "Manage your account, notifications, and preferences." }] }),
  component: Settings,
});

function Section({ icon: Icon, title, description, children }) {
  return (
    <section className="rounded-2xl border border-border bg-card p-6 shadow-soft">
      <div className="flex items-start gap-3">
        <div className="flex h-10 w-10 shrink-0 items-center justify-center rounded-lg bg-primary/10 text-primary"><Icon size={18} /></div>
        <div>
          <h2 className="text-base font-semibold">{title}</h2>
          {description && <p className="text-sm text-muted-foreground">{description}</p>}
        </div>
      </div>
      <div className="mt-5">{children}</div>
    </section>
  );
}

function Settings() {
  const { user } = useAuth();
  const { theme, toggle } = useTheme();
  const [notif, setNotif] = useState({ email: true, product: true, weekly: false });
  const accountForm = useForm({ resolver: zodResolver(accountSchema), defaultValues: { username: user?.username || "", email: user?.email || "" } });
  const pwForm = useForm({ resolver: zodResolver(passwordSchema), defaultValues: { current: "", next: "", confirm: "" } });

  return (
    <div className="space-y-5">
      <div>
        <h1 className="text-2xl font-bold">Settings</h1>
        <p className="text-sm text-muted-foreground">Manage your account and preferences.</p>
      </div>

      <Section icon={Lock} title="Account" description="Update your account details.">
        <form onSubmit={accountForm.handleSubmit(() => toast.success("Account updated"))} className="grid gap-4 md:grid-cols-2">
          <Field label="Username" error={accountForm.formState.errors.username?.message}><Input {...accountForm.register("username")} /></Field>
          <Field label="Email" error={accountForm.formState.errors.email?.message}><Input type="email" {...accountForm.register("email")} /></Field>
          <div className="md:col-span-2"><Button>Save changes</Button></div>
        </form>
      </Section>

      <Section icon={Lock} title="Password" description="Use a strong, unique password.">
        <form onSubmit={pwForm.handleSubmit(() => { pwForm.reset(); toast.success("Password updated"); })} className="grid gap-4 md:grid-cols-3">
          <Field label="Current" error={pwForm.formState.errors.current?.message}><Input type="password" {...pwForm.register("current")} /></Field>
          <Field label="New password" error={pwForm.formState.errors.next?.message}><Input type="password" {...pwForm.register("next")} /></Field>
          <Field label="Confirm" error={pwForm.formState.errors.confirm?.message}><Input type="password" {...pwForm.register("confirm")} /></Field>
          <div className="md:col-span-3"><Button>Update password</Button></div>
        </form>
      </Section>

      <Section icon={Palette} title="Appearance" description="Switch between light and dark theme.">
        <button onClick={toggle} className="rounded-lg border border-border bg-background px-4 py-2 text-sm font-medium hover:bg-accent">
          Current: {theme === "dark" ? "Dark" : "Light"} — click to toggle
        </button>
      </Section>

      <Section icon={Bell} title="Notifications" description="Choose what you want to be notified about.">
        <div className="space-y-3">
          {[
            { k: "email", label: "Email notifications" },
            { k: "product", label: "Product updates" },
            { k: "weekly", label: "Weekly digest" },
          ].map((n) => (
            <label key={n.k} className="flex items-center justify-between rounded-lg border border-border bg-background px-4 py-3 text-sm">
              <span>{n.label}</span>
              <input type="checkbox" checked={notif[n.k]} onChange={(e) => setNotif((s) => ({ ...s, [n.k]: e.target.checked }))} />
            </label>
          ))}
        </div>
      </Section>

      <Section icon={Trash2} title="Danger zone" description="Permanently delete your account and all its data.">
        <Button variant="danger" onClick={() => toast.error("Contact support to delete your account")}>Delete account</Button>
      </Section>
    </div>
  );
}
