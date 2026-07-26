import { cn } from "@/lib/utils";
export function Badge({ children, variant = "default", className }) {
  const variants = {
    default: "bg-secondary text-secondary-foreground",
    primary: "bg-primary/10 text-primary",
    success: "bg-emerald-500/10 text-emerald-600 dark:text-emerald-400",
    warning: "bg-amber-500/10 text-amber-600 dark:text-amber-400",
    danger: "bg-red-500/10 text-red-600 dark:text-red-400",
    info: "bg-sky-500/10 text-sky-600 dark:text-sky-400",
    outline: "border border-border text-foreground",
  };
  return <span className={cn("inline-flex items-center gap-1 rounded-full px-2.5 py-0.5 text-xs font-medium", variants[variant], className)}>{children}</span>;
}
