import { createFileRoute } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import toast from "react-hot-toast";
import { Field, Input, Textarea, Button } from "@/components/ui/FormKit";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { Mail, Phone, MapPin } from "lucide-react";

const schema = z.object({
  name: z.string().trim().min(2, "Name required").max(80),
  email: z.string().trim().email("Enter a valid email").max(200),
  message: z.string().trim().min(10, "Message too short").max(2000),
});

export const Route = createFileRoute("/_public/contact")({
  head: () => ({ meta: [{ title: "Contact — Workly" }, { name: "description", content: "Get in touch with the Workly team." }] }),
  component: Contact,
});

function Contact() {
  const { register, handleSubmit, formState: { errors, isSubmitting }, reset } = useForm({ resolver: zodResolver(schema) });
  const onSubmit = async () => { await new Promise((r) => setTimeout(r, 400)); toast.success("Message sent — we'll reply soon"); reset(); };
  return (
    <div className="mx-auto max-w-5xl px-4 py-16 sm:px-6">
      <Breadcrumbs />
      <h1 className="mt-4 text-4xl font-bold tracking-tight">Contact us</h1>
      <p className="mt-3 max-w-2xl text-muted-foreground">Questions, partnerships, or press? Send us a note.</p>
      <div className="mt-10 grid gap-8 md:grid-cols-3">
        <div className="space-y-4">
          {[
            { icon: Mail, l: "Email", v: "hello@workly.io" },
            { icon: Phone, l: "Phone", v: "+1 (555) 010-1234" },
            { icon: MapPin, l: "Office", v: "San Francisco, CA" },
          ].map((c) => (
            <div key={c.l} className="rounded-xl border border-border bg-card p-4">
              <c.icon size={18} className="text-primary" />
              <div className="mt-2 text-xs text-muted-foreground">{c.l}</div>
              <div className="font-semibold">{c.v}</div>
            </div>
          ))}
        </div>
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4 rounded-2xl border border-border bg-card p-6 md:col-span-2">
          <Field label="Name" error={errors.name?.message}><Input {...register("name")} /></Field>
          <Field label="Email" error={errors.email?.message}><Input type="email" {...register("email")} /></Field>
          <Field label="Message" error={errors.message?.message}><Textarea rows={5} {...register("message")} /></Field>
          <Button disabled={isSubmitting}>{isSubmitting ? "Sending..." : "Send message"}</Button>
        </form>
      </div>
    </div>
  );
}
