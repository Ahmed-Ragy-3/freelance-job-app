import { cn } from "@/lib/utils";
export function StatCard({ label, value, icon: Icon, delta, tone = "primary" }) {
  const tones = {
    primary: "from-primary/15 to-primary/5 text-primary",
    warning: "from-amber-500/15 to-amber-500/5 text-amber-500",
    info: "from-sky-500/15 to-sky-500/5 text-sky-500",
    success: "from-emerald-500/15 to-emerald-500/5 text-emerald-500",
  };
  return (
    <div className="rounded-2xl border border-border bg-card p-5 shadow-soft">
      <div className="flex items-start justify-between">
        <div>
          <p className="text-sm text-muted-foreground">{label}</p>
          <p className="mt-1 text-2xl font-bold text-foreground">{value}</p>
          {delta && <p className="mt-1 text-xs text-emerald-500">{delta}</p>}
        </div>
        {Icon && <div className={cn("flex h-10 w-10 items-center justify-center rounded-xl bg-gradient-to-br", tones[tone])}><Icon size={18} /></div>}
      </div>
    </div>
  );
}
