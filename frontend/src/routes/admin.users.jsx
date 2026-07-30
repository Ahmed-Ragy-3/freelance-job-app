import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { adminService } from "@/services";
import { Avatar } from "@/components/common/Avatar";
import { Badge } from "@/components/common/Badge";
import { Button } from "@/components/ui/FormKit";
import { formatDate } from "@/utils/format";
import toast from "react-hot-toast";

export const Route = createFileRoute("/admin/users")({ component: AdminUsers });

function AdminUsers() {
  const [items, setItems] = useState([]);
  const [q, setQ] = useState("");
  useEffect(() => { adminService.users().then(setItems); }, []);
  const remove = async (id) => { if (!confirm("Delete this user?")) return; await adminService.deleteUser(id); setItems(items.filter((u) => u.id !== id)); toast.success("Deleted"); };
  const filtered = items.filter((u) => (u.username + u.email).toLowerCase().includes(q.toLowerCase()));

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Users</h1>
      <input value={q} onChange={(e) => setQ(e.target.value)} placeholder="Search…" className="w-full max-w-sm rounded-lg border border-border bg-card px-3 py-2 text-sm outline-none focus:border-primary" />
      <div className="overflow-hidden rounded-2xl border border-border bg-card shadow-soft">
        <div className="overflow-x-auto">
          <table className="w-full text-sm">
            <thead className="bg-muted/50 text-left text-xs uppercase tracking-wider text-muted-foreground">
              <tr><th className="p-3">User</th><th className="p-3">Email</th><th className="p-3">Role</th><th className="p-3">Joined</th><th className="p-3 text-right">Actions</th></tr>
            </thead>
            <tbody>
              {filtered.map((u) => (
                <tr key={u.id} className="border-t border-border">
                  <td className="p-3"><div className="flex items-center gap-2"><Avatar src={u.imageUrl} name={u.username} size={32} /><span className="font-medium">{u.username}</span></div></td>
                  <td className="p-3 text-muted-foreground">{u.email}</td>
                  <td className="p-3"><Badge variant={u.role === "Admin" ? "danger" : u.role === "Client" ? "info" : "primary"}>{u.role}</Badge></td>
                  <td className="p-3 text-muted-foreground">{formatDate(u.createdAt)}</td>
                  <td className="p-3 text-right"><Button size="sm" variant="danger" onClick={() => remove(u.id)}>Delete</Button></td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}
