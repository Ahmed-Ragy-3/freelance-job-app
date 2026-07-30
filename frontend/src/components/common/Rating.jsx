import { Star } from "lucide-react";
export function Rating({ value = 0, size = 14, showValue = true }) {
  return (
    <div className="inline-flex items-center gap-1">
      <div className="flex">
        {[1,2,3,4,5].map((i) => (
          <Star key={i} size={size} className={i <= Math.round(value) ? "fill-amber-400 text-amber-400" : "text-muted-foreground/40"} />
        ))}
      </div>
      {showValue && <span className="text-xs font-medium text-muted-foreground">{value.toFixed(1)}</span>}
    </div>
  );
}
