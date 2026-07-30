import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { adminService } from "@/services";
import { Badge } from "@/components/common/Badge";
import { Input, Button } from "@/components/ui/FormKit";
import toast from "react-hot-toast";

export const Route = createFileRoute("/admin/tags")({ component: AdminTags });

function AdminTags() {
  const [items, setItems] = useState([]);
  const [name, setName] = useState("");
  const [editing, setEditing] = useState(null);
  const [editName, setEditName] = useState("");
  const load = () => adminService.tags().then(setItems);
  useEffect(() => { load(); }, []);

  const add = async (e) => {
    e.preventDefault();
    if (!name.trim()) return;
    try {
      await adminService.createTag(name.trim());
      setName("");
      toast.success("Tag added");
      load();
    } catch (err) {
      toast.error(err?.message || "Failed to add tag");
    }
  };
  const save = async (id) => {
    try {
      await adminService.updateTag(id, editName.trim());
      setEditing(null);
      toast.success("Tag updated");
      load();
    } catch (err) {
      toast.error(err?.message || "Failed to update tag");
    }
  };
  const remove = async (id) => {
    if (!confirm("Delete this tag?")) return;
    try {
      await adminService.deleteTag(id);
      toast.success("Tag deleted");
      load();
    } catch (err) {
      toast.error(err?.message || "Failed to delete tag");
    }
  };

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Tags</h1>
      <form onSubmit={add} className="flex flex-wrap gap-2 rounded-2xl border border-border bg-card p-4 shadow-soft">
        <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="New tag name" className="max-w-xs" />
        <Button>Add tag</Button>
      </form>
      <div className="flex flex-wrap gap-2 rounded-2xl border border-border bg-card p-5 shadow-soft">
        {items.map((t) => (
          <div key={t.id} className="flex items-center gap-2 rounded-full border border-border px-2 py-1">
            {editing === t.id ? (
              <>
                <Input value={editName} onChange={(e) => setEditName(e.target.value)} className="h-8 w-36 py-1" />
                <Button size="sm" onClick={() => save(t.id)}>Save</Button>
              </>
            ) : (
              <>
                <Badge variant="outline">{t.name}</Badge>
                <Button size="sm" variant="ghost" onClick={() => { setEditing(t.id); setEditName(t.name); }}>Edit</Button>
                <Button size="sm" variant="ghost" onClick={() => remove(t.id)}>Delete</Button>
              </>
            )}
          </div>
        ))}
        {items.length === 0 && <p className="text-sm text-muted-foreground">No tags yet.</p>}
      </div>
    </div>
  );
}
