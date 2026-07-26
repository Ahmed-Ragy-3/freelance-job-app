import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { dashboardService } from "@/services";
import { StatCard } from "@/components/dashboard/StatCard";
import { LineChart, BarChart, PieChart } from "@/components/dashboard/Charts";
import { Users, Briefcase, FileText, DollarSign } from "lucide-react";
import { formatMoney, formatNumber } from "@/utils/format";

export const Route = createFileRoute("/admin/")({ component: AdminHome });

function AdminHome() {
  const [s, setS] = useState(null);
  useEffect(() => { dashboardService.adminStats().then(setS); }, []);
  if (!s) return null;
  return (
    <div className="space-y-6">
      <h1 className="text-3xl font-bold tracking-tight">Admin overview</h1>
      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        <StatCard label="Users" value={formatNumber(s.users)} icon={Users} tone="primary" delta="+8% MoM" />
        <StatCard label="Jobs" value={formatNumber(s.jobs)} icon={Briefcase} tone="info" />
        <StatCard label="Applications" value={formatNumber(s.applications)} icon={FileText} tone="warning" />
        <StatCard label="Revenue" value={formatMoney(s.revenue)} icon={DollarSign} tone="success" delta="+14% MoM" />
      </div>
      <div className="grid gap-6 lg:grid-cols-2">
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft"><h2 className="font-semibold">Jobs created</h2><div className="mt-4"><LineChart labels={s.chart.labels} data={s.chart.jobsCreated} /></div></div>
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft"><h2 className="font-semibold">Applications</h2><div className="mt-4"><BarChart labels={s.chart.labels} data={s.chart.applications} /></div></div>
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft"><h2 className="font-semibold">Revenue</h2><div className="mt-4"><LineChart labels={s.chart.labels} data={s.chart.revenue} color="#f59e0b" /></div></div>
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft"><h2 className="font-semibold">Category mix</h2><div className="mt-4"><PieChart data={s.chart.categoriesPie} /></div></div>
      </div>
    </div>
  );
}
