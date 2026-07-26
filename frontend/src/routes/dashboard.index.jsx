import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { useAuth } from "@/context/AuthContext";
import { dashboardService, notificationService, applicationService, jobService } from "@/services";
import { StatCard } from "@/components/dashboard/StatCard";
import { LineChart, BarChart, PieChart } from "@/components/dashboard/Charts";
import { JobCard } from "@/components/jobs/JobCard";
import { Badge } from "@/components/common/Badge";
import { formatMoney, timeAgo } from "@/utils/format";
import { Briefcase, CheckCircle2, Clock, DollarSign, Users, TrendingUp, PlusCircle } from "lucide-react";
import { staticData } from "@/services";

export const Route = createFileRoute("/dashboard/")({
  component: DashboardHome,
});

function DashboardHome() {
  const { user, isFreelancer, isClient } = useAuth();
  const [stats, setStats] = useState(null);
  const [apps, setApps] = useState([]);
  const [notifs, setNotifs] = useState([]);
  const [recommended, setRecommended] = useState([]);
  const [myJobs, setMyJobs] = useState([]);

  useEffect(() => {
    if (!user) return;
    if (isFreelancer) {
      dashboardService.freelancerStats(user.id).then(setStats);
      applicationService.list({ freelancerId: user.id }).then(setApps);
      staticData.featuredJobs().then(setRecommended);
    } else if (isClient) {
      dashboardService.clientStats(user.id).then(setStats);
      jobService.byClient(user.id).then(setMyJobs);
    }
    notificationService.list(user.id).then(setNotifs);
  }, [user, isFreelancer, isClient]);

  if (!stats) return null;

  return (
    <div className="space-y-6">
      <header className="flex flex-wrap items-end justify-between gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Welcome back, {user.username}</h1>
          <p className="mt-1 text-sm text-muted-foreground">Here's what's happening across your account.</p>
        </div>
        {isClient && <Link to="/dashboard/jobs/new" className="inline-flex items-center gap-2 rounded-lg bg-primary px-4 py-2.5 text-sm font-semibold text-primary-foreground hover:opacity-90"><PlusCircle size={16} /> Post a job</Link>}
      </header>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {isFreelancer && (
          <>
            <StatCard label="Applications" value={stats.totalApplications} icon={Briefcase} tone="primary" delta="+12% this month" />
            <StatCard label="Accepted" value={stats.accepted} icon={CheckCircle2} tone="success" />
            <StatCard label="Pending" value={stats.pending} icon={Clock} tone="warning" />
            <StatCard label="Earnings" value={formatMoney(stats.earnings)} icon={DollarSign} tone="info" delta="+$1.2k this week" />
          </>
        )}
        {isClient && (
          <>
            <StatCard label="Total jobs" value={stats.totalJobs} icon={Briefcase} tone="primary" />
            <StatCard label="Open jobs" value={stats.openJobs} icon={TrendingUp} tone="success" />
            <StatCard label="Applications" value={stats.applications} icon={Users} tone="info" />
            <StatCard label="Total spend" value={formatMoney(stats.spend)} icon={DollarSign} tone="warning" />
          </>
        )}
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft lg:col-span-2">
          <h2 className="font-semibold">Activity over time</h2>
          <p className="text-sm text-muted-foreground">Monthly {isFreelancer ? "applications" : "jobs posted"}</p>
          <div className="mt-4"><LineChart labels={stats.chart.labels} data={isFreelancer ? stats.chart.applications : stats.chart.jobsCreated} /></div>
        </div>
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft">
          <h2 className="font-semibold">By category</h2>
          <div className="mt-2"><PieChart data={stats.chart.categoriesPie} /></div>
        </div>
      </div>

      <div className="grid gap-6 lg:grid-cols-3">
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft lg:col-span-2">
          <h2 className="font-semibold">Revenue trend</h2>
          <div className="mt-2"><BarChart labels={stats.chart.labels} data={stats.chart.revenue} /></div>
        </div>
        <div className="rounded-2xl border border-border bg-card p-5 shadow-soft">
          <h2 className="font-semibold">Notifications</h2>
          <div className="mt-3 space-y-3">
            {notifs.slice(0, 4).map((n) => (
              <div key={n.id} className="border-b border-border/50 pb-3 last:border-0">
                <div className="flex justify-between gap-2"><p className="text-sm font-medium">{n.title}</p>{!n.read && <span className="mt-1 h-2 w-2 rounded-full bg-primary" />}</div>
                <p className="mt-1 text-xs text-muted-foreground">{timeAgo(n.createdAt)}</p>
              </div>
            ))}
            {notifs.length === 0 && <p className="text-sm text-muted-foreground">No notifications yet.</p>}
          </div>
        </div>
      </div>

      {isFreelancer && (
        <>
          <section>
            <h2 className="mb-4 text-lg font-semibold">Recent applications</h2>
            <div className="overflow-hidden rounded-2xl border border-border bg-card">
              <table className="w-full text-sm">
                <thead className="bg-muted/50 text-left text-xs uppercase tracking-wider text-muted-foreground">
                  <tr><th className="p-3">Job</th><th className="p-3">Bid</th><th className="p-3">Timeline</th><th className="p-3">Status</th><th className="p-3">Date</th></tr>
                </thead>
                <tbody>
                  {apps.slice(0, 5).map((a) => (
                    <tr key={a.id} className="border-t border-border">
                      <td className="p-3 font-medium">{a.job?.title || "—"}</td>
                      <td className="p-3">{formatMoney(a.bid)}</td>
                      <td className="p-3">{a.timeline}</td>
                      <td className="p-3"><Badge variant={a.status === "Accepted" ? "success" : a.status === "Rejected" ? "danger" : a.status === "Shortlisted" ? "info" : "warning"}>{a.status}</Badge></td>
                      <td className="p-3 text-muted-foreground">{timeAgo(a.createdAt)}</td>
                    </tr>
                  ))}
                  {apps.length === 0 && <tr><td colSpan="5" className="p-6 text-center text-muted-foreground">No applications yet.</td></tr>}
                </tbody>
              </table>
            </div>
          </section>
          <section>
            <h2 className="mb-4 text-lg font-semibold">Recommended for you</h2>
            <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">{recommended.slice(0, 3).map((j) => <JobCard key={j.id} job={j} />)}</div>
          </section>
        </>
      )}

      {isClient && (
        <section>
          <h2 className="mb-4 text-lg font-semibold">Your recent jobs</h2>
          <div className="grid gap-5 md:grid-cols-2 lg:grid-cols-3">{myJobs.slice(0, 3).map((j) => <JobCard key={j.id} job={j} />)}</div>
        </section>
      )}
    </div>
  );
}
