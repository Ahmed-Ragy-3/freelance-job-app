import { Link, useRouterState } from "@tanstack/react-router";
import { useAuth } from "@/context/AuthContext";
import {
  LayoutDashboard, Briefcase, FileText, User, Star, Bookmark, Bell,
  Users as UsersIcon, Tag, LayoutList, BarChart3, Building2, PlusCircle, ShieldCheck,
} from "lucide-react";
import { cn } from "@/lib/utils";

const groups = {
  Freelancer: [
    { to: "/dashboard", label: "Overview", icon: LayoutDashboard, exact: true },
    { to: "/dashboard/profile", label: "Profile", icon: User },
    { to: "/dashboard/portfolio", label: "Portfolio", icon: FileText },
    { to: "/dashboard/applications", label: "Applications", icon: Briefcase },
    { to: "/dashboard/reviews", label: "Reviews", icon: Star },
    { to: "/dashboard/bookmarks", label: "Bookmarks", icon: Bookmark },
    { to: "/dashboard/notifications", label: "Notifications", icon: Bell },
  ],
  Client: [
    { to: "/dashboard", label: "Overview", icon: LayoutDashboard, exact: true },
    { to: "/dashboard/company", label: "Company", icon: Building2 },
    { to: "/dashboard/jobs", label: "Manage Jobs", icon: Briefcase },
    { to: "/dashboard/jobs/new", label: "Post a Job", icon: PlusCircle },
    { to: "/dashboard/reviews", label: "Reviews", icon: Star },
    { to: "/dashboard/notifications", label: "Notifications", icon: Bell },
  ],
  Admin: [
    { to: "/admin", label: "Overview", icon: LayoutDashboard, exact: true },
    { to: "/admin/users", label: "Users", icon: UsersIcon },
    { to: "/admin/jobs", label: "Jobs", icon: Briefcase },
    { to: "/admin/categories", label: "Categories", icon: LayoutList },
    { to: "/admin/skills", label: "Skills", icon: ShieldCheck },
    { to: "/admin/tags", label: "Tags", icon: Tag },
    { to: "/admin/reports", label: "Reports", icon: BarChart3 },
  ],
};


export function DashboardSidebar({ isAdmin = false }) {
  const { role } = useAuth();
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  const items = isAdmin ? groups.Admin : (groups[role] || groups.Freelancer);
  return (
    <aside className="hidden w-60 shrink-0 border-r border-sidebar-border bg-sidebar lg:block">
      <nav className="sticky top-16 space-y-0.5 p-4">
        <p className="mb-2 px-2 text-xs font-semibold uppercase tracking-wider text-muted-foreground">{isAdmin ? "Admin" : role}</p>
        {items.map((item) => {
          const Icon = item.icon;
          const active = item.exact ? pathname === item.to : pathname === item.to || pathname.startsWith(item.to + "/");
          return (
            <Link key={item.to} to={item.to} className={cn("flex items-center gap-3 rounded-lg px-3 py-2 text-sm font-medium transition-colors", active ? "bg-sidebar-accent text-primary" : "text-sidebar-foreground hover:bg-accent")}>
              <Icon size={16} />{item.label}
            </Link>
          );
        })}
      </nav>
    </aside>
  );
}
