import { Link, useNavigate } from "@tanstack/react-router";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useAuth } from "@/context/AuthContext";
import toast from "react-hot-toast";
import { motion } from "framer-motion";

// Centered auth card layout (previously a split-screen).
export function AuthShell({ title, subtitle, children, footer }) {
  return (
    <div className="relative flex min-h-screen items-center justify-center overflow-hidden px-4 py-12">
      <div className="absolute inset-0 -z-10" style={{ background: "var(--gradient-hero)" }} />
      <motion.div
        initial={{ opacity: 0, y: 12 }}
        animate={{ opacity: 1, y: 0 }}
        className="w-full max-w-md rounded-2xl border border-border bg-card p-8 shadow-elevated sm:p-10"
      >
        <Link to="/" className="mb-6 inline-flex items-center gap-2">
          <div className="flex h-9 w-9 items-center justify-center rounded-lg bg-primary text-primary-foreground font-bold">W</div>
          <span className="text-lg font-bold text-foreground">Workly</span>
        </Link>
        <h1 className="text-2xl font-bold text-foreground">{title}</h1>
        {subtitle && <p className="mt-2 text-sm text-muted-foreground">{subtitle}</p>}
        <div className="mt-8">{children}</div>
        {footer && <div className="mt-6 text-center text-sm text-muted-foreground">{footer}</div>}
      </motion.div>
    </div>
  );
}

export function Field({ label, error, children, hint }) {
  return (
    <div>
      {label && <label className="mb-1.5 block text-sm font-medium text-foreground">{label}</label>}
      {children}
      {hint && !error && <p className="mt-1 text-xs text-muted-foreground">{hint}</p>}
      {error && <p className="mt-1 text-xs text-destructive">{error}</p>}
    </div>
  );
}
export function Input(props) {
  return <input {...props} className={"w-full rounded-lg border border-border bg-background px-3.5 py-2.5 text-sm text-foreground outline-none transition-colors placeholder:text-muted-foreground focus:border-primary " + (props.className || "")} />;
}
export function Textarea(props) {
  return <textarea {...props} className={"w-full rounded-lg border border-border bg-background px-3.5 py-2.5 text-sm text-foreground outline-none transition-colors placeholder:text-muted-foreground focus:border-primary " + (props.className || "")} />;
}
export function Select({ children, ...props }) {
  return <select {...props} className={"w-full rounded-lg border border-border bg-background px-3.5 py-2.5 text-sm outline-none transition-colors focus:border-primary " + (props.className || "")}>{children}</select>;
}
// Chip-style multi select used for categories, tags and skills.
export function MultiSelect({ options = [], value = [], onChange, getKey = (o) => o.value, getLabel = (o) => o.label, empty = "No options" }) {
  if (!options.length) return <p className="text-xs text-muted-foreground">{empty}</p>;
  const toggle = (k) => onChange(value.includes(k) ? value.filter((x) => x !== k) : [...value, k]);
  return (
    <div className="flex flex-wrap gap-1.5">
      {options.map((o) => {
        const k = getKey(o);
        const active = value.includes(k);
        return (
          <button
            type="button"
            key={k}
            aria-pressed={active}
            onClick={() => toggle(k)}
            className={"rounded-full border px-2.5 py-1 text-xs transition-colors " + (active ? "border-primary bg-primary/10 text-primary" : "border-border text-muted-foreground hover:bg-accent")}
          >
            {getLabel(o)}
          </button>
        );
      })}
    </div>
  );
}

export function Button({ children, variant = "primary", size = "md", className = "", ...props }) {
  const variants = {
    primary: "bg-primary text-primary-foreground hover:opacity-90",
    outline: "border border-border bg-background text-foreground hover:bg-accent",
    ghost: "text-foreground hover:bg-accent",
    danger: "bg-destructive text-destructive-foreground hover:opacity-90",
  };
  const sizes = { sm: "px-3 py-1.5 text-xs", md: "px-4 py-2.5 text-sm", lg: "px-6 py-3 text-base" };
  return <button {...props} className={`inline-flex items-center justify-center gap-2 rounded-lg font-semibold transition-opacity disabled:opacity-50 ${variants[variant]} ${sizes[size]} ${className}`}>{children}</button>;
}

export { useForm, zodResolver, z, useNavigate, useAuth, toast };
