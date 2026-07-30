import { Link, useRouterState } from "@tanstack/react-router";
import { ChevronRight } from "lucide-react";

export function Breadcrumbs() {
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  if (pathname === "/") return null;
  const parts = pathname.split("/").filter(Boolean);
  return (
    <nav className="flex items-center gap-1.5 text-sm text-muted-foreground" aria-label="Breadcrumb">
      <Link to="/" className="hover:text-foreground">Home</Link>
      {parts.map((p, i) => {
        const href = "/" + parts.slice(0, i + 1).join("/");
        const isLast = i === parts.length - 1;
        const label = p.replace(/-/g, " ").replace(/\b\w/g, (c) => c.toUpperCase());
        return (
          <span key={href} className="flex items-center gap-1.5">
            <ChevronRight size={14} />
            {isLast ? <span className="text-foreground">{label}</span> : <a href={href} className="hover:text-foreground">{label}</a>}
          </span>
        );
      })}
    </nav>
  );
}
