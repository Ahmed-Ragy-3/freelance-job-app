import { createFileRoute } from "@tanstack/react-router";
import { Field, Input, Button } from "@/components/ui/FormKit";
import toast from "react-hot-toast";

export const Route = createFileRoute("/admin/settings")({ component: Settings });

function Settings() {
  return (
    <form onSubmit={(e) => { e.preventDefault(); toast.success("Settings saved"); }} className="max-w-xl space-y-5 rounded-2xl border border-border bg-card p-6 shadow-soft">
      <h1 className="text-2xl font-bold">Platform settings</h1>
      <Field label="Platform name"><Input defaultValue="Workly" /></Field>
      <Field label="Support email"><Input type="email" defaultValue="hello@workly.io" /></Field>
      <Field label="Service fee (%)"><Input type="number" defaultValue={8} /></Field>
      <div className="flex justify-end"><Button>Save</Button></div>
    </form>
  );
}
