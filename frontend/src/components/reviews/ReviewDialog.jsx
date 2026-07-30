import { useState } from "react";
import { Star, X } from "lucide-react";
import { motion, AnimatePresence } from "framer-motion";
import { z } from "zod";
import toast from "react-hot-toast";
import { Button, Field, Textarea } from "@/components/ui/FormKit";
import { reviewService } from "@/services";

const schema = z.object({
  rating: z.number().int().min(1, "Select a rating").max(5),
  text: z.string().trim().min(10, "Comment must be at least 10 characters").max(1000, "Comment must be under 1000 characters"),
});

function StarInput({ value, onChange }) {
  const [hover, setHover] = useState(0);
  return (
    <div className="flex items-center gap-1" role="radiogroup" aria-label="Rating">
      {[1, 2, 3, 4, 5].map((i) => (
        <button
          key={i}
          type="button"
          role="radio"
          aria-checked={value === i}
          aria-label={`${i} star${i > 1 ? "s" : ""}`}
          onMouseEnter={() => setHover(i)}
          onMouseLeave={() => setHover(0)}
          onClick={() => onChange(i)}
          className="rounded p-0.5 transition-transform hover:scale-110"
        >
          <Star size={26} className={i <= (hover || value) ? "fill-amber-400 text-amber-400" : "text-muted-foreground/40"} />
        </button>
      ))}
      {value > 0 && <span className="ml-2 text-sm font-medium text-muted-foreground">{value}.0</span>}
    </div>
  );
}

// Client-only review form for a completed job. Submits through reviewService,
// which enforces the eligibility rules server-side-ready.
export function ReviewDialog({ open, onClose, job, freelancerName, clientId, onSubmitted }) {
  const [rating, setRating] = useState(0);
  const [text, setText] = useState("");
  const [errors, setErrors] = useState({});
  const [saving, setSaving] = useState(false);

  const close = () => { if (!saving) { setRating(0); setText(""); setErrors({}); onClose(); } };

  const submit = async (e) => {
    e.preventDefault();
    const parsed = schema.safeParse({ rating, text });
    if (!parsed.success) {
      const fe = {};
      parsed.error.issues.forEach((i) => { fe[i.path[0]] = i.message; });
      setErrors(fe);
      return;
    }
    setErrors({});
    setSaving(true);
    try {
      const review = await reviewService.create({ jobId: job.id, from: clientId, rating, text });
      toast.success("Review submitted");
      onSubmitted?.(review);
      setRating(0); setText("");
      onClose();
    } catch (err) {
      toast.error(err.message || "Could not submit review");
    } finally {
      setSaving(false);
    }
  };

  return (
    <AnimatePresence>
      {open && (
        <motion.div
          initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }}
          className="fixed inset-0 z-50 flex items-center justify-center bg-black/50 p-4"
          onClick={close}
        >
          <motion.div
            initial={{ opacity: 0, y: 12, scale: 0.98 }} animate={{ opacity: 1, y: 0, scale: 1 }} exit={{ opacity: 0, y: 8 }}
            onClick={(e) => e.stopPropagation()}
            className="w-full max-w-lg rounded-2xl border border-border bg-card p-6 shadow-elevated"
            role="dialog" aria-modal="true" aria-label="Leave a review"
          >
            <div className="flex items-start justify-between gap-4">
              <div>
                <h2 className="text-lg font-semibold">Leave a review</h2>
                <p className="mt-1 text-sm text-muted-foreground">
                  {freelancerName} — <span className="text-foreground">{job?.title}</span>
                </p>
              </div>
              <button type="button" onClick={close} aria-label="Close" className="rounded-md p-1 text-muted-foreground hover:bg-accent"><X size={18} /></button>
            </div>
            <form onSubmit={submit} className="mt-6 space-y-5">
              <Field label="Rating" error={errors.rating}>
                <StarInput value={rating} onChange={setRating} />
              </Field>
              <Field label="Comment" error={errors.text} hint="Share how the freelancer performed on this job.">
                <Textarea rows={5} value={text} onChange={(e) => setText(e.target.value)} placeholder="Describe the quality of work, communication and timeliness..." />
              </Field>
              <div className="flex justify-end gap-2">
                <Button type="button" variant="outline" onClick={close} disabled={saving}>Cancel</Button>
                <Button type="submit" disabled={saving}>{saving ? "Submitting..." : "Submit review"}</Button>
              </div>
            </form>
          </motion.div>
        </motion.div>
      )}
    </AnimatePresence>
  );
}
