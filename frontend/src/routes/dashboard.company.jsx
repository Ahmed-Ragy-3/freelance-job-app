import { createFileRoute, Link } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { profileService } from "@/services";
import { useAuth } from "@/context/AuthContext";
import { Avatar } from "@/components/common/Avatar";
import { Button } from "@/components/ui/FormKit";
import { Edit } from "lucide-react";

export const Route = createFileRoute("/dashboard/company")({ component: Company });

function Company() {
  const { user } = useAuth();
  const [c, setC] = useState(null);
  useEffect(() => { if (user) profileService.getClient(user.id).then(setC); }, [user]);
  if (!c) return null;
  return (
    <div className="space-y-5">
      <div className="rounded-2xl border border-border bg-card p-6 shadow-soft">
        <div className="flex flex-wrap items-start justify-between gap-4">
          <div className="flex items-center gap-4">
            <Avatar src={c.logo} name={c.companyName} size={64} />
            <div>
              <h1 className="text-2xl font-bold">{c.companyName}</h1>
              <p className="text-sm text-muted-foreground">Client account</p>
            </div>
          </div>
          <Link to="/dashboard/profile/edit"><Button variant="outline"><Edit size={14} />Edit</Button></Link>
        </div>
        <p className="mt-6 text-muted-foreground">{c.companyDetails}</p>
      </div>
    </div>
  );
}
