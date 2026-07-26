import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { adminService } from "@/services";
import { Badge } from "@/components/common/Badge";

export const Route = createFileRoute("/admin/tags")({ component: AdminTags });

function AdminTags() {
  const [items, setItems] = useState([]);
  useEffect(() => { adminService.tags().then(setItems); }, []);
  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Tags</h1>
      <div className="flex flex-wrap gap-2 rounded-2xl border border-border bg-card p-5 shadow-soft">
        {items.map((t) => <Badge key={t.id} variant="outline">{t.name}</Badge>)}
      </div>
    </div>
  );
}
