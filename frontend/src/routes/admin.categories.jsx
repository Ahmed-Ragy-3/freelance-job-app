import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { adminService } from "@/services";

function AdminList({ title, fetcher }) {
  const [items, setItems] = useState([]);
  useEffect(() => { fetcher().then(setItems); }, [fetcher]);
  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">{title}</h1>
      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
        {items.map((item) => (
          <div key={item.id} className="flex items-center justify-between rounded-xl border border-border bg-card p-4 shadow-soft">
            <span className="font-medium">{item.name}</span>
            {item.jobs !== undefined && <span className="text-xs text-muted-foreground">{item.jobs} jobs</span>}
          </div>
        ))}
      </div>
    </div>
  );
}

export const Route = createFileRoute("/admin/categories")({
  component: () => <AdminList title="Categories" fetcher={adminService.categories} />,
});
