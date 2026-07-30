import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { dashboardService } from "@/services";
import { LineChart, BarChart, PieChart } from "@/components/dashboard/Charts";

export const Route = createFileRoute("/admin/reports")({ component: Reports });

function Reports() {
  const [s, setS] = useState(null);
  useEffect(() => { dashboardService.adminStats().then(setS); }, []);
  if (!s) return null;
  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Reports</h1>
      <div className="grid gap-6 lg:grid-cols-2">
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft"><h2 className="font-semibold">Monthly activity</h2><div className="mt-4"><LineChart labels={s.chart?.labels} data={s.chart?.applications} label="Applications" /></div></div>
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft"><h2 className="font-semibold">Revenue</h2><div className="mt-4"><BarChart labels={s.chart?.labels} data={s.chart?.revenue} color="#f59e0b" /></div></div>
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft"><h2 className="font-semibold">Category distribution</h2><div className="mt-4"><PieChart data={s.chart?.categoriesPie} /></div></div>
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft"><h2 className="font-semibold">Jobs created</h2><div className="mt-4"><LineChart labels={s.chart?.labels} data={s.chart?.jobsCreated} /></div></div>
      </div>
    </div>
  );
}
