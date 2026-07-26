import { createFileRoute, Outlet, useNavigate } from "@tanstack/react-router";
import { useEffect } from "react";
import { Navbar } from "@/components/layout/Navbar";
import { DashboardSidebar } from "@/components/layout/DashboardSidebar";
import { Breadcrumbs } from "@/components/layout/Breadcrumbs";
import { useAuth } from "@/context/AuthContext";
import { Spinner } from "@/components/common/States";

export const Route = createFileRoute("/dashboard")({
  head: () => ({ meta: [{ title: "Dashboard — Workly" }, { name: "description", content: "Manage jobs, applications, and your profile." }] }),
  component: DashboardLayout,
});

function DashboardLayout() {
  const { isAuthenticated, loading } = useAuth();
  const navigate = useNavigate();
  useEffect(() => { if (!loading && !isAuthenticated) navigate({ to: "/login" }); }, [loading, isAuthenticated, navigate]);

  if (loading) return <div className="flex min-h-screen items-center justify-center"><Spinner /></div>;
  if (!isAuthenticated) return null;

  return (
    <div className="flex min-h-screen flex-col">
      <Navbar />
      <div className="flex flex-1">
        <DashboardSidebar />
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
