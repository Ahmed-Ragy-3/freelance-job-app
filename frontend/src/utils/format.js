export const formatMoney = (n) => new Intl.NumberFormat("en-US", { style: "currency", currency: "USD", maximumFractionDigits: 0 }).format(n || 0);
export const formatNumber = (n) => new Intl.NumberFormat("en-US").format(n || 0);
export const formatDate = (d) => new Date(d).toLocaleDateString("en-US", { month: "short", day: "numeric", year: "numeric" });
export const timeAgo = (d) => {
  const diff = (Date.now() - new Date(d).getTime()) / 1000;
  if (diff < 60) return "just now";
  if (diff < 3600) return `${Math.floor(diff / 60)}m ago`;
  if (diff < 86400) return `${Math.floor(diff / 3600)}h ago`;
  if (diff < 2592000) return `${Math.floor(diff / 86400)}d ago`;
  return formatDate(d);
};
export const daysUntil = (d) => Math.max(0, Math.ceil((new Date(d).getTime() - Date.now()) / 86400000));
export const initials = (name = "") => name.split(" ").map((w) => w[0]).slice(0, 2).join("").toUpperCase();
