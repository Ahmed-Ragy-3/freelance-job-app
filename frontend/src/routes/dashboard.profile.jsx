import { createFileRoute, Link, Outlet, useRouterState } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { profileService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Avatar } from "@/components/common/Avatar";
import { Rating } from "@/components/common/Rating";
import { Badge } from "@/components/common/Badge";
import { Button } from "@/components/ui/FormKit";
import { MapPin, Briefcase, ExternalLink, Edit } from "lucide-react";
import { formatMoney } from "@/utils/format";

export const Route = createFileRoute("/dashboard/profile")({
  component: Profile,
});

function Profile() {
  const pathname = useRouterState({ select: (s) => s.location.pathname });
  const isEdit = pathname.endsWith("/edit");
  const { user, isFreelancer, isAdmin } = useAuth();
  const [prof, setProf] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (isEdit || !user) return;
    if (isAdmin) { setProf(null); setLoading(false); return; }
    setLoading(true);
    (isFreelancer ? profileService.getFreelancer(user.id) : profileService.getClient(user.id))
      .then((p) => { setProf(p); setLoading(false); });
  }, [user, isFreelancer, isAdmin, isEdit]);

  if (isEdit) return <Outlet />;

  if (!user) return <p className="text-sm text-muted-foreground">Loading profile…</p>;

  if (isAdmin) {
    return (
      <div className="space-y-6">
        <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
          <div className="flex flex-wrap items-start justify-between gap-4">
            <div className="flex items-center gap-4">
              <Avatar src={user.imageUrl} name={user.username} size={72} />
              <div>
                <h1 className="text-2xl font-bold">{user.username}</h1>
                <p className="text-sm text-muted-foreground">{user.email}</p>
                <div className="mt-2"><Badge variant="primary">Admin</Badge></div>
              </div>
            </div>
            <Link to="/admin"><Button variant="outline">Open admin panel</Button></Link>
          </div>
          <p className="mt-4 text-sm text-muted-foreground">You have administrator access. Manage users, jobs, categories, skills and platform settings from the admin panel.</p>
        </div>
      </div>
    );
  }

  if (loading) return <p className="text-sm text-muted-foreground">Loading profile…</p>;
  if (!prof) return <p className="text-sm text-muted-foreground">No profile data found.</p>;

  return (
    <div className="space-y-6">
      <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div className="flex items-center gap-4">
            <Avatar src={user.imageUrl} name={user.username} size={72} />
            <div>
              <h1 className="text-2xl font-bold">{prof.name || prof.companyName}</h1>
              <p className="text-sm text-muted-foreground">{prof.title || prof.companyDetails}</p>
              {isFreelancer && (
                <div className="mt-2 flex items-center gap-3 text-xs text-muted-foreground">
                  <Rating value={prof.rating} />
                  <span className="inline-flex items-center gap-1"><MapPin size={11} />{prof.location}</span>
                  <span className="inline-flex items-center gap-1"><Briefcase size={11} />{prof.completed} projects</span>
                </div>
              )}
            </div>
          </div>
          <Link to="/dashboard/profile/edit" className="inline-flex items-center gap-2 rounded-lg border border-border bg-background px-4 py-2.5 text-sm font-semibold hover:bg-accent"><Edit size={14} /> Edit profile</Link>
        </div>
        <p className="mt-4 text-sm text-muted-foreground">{prof.bio || prof.companyDetails}</p>
        {isFreelancer && (
          <>
            <div className="mt-6 flex flex-wrap gap-1.5">
              {(prof.skills || []).map((s) => {
                const label = typeof s === "string" ? s : s.skillName || s.name || "";
                const key = typeof s === "object" ? s.skillId ?? s.id ?? label : label;
                return label ? <Badge key={key} variant="primary">{label}</Badge> : null;
              })}
            </div>
            <div className="mt-6 grid gap-4 sm:grid-cols-3">
              <div className="rounded-xl border border-border bg-background p-4"><div className="text-xs text-muted-foreground">Hourly rate</div><div className="mt-1 text-lg font-semibold">{formatMoney(prof.averageRate || prof.avgRate)}/hr</div></div>
              <div className="rounded-xl border border-border bg-background p-4"><div className="text-xs text-muted-foreground">Rating</div><div className="mt-1 text-lg font-semibold">{prof.rating || prof.avgRate || 0}</div></div>
              <div className="rounded-xl border border-border bg-background p-4"><div className="text-xs text-muted-foreground">Website</div>{prof.link ? <a href={prof.link} target="_blank" rel="noreferrer" className="mt-1 inline-flex items-center gap-1 text-sm text-primary hover:underline">{prof.link} <ExternalLink size={12} /></a> : <div className="mt-1 text-sm text-muted-foreground">—</div>}</div>
            </div>
          </>
        )}
      </div>
    </div>
  );
}
