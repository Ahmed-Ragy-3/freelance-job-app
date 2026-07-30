import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { adminService } from "@/services";
import { Badge } from "@/components/common/Badge";
import { Input, Button } from "@/components/ui/FormKit";
import toast from "react-hot-toast";

export const Route = createFileRoute("/admin/skills")({ component: AdminSkills });

function AdminSkills() {
  const [items, setItems] = useState([]);
  const [name, setName] = useState("");
  const [editing, setEditing] = useState(null);
  const [editName, setEditName] = useState("");
  const load = () => adminService.skills().then(setItems);
  useEffect(() => { load(); }, []);

  const add = async (e) => {
    e.preventDefault();
    if (!name.trim()) return;
    try {
      await adminService.createSkill(name.trim());
      setName("");
      toast.success("Skill added");
      load();
    } catch (err) {
      toast.error(err?.message || "Failed to add skill");
    }
  };
  const save = async (id) => {
    try {
      await adminService.updateSkill(id, editName.trim());
      setEditing(null);
      toast.success("Skill updated");
      load();
    } catch (err) {
      toast.error(err?.message || "Failed to update skill");
    }
  };
  const remove = async (id) => {
    if (!confirm("Delete this skill?")) return;
    try {
      await adminService.deleteSkill(id);
      toast.success("Skill deleted");
      load();
    } catch (err) {
      toast.error(err?.message || "Failed to delete skill");
    }
  };

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Skills</h1>
      <form onSubmit={add} className="flex flex-wrap gap-2 rounded-2xl border border-border bg-card p-4 shadow-soft">
        <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="New skill name" className="max-w-xs" />
        <Button>Add skill</Button>
      </form>
      <div className="flex flex-wrap gap-2 rounded-2xl border border-border bg-card p-5 shadow-soft">
        {items.map((s) => (
          <div key={s.id} className="flex items-center gap-2 rounded-full border border-border px-2 py-1">
            {editing === s.id ? (
              <>
                <Input value={editName} onChange={(e) => setEditName(e.target.value)} className="h-8 w-36 py-1" />
                <Button size="sm" onClick={() => save(s.id)}>Save</Button>
              </>
            ) : (
              <>
                <Badge variant="primary">{s.name}</Badge>
                <Button size="sm" variant="ghost" onClick={() => { setEditing(s.id); setEditName(s.name); }}>Edit</Button>
                <Button size="sm" variant="ghost" onClick={() => remove(s.id)}>Delete</Button>
              </>
            )}
          </div>
        ))}
        {items.length === 0 && <p className="text-sm text-muted-foreground">No skills yet.</p>}
      </div>
    </div>
  );
}
