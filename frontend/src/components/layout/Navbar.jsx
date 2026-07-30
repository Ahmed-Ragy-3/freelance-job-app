import { Link, useRouterState } from "@tanstack/react-router";
import { Menu, Search, Bell, Sun, Moon, LogOut, User, LayoutDashboard, Bookmark, X } from "lucide-react";
import { useState, useEffect, useRef } from "react";
import { useAuth } from "@/context/AuthContext";
import { useTheme } from "@/context/ThemeContext";
import { useNotificationsContext } from "@/context/NotificationContext";
import { Avatar } from "@/components/common/Avatar";
import { Badge } from "@/components/common/Badge";
import { timeAgo } from "@/utils/format";
import { motion, AnimatePresence } from "framer-motion";

export function Navbar() {
  const { user, isAuthenticated, logout } = useAuth();
  const { theme, toggle } = useTheme();
  const { notifications, unreadCount, markRead, markAllRead } = useNotificationsContext();
  const [menuOpen, setMenuOpen] = useState(false);
  const [notifOpen, setNotifOpen] = useState(false);
  const [userOpen, setUserOpen] = useState(false);
  const [search, setSearch] = useState("");
  const notifRef = useRef(null);
  const userRef = useRef(null);
  const pathname = useRouterState({ select: (s) => s.location.pathname });

  useEffect(() => { setMenuOpen(false); setNotifOpen(false); setUserOpen(false); }, [pathname]);
  useEffect(() => {
    const onClick = (e) => {
      if (notifRef.current && !notifRef.current.contains(e.target)) setNotifOpen(false);
      if (userRef.current && !userRef.current.contains(e.target)) setUserOpen(false);
    };
    document.addEventListener("mousedown", onClick);
    return () => document.removeEventListener("mousedown", onClick);
  }, []);

  const navLinks = [
    { to: "/jobs", label: "Find Work" },
    { to: "/about", label: "About" },
  ];

  return (
    <header className="sticky top-0 z-40 border-b border-border glass">
      <div className="mx-auto flex h-16 max-w-7xl items-center gap-4 px-4 sm:px-6">
        <Link to="/" className="flex items-center gap-2 shrink-0">
          <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary text-primary-foreground font-bold">W</div>
          <span className="hidden text-lg font-bold text-foreground sm:inline">Workly</span>
        </Link>

        <nav className="hidden items-center gap-1 md:flex">
          {navLinks.map((l) => (
            <Link key={l.to} to={l.to} className="rounded-md px-3 py-2 text-sm font-medium text-muted-foreground hover:bg-accent hover:text-foreground" activeProps={{ className: "text-foreground bg-accent" }}>{l.label}</Link>
          ))}
        </nav>

        <form
          onSubmit={(e) => { e.preventDefault(); if (search) window.location.href = `/jobs?q=${encodeURIComponent(search)}`; }}
          className="hidden flex-1 max-w-md lg:block"
        >
          <div className="relative">
            <Search size={16} className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-muted-foreground" />
            <input value={search} onChange={(e) => setSearch(e.target.value)} placeholder="Search jobs, skills, companies..." className="w-full rounded-full border border-border bg-background/60 py-2 pl-9 pr-4 text-sm outline-none focus:border-primary" />
          </div>
        </form>

        <div className="ml-auto flex items-center gap-1.5">
          <button onClick={toggle} aria-label="Toggle theme" className="rounded-full p-2 text-muted-foreground hover:bg-accent hover:text-foreground">
            {theme === "dark" ? <Sun size={18} /> : <Moon size={18} />}
          </button>

          {isAuthenticated ? (
            <>
              <div ref={notifRef} className="relative">
                <button onClick={() => setNotifOpen((v) => !v)} className="relative rounded-full p-2 text-muted-foreground hover:bg-accent hover:text-foreground">
                  <Bell size={18} />
                  {unreadCount > 0 && <span className="absolute right-1 top-1 h-2.5 w-2.5 rounded-full bg-primary ring-2 ring-background" />}
                </button>
                <AnimatePresence>
                  {notifOpen && (
                    <motion.div initial={{ opacity: 0, y: -6 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -6 }} className="absolute right-0 mt-2 w-80 overflow-hidden rounded-xl border border-border bg-popover shadow-elevated">
                      <div className="flex items-center justify-between border-b border-border p-3">
                        <span className="text-sm font-semibold">Notifications</span>
                        <div className="flex items-center gap-2">
                          {unreadCount > 0 && (
                            <>
                              <Badge variant="primary">{unreadCount} new</Badge>
                              <button onClick={markAllRead} className="text-xs text-primary hover:underline font-medium">Mark all read</button>
                            </>
                          )}
                        </div>
                      </div>
                      <div className="max-h-96 overflow-y-auto">
                        {notifications.length === 0 && <p className="p-6 text-center text-sm text-muted-foreground">You're all caught up.</p>}
                        {notifications.map((n) => (
                          <div
                            key={n.id}
                            onClick={() => { if (!n.read && !n.isRead) markRead(n.id); }}
                            className={`border-b border-border/60 p-3 last:border-0 hover:bg-accent/50 cursor-pointer transition-colors ${!n.read && !n.isRead ? "bg-accent/20" : ""}`}
                          >
                            <div className="flex justify-between gap-2">
                              <p className="text-sm font-medium text-foreground">{n.title}</p>
                              {(!n.read && !n.isRead) && <span className="mt-1 h-2 w-2 shrink-0 rounded-full bg-primary" />}
                            </div>
                            <p className="mt-0.5 text-xs text-muted-foreground">{n.message || n.body}</p>
                            <p className="mt-1 text-[10px] uppercase tracking-wide text-muted-foreground">{timeAgo(n.createdAt)}</p>
                          </div>
                        ))}
                      </div>
                    </motion.div>
                  )}
                </AnimatePresence>
              </div>

              <div ref={userRef} className="relative">
                <button onClick={() => setUserOpen((v) => !v)} className="flex items-center gap-2 rounded-full p-1 hover:bg-accent">
                  <Avatar src={user.imageUrl} name={user.username} size={32} />
                </button>
                <AnimatePresence>
                  {userOpen && (
                    <motion.div initial={{ opacity: 0, y: -6 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -6 }} className="absolute right-0 mt-2 w-56 overflow-hidden rounded-xl border border-border bg-popover shadow-elevated">
                      <div className="border-b border-border p-3">
                        <p className="truncate text-sm font-semibold">{user.username}</p>
                        <p className="truncate text-xs text-muted-foreground">{user.email}</p>
                        <Badge variant="primary" className="mt-2">{user.role}</Badge>
                      </div>
                      <div className="p-1">
                        <Link to="/dashboard" className="flex items-center gap-2 rounded-md px-3 py-2 text-sm hover:bg-accent"><LayoutDashboard size={15} />Dashboard</Link>
                        <Link to="/dashboard/profile" className="flex items-center gap-2 rounded-md px-3 py-2 text-sm hover:bg-accent"><User size={15} />Profile</Link>
                        <Link to="/dashboard/bookmarks" className="flex items-center gap-2 rounded-md px-3 py-2 text-sm hover:bg-accent"><Bookmark size={15} />Bookmarks</Link>
                        <button onClick={logout} className="flex w-full items-center gap-2 rounded-md px-3 py-2 text-sm text-red-500 hover:bg-accent"><LogOut size={15} />Sign out</button>
                      </div>
                    </motion.div>
                  )}
                </AnimatePresence>
              </div>
            </>
          ) : (
            <div className="hidden items-center gap-2 sm:flex">
              <Link to="/login" className="rounded-md px-3 py-2 text-sm font-medium text-foreground hover:bg-accent">Sign in</Link>
              <Link to="/register" className="rounded-md bg-primary px-4 py-2 text-sm font-semibold text-primary-foreground hover:opacity-90">Join</Link>
            </div>
          )}

          <button className="rounded-md p-2 md:hidden" onClick={() => setMenuOpen(true)}><Menu size={20} /></button>
        </div>
      </div>

      <AnimatePresence>
        {menuOpen && (
          <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="fixed inset-0 z-50 bg-background/80 md:hidden">
            <motion.div initial={{ x: "100%" }} animate={{ x: 0 }} exit={{ x: "100%" }} transition={{ type: "tween" }} className="ml-auto flex h-full w-72 flex-col bg-card p-4">
              <div className="flex justify-end"><button onClick={() => setMenuOpen(false)}><X size={20} /></button></div>
              {navLinks.map((l) => (
                <Link key={l.to} to={l.to} className="rounded-md px-3 py-3 text-base font-medium hover:bg-accent">{l.label}</Link>
              ))}
              {!isAuthenticated && (
                <div className="mt-4 flex flex-col gap-2 border-t border-border pt-4">
                  <Link to="/login" className="rounded-md border border-border px-3 py-2 text-center text-sm">Sign in</Link>
                  <Link to="/register" className="rounded-md bg-primary px-3 py-2 text-center text-sm text-primary-foreground">Join</Link>
                </div>
              )}
            </motion.div>
          </motion.div>
        )}
      </AnimatePresence>
    </header>
  );
}
