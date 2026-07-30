import { cn } from "@/lib/utils";
import { initials } from "@/utils/format";
export function Avatar({ src, name = "", size = 40, className }) {
  return (
    <div
      className={cn("relative inline-flex shrink-0 items-center justify-center overflow-hidden rounded-full bg-accent text-accent-foreground font-medium", className)}
      style={{ width: size, height: size, fontSize: size * 0.4 }}
    >
      {src ? <img src={src} alt={name} className="h-full w-full object-cover" loading="lazy" /> : <span>{initials(name)}</span>}
    </div>
  );
}
