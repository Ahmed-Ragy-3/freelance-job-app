import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { useEffect } from "react";
import { Navbar } from "@/components/layout/Navbar";
import { DashboardSidebar } from "@/components/layout/DashboardSidebar";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { useAuth } from "@/context/AuthContext";
import { Spinner } from "@/components/common/States";

export const Route = createFileRoute("/admin")({
  head: () => ({ meta: [{ title: "Admin — Workly" }, { name: "description", content: "Admin dashboard for managing the marketplace." }] }),
  component: AdminLayout,
});

function AdminLayout() {
  const { isAdmin, loading, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  useEffect(() => { if (!loading) { if (!isAuthenticated) navigate({ to: "/login" }); else if (!isAdmin) navigate({ to: "/dashboard" }); } }, [loading, isAuthenticated, isAdmin, navigate]);
  if (loading || !isAdmin) return <div className="flex min-h-screen items-center justify-center"><Spinner /></div>;
  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <div className="flex flex-1">
        <DashboardSidebar isAdmin />
        <main className="flex-1 bg-muted/30">
          <div className="mx-auto max-w-6xl px-4 py-6 sm:px-8">
            <Breadcrumbs />
            <div className="mt-4"><Outlet /></div>
          </div>
        </main>
      </div>
    </div>
  );
}
