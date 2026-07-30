import { cn } from "@/lib/utils";
export function Skeleton({ className }) {
  return <div className={cn("animate-pulse rounded-md bg-muted", className)} />;
}
export function Spinner({ className }) {
  return <div className={cn("h-6 w-6 animate-spin rounded-full border-2 border-muted border-t-primary", className)} />;
}
export function EmptyState({ icon: Icon, title, description, action }) {
  return (
    <div className="flex flex-col items-center justify-center rounded-2xl border border-dashed border-border bg-card/50 px-6 py-16 text-center">
      {Icon && <div className="mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-accent text-accent-foreground"><Icon size={22} /></div>}
      <h3 className="text-base font-semibold text-foreground">{title}</h3>
      {description && <p className="mt-1.5 max-w-md text-sm text-muted-foreground">{description}</p>}
      {action && <div className="mt-6">{action}</div>}
    </div>
  );
}
export function ErrorState({ title = "Something went wrong", description, onRetry }) {
  return (
    <div className="rounded-2xl border border-destructive/30 bg-destructive/5 p-8 text-center">
      <h3 className="font-semibold text-destructive">{title}</h3>
      {description && <p className="mt-1 text-sm text-muted-foreground">{description}</p>}
      {onRetry && <button onClick={onRetry} className="mt-4 rounded-md bg-primary px-4 py-2 text-sm font-medium text-primary-foreground">Try again</button>}
    </div>
  );
}
