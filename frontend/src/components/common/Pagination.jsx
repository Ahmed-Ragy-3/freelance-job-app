import { cn } from "@/lib/utils";
export function Pagination({ page, pageSize, total, onPage }) {
  const pages = Math.max(1, Math.ceil(total / pageSize));
  if (pages <= 1) return null;
  const list = Array.from({ length: pages }, (_, i) => i + 1);
  return (
    <div className="flex items-center justify-center gap-1">
      <button disabled={page === 1} onClick={() => onPage(page - 1)} className="rounded-md border border-border px-3 py-1.5 text-sm disabled:opacity-50 hover:bg-accent">Prev</button>
      {list.map((n) => (
        <button key={n} onClick={() => onPage(n)} className={cn("rounded-md px-3 py-1.5 text-sm", n === page ? "bg-primary text-primary-foreground" : "hover:bg-accent")}>{n}</button>
      ))}
      <button disabled={page === pages} onClick={() => onPage(page + 1)} className="rounded-md border border-border px-3 py-1.5 text-sm disabled:opacity-50 hover:bg-accent">Next</button>
    </div>
  );
}
