import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { adminService } from "@/services";
import { Badge } from "@/components/common/Badge";

export const Route = createFileRoute("/admin/skills")({ component: AdminSkills });

function AdminSkills() {
  const [items, setItems] = useState([]);
  useEffect(() => { adminService.skills().then(setItems); }, []);
  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Skills</h1>
      <div className="flex flex-wrap gap-2 rounded-2xl border border-border bg-card p-5 shadow-soft">
        {items.map((s) => <Badge key={s.id} variant="primary">{s.name}</Badge>)}
      </div>
    </div>
  );
}
