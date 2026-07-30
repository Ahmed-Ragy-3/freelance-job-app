// Mocked REST-like services. Each returns a Promise to simulate network.
import * as db from "./mockData";

const delay = (ms = 350) => new Promise((r) => setTimeout(r, ms));
const clone = (x) => JSON.parse(JSON.stringify(x));

// ---------------- Auth ----------------
const USERS_KEY = "mp_users";
const SESSION_KEY = "mp_session";
const initUsers = () => {
  if (typeof window === "undefined") return;
  if (!localStorage.getItem(USERS_KEY)) {
    localStorage.setItem(USERS_KEY, JSON.stringify(db.users.map((u) => ({ ...u, password: "password123" }))));
  }
};
const readUsers = () => { initUsers(); return JSON.parse(localStorage.getItem(USERS_KEY) || "[]"); };
const writeUsers = (u) => localStorage.setItem(USERS_KEY, JSON.stringify(u));

export const authService = {
  async login({ email, password }) {
    await delay();
    const u = readUsers().find((x) => x.email === email && x.password === password);
    if (!u) throw new Error("Invalid credentials");
    const session = { userId: u.id, role: u.role };
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    return { user: { ...u, password: undefined }, session };
  },
  async register({ username, email, password, role, companyName, bio }) {
    await delay();
    const users = readUsers();
    if (users.some((u) => u.email === email)) throw new Error("Email already registered");
    const id = `u${Date.now()}`;
    const newUser = { id, username, email, password, role, imageUrl: `https://i.pravatar.cc/200?u=${id}`, createdAt: new Date().toISOString() };
    users.push(newUser);
    writeUsers(users);
    const session = { userId: id, role };
    localStorage.setItem(SESSION_KEY, JSON.stringify(session));
    return { user: { ...newUser, password: undefined }, session, extra: { companyName, bio } };
  },
  async logout() { await delay(120); localStorage.removeItem(SESSION_KEY); },
  async me() {
    await delay(120);
    const s = JSON.parse(localStorage.getItem(SESSION_KEY) || "null");
    if (!s) return null;
    const u = readUsers().find((x) => x.id === s.userId);
    return u ? { ...u, password: undefined } : null;
  },
  async verifyEmail(token) { await delay(); return { ok: true }; },
};

// ---------------- Jobs ----------------
export const jobService = {
  async list({ search = "", category = "", skills = [], status = "", minBudget = 0, maxBudget = Infinity, sort = "newest", page = 1, pageSize = 9, includeAll = false } = {}) {
    await delay();
    let out = clone(db.jobs);
    if (!includeAll) out = out.filter((j) => j.status !== "Pending" && j.status !== "Rejected");
    if (search) out = out.filter((j) => (j.title + j.description).toLowerCase().includes(search.toLowerCase()));
    if (category) out = out.filter((j) => (j.categoryIds || [j.categoryId]).includes(category));
    if (skills.length) out = out.filter((j) => skills.every((s) => j.requiredSkills.includes(s)));
    if (status) out = out.filter((j) => j.status === status);
    out = out.filter((j) => j.budget >= minBudget && j.budget <= maxBudget);
    if (sort === "newest") out.sort((a,b) => new Date(b.createdAt) - new Date(a.createdAt));
    if (sort === "oldest") out.sort((a,b) => new Date(a.createdAt) - new Date(b.createdAt));
    if (sort === "budget") out.sort((a,b) => b.budget - a.budget);
    if (sort === "deadline") out.sort((a,b) => new Date(a.deadline) - new Date(b.deadline));
    const total = out.length;
    const start = (page - 1) * pageSize;
    return { items: out.slice(start, start + pageSize), total, page, pageSize };
  },
  async get(id) { await delay(); return clone(db.jobs.find((j) => j.id === id)); },
  async similar(id) { await delay(); const j = db.jobs.find((x) => x.id === id); return clone(db.jobs.filter((x) => x.categoryId === j?.categoryId && x.id !== id && x.status !== "Pending" && x.status !== "Rejected").slice(0, 3)); },
  async create(data) { await delay(); const j = { id: `j${Date.now()}`, createdAt: new Date().toISOString(), status: "Pending", proposals: 0, cover: `https://picsum.photos/seed/${Date.now()}/800/400`, attachments: [], ...data, status: "Pending" }; db.jobs.unshift(j); return j; },
  async update(id, patch) { await delay(); const j = db.jobs.find((x) => x.id === id); Object.assign(j, patch); return clone(j); },
  async remove(id) { await delay(); const i = db.jobs.findIndex((x) => x.id === id); if (i >= 0) db.jobs.splice(i, 1); return { ok: true }; },
  async byClient(clientId) { await delay(); return clone(db.jobs.filter((j) => j.clientId === clientId)); },
  async approve(id) { await delay(); const j = db.jobs.find((x) => x.id === id); if (j) j.status = "Open"; return clone(j); },
  async reject(id) { await delay(); const j = db.jobs.find((x) => x.id === id); if (j) j.status = "Rejected"; return clone(j); },
  async complete(id) { await delay(); const j = db.jobs.find((x) => x.id === id); if (j) j.status = "Completed"; return clone(j); },

};

// ---------------- Applications ----------------
export const applicationService = {
  async list({ freelancerId, jobId } = {}) {
    await delay();
    let out = clone(db.applications);
    if (freelancerId) out = out.filter((a) => a.freelancerId === freelancerId);
    if (jobId) out = out.filter((a) => a.jobId === jobId);
    return out.map((a) => ({ ...a, job: db.jobs.find((j) => j.id === a.jobId) }));
  },
  async get(id) { await delay(); const a = db.applications.find((x) => x.id === id); return a ? clone(a) : null; },
  async getByFreelancerAndJob(freelancerId, jobId) {
    await delay(120);
    const a = db.applications.find((x) => x.freelancerId === freelancerId && x.jobId === jobId);
    return a ? clone(a) : null;
  },
  async create(data) { await delay(); const a = { id: `a${Date.now()}`, createdAt: new Date().toISOString(), ...data, status: "Submitted", appStatus: "Submitted" }; db.applications.push(a); return clone(a); },
  async update(id, patch) { await delay(); const a = db.applications.find((x) => x.id === id); if (a) Object.assign(a, patch); return clone(a); },
  async updateStatus(id, status) { await delay(); const a = db.applications.find((x) => x.id === id); if (a) a.status = status; return clone(a); },
  async withdraw(id) { await delay(); const i = db.applications.findIndex((x) => x.id === id); if (i >= 0) db.applications.splice(i, 1); return { ok: true }; },
};

// ---------------- Profile ----------------
export const profileService = {
  async getFreelancer(userId) {
    await delay();
    const f = db.freelancers.find((x) => x.userId === userId);
    if (!f) return null;
    const user = db.users.find((u) => u.id === userId);
    return clone({ ...f, imageUrl: user?.imageUrl, email: user?.email, username: user?.username, memberSince: user?.createdAt });
  },
  async getClient(userId) {
    await delay();
    const c = db.clients.find((x) => x.userId === userId);
    if (!c) return null;
    const user = db.users.find((u) => u.id === userId);
    const openJobs = db.jobs.filter((j) => j.clientId === userId && j.status === "Open").length;
    const totalJobs = db.jobs.filter((j) => j.clientId === userId).length;
    return clone({ ...c, imageUrl: c.logo, email: user?.email, username: user?.username, openJobs, totalJobs });
  },
  async listFreelancers() { await delay(); return clone(db.freelancers); },
  async listClients() { await delay(); return clone(db.clients); },
  async updateFreelancer(userId, patch) { await delay(); const f = db.freelancers.find((x) => x.userId === userId); if (f) Object.assign(f, patch); return clone(f); },
  async updateClient(userId, patch) { await delay(); const c = db.clients.find((x) => x.userId === userId); if (c) Object.assign(c, patch); return clone(c); },
  async portfolio(userId) { await delay(); return clone(db.portfolio.filter((p) => p.userId === userId)); },
  async addPortfolioItem(userId, item) { await delay(); const p = { id: `p${Date.now()}`, userId, ...item }; db.portfolio.push(p); return clone(p); },
};


// ---------------- Reviews ----------------
// Business rules: only a Client can review, only on a Completed job they own,
// only the Freelancer whose application was Accepted, and only once per job.
const acceptedFreelancerId = (jobId) =>
  db.applications.find((a) => a.jobId === jobId && a.status === "Accepted")?.freelancerId || null;

// Average rating = historical baseline (seeded) blended with reviews left here.
const ratingBaseline = new Map();
const recalcFreelancerRating = (freelancerId) => {
  const f = db.freelancers.find((x) => x.userId === freelancerId);
  if (!f) return;
  if (!ratingBaseline.has(freelancerId)) {
    const seeded = db.reviews.filter((r) => r.to === freelancerId).length;
    ratingBaseline.set(freelancerId, { count: Math.max(0, (f.reviews || 0) - seeded), rating: f.rating || 0 });
  }
  const base = ratingBaseline.get(freelancerId);
  const rs = db.reviews.filter((r) => r.to === freelancerId);
  const count = base.count + rs.length;
  const sum = base.count * base.rating + rs.reduce((s, r) => s + r.rating, 0);
  f.reviews = count;
  f.rating = count ? Math.round((sum / count) * 10) / 10 : 0;
};

export const reviewService = {
  async list({ userId, to, from, jobId } = {}) {
    await delay();
    let out = clone(db.reviews);
    if (userId) out = out.filter((r) => r.to === userId || r.from === userId);
    if (to) out = out.filter((r) => r.to === to);
    if (from) out = out.filter((r) => r.from === from);
    if (jobId) out = out.filter((r) => r.jobId === jobId);
    return out.map((r) => ({
      ...r,
      job: db.jobs.find((j) => j.id === r.jobId) || null,
      author: db.users.find((u) => u.id === r.from) || null,
      target: db.users.find((u) => u.id === r.to) || null,
      targetName: db.freelancers.find((f) => f.userId === r.to)?.name || null,
    }));
  },
  async getByJob(jobId) { await delay(120); const r = db.reviews.find((x) => x.jobId === jobId); return r ? clone(r) : null; },

  // Completed jobs owned by the client that have an accepted freelancer,
  // annotated with the review already left (if any).
  async reviewableJobs(clientId) {
    await delay();
    return db.jobs
      .filter((j) => j.clientId === clientId && j.status === "Completed")
      .map((j) => {
        const freelancerId = acceptedFreelancerId(j.id);
        if (!freelancerId) return null;
        const f = db.freelancers.find((x) => x.userId === freelancerId);
        return clone({
          job: j,
          freelancerId,
          freelancerName: f?.name || db.users.find((u) => u.id === freelancerId)?.username || "Freelancer",
          review: db.reviews.find((r) => r.jobId === j.id) || null,
        });
      })
      .filter(Boolean);
  },

  // Returns { allowed, reason, freelancerId, review }
  async eligibility({ jobId, clientId }) {
    await delay(120);
    const job = db.jobs.find((j) => j.id === jobId);
    if (!job) return { allowed: false, reason: "Job not found" };
    if (job.clientId !== clientId) return { allowed: false, reason: "You can only review your own jobs" };
    if (job.status !== "Completed") return { allowed: false, reason: "This job is not completed yet" };
    const freelancerId = acceptedFreelancerId(jobId);
    if (!freelancerId) return { allowed: false, reason: "No freelancer completed this job" };
    const existing = db.reviews.find((r) => r.jobId === jobId);
    if (existing) return { allowed: false, reason: "You already reviewed this job", freelancerId, review: clone(existing) };
    return { allowed: true, freelancerId };
  },

  async create({ jobId, from, rating, text }) {
    await delay();
    const { allowed, reason, freelancerId } = await this.eligibility({ jobId, clientId: from });
    if (!allowed) throw new Error(reason || "You cannot review this job");
    if (!(rating >= 1 && rating <= 5)) throw new Error("Rating must be between 1 and 5");
    if (!text || !text.trim()) throw new Error("Comment is required");
    const r = { id: `r${Date.now()}`, jobId, from, to: freelancerId, rating, text: text.trim(), createdAt: new Date().toISOString() };
    db.reviews.push(r);
    recalcFreelancerRating(freelancerId);
    return clone(r);
  },
};


// ---------------- Bookmarks ----------------
export const bookmarkService = {
  async list(userId) { await delay(); return clone(db.bookmarks.filter((b) => b.userId === userId)).map((b) => ({ ...b, job: db.jobs.find((j) => j.id === b.jobId) })); },
  async toggle(userId, jobId) { await delay(); const i = db.bookmarks.findIndex((b) => b.userId === userId && b.jobId === jobId); if (i >= 0) { db.bookmarks.splice(i, 1); return { bookmarked: false }; } db.bookmarks.push({ id: `b${Date.now()}`, userId, jobId, createdAt: new Date().toISOString() }); return { bookmarked: true }; },
  async isBookmarked(userId, jobId) { await delay(80); return db.bookmarks.some((b) => b.userId === userId && b.jobId === jobId); },
};

// ---------------- Notifications ----------------
export const notificationService = {
  async list(userId) { await delay(); return clone(db.notifications.filter((n) => n.userId === userId)); },
  async markRead(id) { await delay(120); const n = db.notifications.find((x) => x.id === id); if (n) n.read = true; return clone(n); },
  async markAllRead(userId) { await delay(); db.notifications.forEach((n) => { if (n.userId === userId) n.read = true; }); return { ok: true }; },
};

// ---------------- Dashboard ----------------
export const dashboardService = {
  async freelancerStats(userId) {
    await delay();
    const apps = db.applications.filter((a) => a.freelancerId === userId);
    return {
      totalApplications: apps.length,
      accepted: apps.filter((a) => a.status === "Accepted").length,
      pending: apps.filter((a) => a.status === "Pending").length,
      earnings: apps.filter((a) => a.status === "Accepted").reduce((s, a) => s + a.bid, 0),
      chart: db.chartData,
    };
  },
  async clientStats(userId) {
    await delay();
    const myJobs = db.jobs.filter((j) => j.clientId === userId);
    const myApps = db.applications.filter((a) => myJobs.some((j) => j.id === a.jobId));
    return {
      totalJobs: myJobs.length,
      openJobs: myJobs.filter((j) => j.status === "Open").length,
      applications: myApps.length,
      spend: myApps.filter((a) => a.status === "Accepted").reduce((s, a) => s + a.bid, 0),
      chart: db.chartData,
    };
  },
  async adminStats() {
    await delay();
    return {
      users: db.users.length + 12500,
      jobs: db.jobs.length + 1500,
      applications: db.applications.length + 4800,
      revenue: 245000,
      chart: db.chartData,
    };
  },
};

// ---------------- Admin ----------------
export const adminService = {
  async users() { await delay(); return clone(db.users); },
  async jobs() { await delay(); return clone(db.jobs); },
  async pendingJobs() { await delay(); return clone(db.jobs.filter((j) => j.status === "Pending")); },
  async approveJob(id) { return jobService.approve(id); },
  async rejectJob(id) { return jobService.reject(id); },
  async categories() { await delay(); return clone(db.categories); },
  async skills() { await delay(); return clone(db.skills.map((s) => ({ id: s, name: s }))); },
  async tags() { await delay(); return clone(db.tags.map((t) => ({ id: t, name: t }))); },
  async createCategory(name) { await delay(); const c = { id: `c${Date.now()}`, name, slug: name.toLowerCase().replace(/\s+/g, "-"), icon: "LayoutList", jobs: 0 }; db.categories.push(c); return clone(c); },
  async updateCategory(id, name) { await delay(); const c = db.categories.find((x) => x.id === id); if (c) c.name = name; return clone(c); },
  async deleteCategory(id) { await delay(); const i = db.categories.findIndex((x) => x.id === id); if (i >= 0) db.categories.splice(i, 1); return { ok: true }; },
  async createSkill(name) { await delay(); if (!db.skills.includes(name)) db.skills.push(name); return { id: `s${db.skills.indexOf(name)}`, name }; },
  async updateSkill(id, name) { await delay(); const i = db.skills.indexOf(id); if (i >= 0) db.skills[i] = name; return { id: `s${i}`, name }; },
  async deleteSkill(name) { await delay(); const i = db.skills.indexOf(name); if (i >= 0) db.skills.splice(i, 1); return { ok: true }; },
  async createTag(name) { await delay(); if (!db.tags.includes(name)) db.tags.push(name); return { id: `t${db.tags.indexOf(name)}`, name }; },
  async updateTag(id, name) { await delay(); const i = db.tags.indexOf(id); if (i >= 0) db.tags[i] = name; return { id: `t${i}`, name }; },
  async deleteTag(name) { await delay(); const i = db.tags.indexOf(name); if (i >= 0) db.tags.splice(i, 1); return { ok: true }; },
  async deleteUser(id) { await delay(); const i = db.users.findIndex((u) => u.id === id); if (i >= 0) db.users.splice(i, 1); return { ok: true }; },
  async deleteJob(id) { return jobService.remove(id); },
};

// ---------------- Users (public) ----------------
export const userService = {
  async freelancer(userId) { return profileService.getFreelancer(userId); },
  async client(userId) { return profileService.getClient(userId); },
  async byId(userId) {
    await delay(120);
    const u = db.users.find((x) => x.id === userId);
    return u ? { ...u, password: undefined } : null;
  },
  async search(q) {
    await delay();
    const needle = (q || "").toLowerCase();
    const f = db.freelancers.filter((x) => (x.name + x.title + x.bio + x.skills.join(",")).toLowerCase().includes(needle));
    const c = db.clients.filter((x) => (x.companyName + x.companyDetails).toLowerCase().includes(needle));
    return { freelancers: clone(f), clients: clone(c) };
  },
};

// ---------------- Static ----------------
export const staticData = {
  categories: () => Promise.resolve(clone(db.categories)),
  skills: () => Promise.resolve(clone(db.skills)),
  tags: () => Promise.resolve(clone(db.tags)),
  testimonials: () => Promise.resolve(clone(db.testimonials)),
  stats: () => Promise.resolve(clone(db.stats)),
  topFreelancers: () => Promise.resolve(clone(db.freelancers.slice(0, 4))),
  topClients: () => Promise.resolve(clone(db.clients.map((c) => ({
    ...c,
    openJobs: db.jobs.filter((j) => j.clientId === c.userId && j.status === "Open").length,
    totalJobs: db.jobs.filter((j) => j.clientId === c.userId).length,
  })))),
  featuredJobs: () => Promise.resolve(clone(db.jobs.slice(0, 6))),
};

