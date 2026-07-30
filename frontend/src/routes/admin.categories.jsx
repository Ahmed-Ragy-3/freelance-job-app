import { createFileRoute } from "@tanstack/react-router";
import { useEffect, useState } from "react";
import { adminService } from "@/services";
import { Input, Button } from "@/components/ui/FormKit";
import toast from "react-hot-toast";

export const Route = createFileRoute("/admin/categories")({ component: AdminCategories });

function AdminCategories() {
  const [items, setItems] = useState([]);
  const [name, setName] = useState("");
  const [editing, setEditing] = useState(null);
  const [editName, setEditName] = useState("");
  const load = () => adminService.categories().then(setItems);
  useEffect(() => { load(); }, []);

  const add = async (e) => {
    e.preventDefault();
    if (!name.trim()) return;
    await adminService.createCategory(name.trim());
    setName(""); toast.success("Category added"); load();
  };
  const save = async (id) => { await adminService.updateCategory(id, editName.trim()); setEditing(null); toast.success("Category updated"); load(); };
  const remove = async (id) => { if (!confirm("Delete this category?")) return; await adminService.deleteCategory(id); toast.success("Category deleted"); load(); };

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Categories</h1>
      <form onSubmit={add} className="flex flex-wrap gap-2 rounded-2xl border border-border bg-card p-4 shadow-soft">
        <Input value={name} onChange={(e) => setName(e.target.value)} placeholder="New category name" className="max-w-xs" />
        <Button>Add category</Button>
      </form>
      <div className="grid gap-3 sm:grid-cols-2 lg:grid-cols-3">
        {items.map((item) => (
          <div key={item.id} className="flex items-center justify-between gap-2 rounded-xl border border-border bg-card p-4 shadow-soft">
            {editing === item.id ? (
              <>
                <Input value={editName} onChange={(e) => setEditName(e.target.value)} />
                <Button size="sm" onClick={() => save(item.id)}>Save</Button>
              </>
            ) : (
              <>
                <span className="font-medium">{item.name}</span>
                <div className="flex gap-2">
                  <Button size="sm" variant="outline" onClick={() => { setEditing(item.id); setEditName(item.name); }}>Edit</Button>
                  <Button size="sm" variant="danger" onClick={() => remove(item.id)}>Delete</Button>
                </div>
              </>
            )}
          </div>
        ))}
        {items.length === 0 && <p className="text-sm text-muted-foreground">No categories yet.</p>}
      </div>
    </div>
  );
}
