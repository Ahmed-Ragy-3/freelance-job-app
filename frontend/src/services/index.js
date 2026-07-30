import api from "./api";

// Helper Enum Normalizers
function normalizeRole(role) {
  if (role === 0 || role === "Admin") return "Admin";
  if (role === 1 || role === "Freelancer") return "Freelancer";
  if (role === 2 || role === "Client") return "Client";
  return role || "Guest";
}

function parseJobStatusEnum(val) {
  const map = {
    0: "Pending", 1: "Approved", 2: "Rejected",
    3: "In_Progress", 4: "Finished", 5: "Passed", 6: "Delayed"
  };
  return map[val] || val || "Pending";
}

function parseAppStatusEnum(val) {
  const map = {
    0: "Draft", 1: "In_Progress", 2: "Accepted",
    3: "Rejected", 4: "Withdrawn", 5: "JobDone"
  };
  return map[val] || val || "Submitted";
}

function normalizeUser(u) {
  if (!u) return null;
  const id = u.id || u.userId;
  return {
    id,
    userId: id,
    username: u.userName || u.username || "",
    userName: u.userName || u.username || "",
    email: u.email || "",
    role: normalizeRole(u.role),
    imageUrl: u.imageUrl || u.logo || `https://i.pravatar.cc/200?u=${id}`,
    createdAt: u.createdAt || new Date().toISOString(),
    isProfileComplete: u.isProfileComplete ?? true,
  };
}

function normalizeJob(j) {
  if (!j) return null;
  const statusStr = typeof j.jobStatus === "string" ? j.jobStatus : parseJobStatusEnum(j.jobStatus);
  const categories = j.categories ? j.categories.map((c) => (c.category ? c.category : c)) : [];
  const tags = j.tags ? j.tags.map((t) => (t.tag ? t.tag : t)) : [];
  const skills = j.skills ? j.skills.map((s) => (s.skill ? s.skill : s)) : [];

  return {
    id: j.id,
    title: j.title || "",
    description: j.description || "",
    budget: j.budget || 0,
    deadline: j.deadline || "",
    postedAt: j.postedAt || j.createdAt || new Date().toISOString(),
    createdAt: j.postedAt || j.createdAt || new Date().toISOString(),
    status: statusStr,
    jobStatus: statusStr,
    proposals: j.applicants || j.proposals || 0,
    applicants: j.applicants || 0,
    categories,
    categoryId: categories[0]?.id || j.categoryId,
    categoryIds: categories.map((c) => c.id),
    tags,
    skills,
    requiredSkills: skills.map((s) => s.name || s) || j.requiredSkills || [],
    client: j.client
      ? {
          userId: j.client.userId,
          companyName: j.client.companyName || "",
          companyDetails: j.client.companyDetails || "",
          logo: j.client.logo || "",
          imageUrl: j.client.logo || "",
        }
      : j.clientCompanyName
      ? { companyName: j.clientCompanyName }
      : null,
    clientId: j.client?.userId || j.clientId,
    attachments: j.attachments || [],
  };
}

function normalizeApplication(a) {
  if (!a) return null;
  const statusStr = typeof a.appStatus === "string" ? a.appStatus : parseAppStatusEnum(a.appStatus);
  return {
    id: a.applicationId || a.id,
    applicationId: a.applicationId || a.id,
    jobId: a.jobId,
    freelancerId: a.freelancerId,
    freelancerName: a.freelancerName || "",
    jobTitle: a.jobTitle || a.job?.title || "",
    jobBudget: a.jobBudget || a.job?.budget || 0,
    companyName: a.companyName || a.job?.client?.companyName || "",
    companyLogo: a.companyLogo || a.job?.client?.logo || "",
    coverLetter: a.coverLetter || "",
    bid: a.bid || 0,
    timeline: a.timeline || a.timelineDays || 0,
    status: statusStr,
    appStatus: statusStr,
    jobDeadline: a.jobDeadline,
    attachments: a.attachments || [],
    createdAt: a.submittedAt || a.createdAt || new Date().toISOString(),
    job: a.job ? normalizeJob(a.job) : (a.jobTitle ? { id: a.jobId, title: a.jobTitle, budget: a.jobBudget } : null),
  };
}

// ---------------- Auth Service ----------------
export const authService = {
  async login({ email, password }) {
    const res = await api.post("/auth/login", { email, password });
    const { token, userId, username, role } = res.data;
    if (token) {
      localStorage.setItem("mp_token", token);
    }
    const user = await authService.me();
    return { user: user || { id: userId, username, role: normalizeRole(role) }, session: { userId, role } };
  },

  async register({ username, email, password, role, companyName, bio }) {
    const payload = {
      username,
      email,
      password,
      role: role || "Freelancer",
      bio: bio || undefined,
      companyName: companyName || undefined,
    };
    const res = await api.post("/auth/register", payload);
    const { token, userId, role: userRole } = res.data;
    if (token) {
      localStorage.setItem("mp_token", token);
    }
    const user = await authService.me();
    return { user: user || { id: userId, username, role: normalizeRole(userRole) }, session: { userId, role: userRole } };
  },

  async logout() {
    localStorage.removeItem("mp_token");
    localStorage.removeItem("mp_user");
  },

  async me() {
    const token = localStorage.getItem("mp_token");
    if (!token) return null;
    try {
      const res = await api.get("/users/me");
      return normalizeUser(res.data);
    } catch {
      localStorage.removeItem("mp_token");
      return null;
    }
  },

  async verifyEmail() {
    return { ok: true };
  },
};

// ---------------- Job Service ----------------
export const jobService = {
  async list({ search = "", category = "", skills = [], status = "", minBudget = 0, maxBudget, sort = "newest", page = 1, pageSize = 9, includeAll = false, clientId } = {}) {
    const sortMap = { newest: 0, oldest: 1, budget: 2, applicants: 3, deadline: 4 };
    const params = {
      Search: search || undefined,
      CategoryId: category ? Number(category) : undefined,
      Status: status || undefined,
      MinBudget: minBudget || undefined,
      MaxBudget: maxBudget && maxBudget !== Infinity ? maxBudget : undefined,
      ClientId: clientId || undefined,
      SortBy: sortMap[sort] ?? 0,
      Page: page,
      PageSize: pageSize,
    };
    if (skills && skills.length > 0) {
      params.SkillIds = skills.map(Number).filter((n) => !isNaN(n));
    }

    const res = await api.get("/jobs", { params });
    const data = res.data;
    const items = (data.items || []).map(normalizeJob);
    return { items, total: data.totalCount || items.length, page, pageSize };
  },

  async get(id) {
    const res = await api.get(`/jobs/${id}`);
    return normalizeJob(res.data);
  },

  async similar(id) {
    try {
      const current = await this.get(id);
      const res = await this.list({ category: current?.categoryId, pageSize: 4 });
      return (res.items || []).filter((j) => String(j.id) !== String(id)).slice(0, 3);
    } catch {
      return [];
    }
  },

  async create(data) {
    const payload = {
      title: data.title,
      description: data.description,
      budget: Number(data.budget),
      deadline: data.deadline,
      categoryIds: data.categoryIds || (data.categoryId ? [Number(data.categoryId)] : []),
      skillIds: (data.skillIds || data.requiredSkills || []).map(Number).filter((n) => !isNaN(n)),
      tagIds: (data.tagIds || []).map(Number).filter((n) => !isNaN(n)),
    };
    const res = await api.post("/jobs", payload);
    return res.data;
  },

  async update(id, patch) {
    const payload = {
      title: patch.title,
      description: patch.description,
      budget: Number(patch.budget),
      deadline: patch.deadline,
      categoryIds: patch.categoryIds || (patch.categoryId ? [Number(patch.categoryId)] : []),
      skillIds: (patch.skillIds || patch.requiredSkills || []).map(Number).filter((n) => !isNaN(n)),
      tagIds: (patch.tagIds || []).map(Number).filter((n) => !isNaN(n)),
      jobStatus: patch.jobStatus || patch.status || "Pending",
    };
    const res = await api.put(`/jobs/${id}`, payload);
    return res.data;
  },

  async remove(id) {
    await api.delete(`/jobs/${id}`);
    return { ok: true };
  },

  async byClient(clientId) {
    const res = await api.get("/jobs/client");
    const items = (res.data.items || []).map(normalizeJob);
    return items;
  },

  async approve(id) {
    const res = await api.put(`/admin/jobs/${id}/approve`);
    return res.data;
  },

  async reject(id) {
    const res = await api.put(`/admin/jobs/${id}/reject`);
    return res.data;
  },

  async complete(id) {
    const res = await api.put(`/jobs/${id}/finish`);
    return res.data;
  },

  async finish(id) {
    const res = await api.put(`/jobs/${id}/finish`);
    return res.data;
  },

  async hire(jobId, freelancerId) {
    const res = await api.put(`/jobs/${jobId}/applications/${freelancerId}/hire`);
    return res.data;
  },

  async getApplications(jobId) {
    const res = await api.get(`/jobs/${jobId}/applications`);
    return (res.data || []).map(normalizeApplication);
  },
};

// ---------------- Application Service ----------------
export const applicationService = {
  async list({ freelancerId, jobId } = {}) {
    if (jobId) {
      const res = await api.get(`/jobs/${jobId}/applications`);
      return (res.data || []).map(normalizeApplication);
    }
    const res = await api.get("/freelancer/applications");
    return (res.data || []).map(normalizeApplication);
  },

  async get(id) {
    const apps = await this.list();
    return apps.find((a) => String(a.id) === String(id)) || null;
  },

  async getByFreelancerAndJob(freelancerId, jobId) {
    try {
      const apps = await this.list();
      return apps.find((a) => String(a.jobId) === String(jobId)) || null;
    } catch {
      return null;
    }
  },

  async create(data) {
    if (data instanceof FormData) {
      const res = await api.post("/freelancer/applications", data, {
        headers: { "Content-Type": "multipart/form-data" },
      });
      return normalizeApplication(res.data);
    }
    const formData = new FormData();
    formData.append("jobId", data.jobId);
    formData.append("coverLetter", data.coverLetter);
    formData.append("bid", data.bid);
    formData.append("timeline", data.timeline || data.timelineDays || 1);
    if (data.attachments && data.attachments.length) {
      data.attachments.forEach((file) => formData.append("attachments", file));
    }
    const res = await api.post("/freelancer/applications", formData, {
      headers: { "Content-Type": "multipart/form-data" },
    });
    return normalizeApplication(res.data);
  },

  async saveDraft(data) {
    const payload = {
      jobId: Number(data.jobId),
      coverLetter: data.coverLetter || "",
      bid: Number(data.bid || 0),
      timeline: Number(data.timeline || data.timelineDays || 0),
    };
    const res = await api.post("/freelancer/applications/draft", payload);
    return normalizeApplication(res.data);
  },

  async submitDraft(jobId, data) {
    const payload = {
      jobId: Number(jobId),
      coverLetter: data.coverLetter,
      bid: Number(data.bid),
      timeline: Number(data.timeline || data.timelineDays || 1),
    };
    const res = await api.put(`/freelancer/applications/job/${jobId}/submit-application`, payload);
    return normalizeApplication(res.data);
  },

  async submitWork(jobId) {
    const res = await api.put(`/freelancer/applications/job/${jobId}/submit`);
    return normalizeApplication(res.data);
  },

  async withdraw(jobId) {
    await api.put(`/freelancer/applications/job/${jobId}/withdraw`);
    return { ok: true };
  },
};

// ---------------- Profile & User Service ----------------
export const profileService = {
  async getFreelancer(userId) {
    try {
      const res = await api.get(`/users/freelancer/${userId}`);
      const data = res.data;
      return {
        userId: data.userId,
        id: data.userId,
        username: data.userName || "",
        name: data.userName || "",
        email: data.email || "",
        imageUrl: data.imageUrl || `https://i.pravatar.cc/200?u=${data.userId}`,
        bio: data.bio || "",
        link: data.link || "",
        avgRate: data.avgRate || 0,
        rating: data.avgRate || 0,
        skills: data.skills || [],
      };
    } catch {
      return null;
    }
  },

  async getClient(userId) {
    try {
      const res = await api.get(`/users/client/${userId}`);
      const data = res.data;
      return {
        userId: data.userId,
        id: data.userId,
        username: data.userName || "",
        email: data.email || "",
        companyName: data.companyName || "",
        companyDetails: data.companyDetails || "",
        logo: data.logo || "",
        imageUrl: data.logo || data.imageUrl || `https://i.pravatar.cc/200?u=${data.userId}`,
      };
    } catch {
      return null;
    }
  },

  async updateFreelancer(userId, patch) {
    const payload = {
      bio: patch.bio || "",
      link: patch.link || "",
      skills: (patch.skills || []).map((s) => ({
        skillId: typeof s === "object" ? s.skillId || s.id : s,
        experienceLevel: s.experienceLevel || 1,
      })),
    };
    const res = await api.put(`/users/freelancer/${userId}`, payload);
    return res.data;
  },

  async updateClient(userId, patch) {
    const payload = {
      companyName: patch.companyName || "",
      companyDetails: patch.companyDetails || "",
      logo: patch.logo || "",
    };
    const res = await api.put(`/users/client/${userId}`, payload);
    return res.data;
  },

  async updateUser(patch) {
    const payload = {
      userName: patch.username || patch.userName,
      imageUrl: patch.imageUrl,
    };
    const res = await api.put("/users/me", payload);
    return normalizeUser(res.data);
  },

  async portfolio(userId) {
    return [];
  },
};

// ---------------- Review Service ----------------
export const reviewService = {
  async list({ userId, to, jobId } = {}) {
    if (jobId) {
      const res = await api.get(`/jobs/${jobId}/reviews`);
      return (res.data || []).map((r) => ({
        id: r.id,
        jobId: r.jobId,
        from: r.reviewerId,
        to: r.revieweeId,
        rating: r.rate,
        text: r.comment || "",
        createdAt: r.createdAt,
        authorName: r.reviewerName,
        targetName: r.revieweeName,
        jobTitle: r.jobTitle,
      }));
    }

    const targetId = userId || to;
    if (!targetId) return [];
    try {
      const res = await api.get(`/reviews/user/${targetId}`);
      const data = res.data;
      return (data.reviews || []).map((r) => ({
        id: r.id,
        jobId: r.jobId,
        from: r.reviewerId,
        to: r.revieweeId,
        rating: r.rate,
        text: r.comment || "",
        createdAt: r.createdAt,
        authorName: r.reviewerName,
        targetName: r.revieweeName,
        jobTitle: r.jobTitle,
      }));
    } catch {
      return [];
    }
  },

  async getByJob(jobId) {
    const res = await api.get(`/jobs/${jobId}/reviews`);
    const reviews = res.data || [];
    return reviews.length > 0 ? reviews[0] : null;
  },

  async create({ jobId, rating, text, comment }) {
    const payload = {
      rate: Number(rating),
      comment: text || comment || "",
    };
    const res = await api.post(`/jobs/${jobId}/reviews`, payload);
    return res.data;
  },

  async reviewableJobs(clientId) {
    try {
      const jobs = await jobService.byClient(clientId);
      return jobs
        .filter((j) => j.status === "Finished" || j.status === "Completed")
        .map((j) => ({
          job: j,
          freelancerId: j.freelancerId,
          freelancerName: j.freelancerName || "Freelancer",
          review: null,
        }));
    } catch {
      return [];
    }
  },
};

// ---------------- Bookmark Service ----------------
export const bookmarkService = {
  async list() {
    const res = await api.get("/bookmark");
    const items = (res.data?.items || []).map(normalizeJob);
    return items.map((j) => ({ id: `b_${j.id}`, jobId: j.id, job: j }));
  },

  async save(jobId) {
    await api.post(`/bookmark/${jobId}`);
    return { bookmarked: true };
  },

  async remove(jobId) {
    await api.delete(`/bookmark/${jobId}`);
    return { bookmarked: false };
  },

  async toggle(userId, jobId) {
    try {
      await this.save(jobId);
      return { bookmarked: true };
    } catch {
      await this.remove(jobId);
      return { bookmarked: false };
    }
  },

  async isBookmarked(userId, jobId) {
    try {
      const list = await this.list();
      return list.some((b) => String(b.jobId) === String(jobId));
    } catch {
      return false;
    }
  },
};

// ---------------- Notification Service ----------------
export const notificationService = {
  async list() {
    const res = await api.get("/notifications");
    return (res.data || []).map((n) => ({
      id: n.id,
      title: n.title,
      createdAt: n.createdAt,
      read: n.read,
    }));
  },

  async getUnreadCount() {
    const res = await api.get("/notifications/unread-count");
    return res.data?.unreadCount || 0;
  },

  async markRead(id) {
    await api.patch(`/notifications/${id}/read`);
    return { ok: true };
  },

  async markAllRead() {
    await api.patch("/notifications/read-all");
    return { ok: true };
  },

  async delete(id) {
    await api.delete(`/notifications/${id}`);
    return { ok: true };
  },
};

// ---------------- Dashboard Service ----------------
export const dashboardService = {
  async freelancerStats() {
    const res = await api.get("/freelancer/dashboard");
    const d = res.data;
    return {
      totalApplications: d.activeApplicationsCount,
      accepted: d.completedJobsCount,
      pending: d.activeApplicationsCount,
      earnings: d.totalEarnings,
      activeJobs: d.activeJobsCount,
      bookmarks: d.totalBookmarksCount,
      rating: d.avgRating,
      unreadNotifications: d.unreadNotificationsCount,
      recentApplications: d.recentApplications || [],
      chart: [],
    };
  },

  async adminStats() {
    const res = await api.get("/admin/overview/stats");
    const d = res.data;
    return {
      users: d.totalUsers,
      jobs: d.totalJobs,
      applications: d.totalApplications,
      revenue: d.totalRevenue,
      chart: [],
    };
  },

  async clientStats(userId) {
    const jobs = await jobService.byClient(userId);
    const openJobs = jobs.filter((j) => j.status === "Approved" || j.status === "In_Progress").length;
    return {
      totalJobs: jobs.length,
      openJobs,
      applications: jobs.reduce((sum, j) => sum + (j.proposals || 0), 0),
      spend: jobs.filter((j) => j.status === "Finished").reduce((sum, j) => sum + j.budget, 0),
      chart: [],
    };
  },
};

// ---------------- Admin Service ----------------
export const adminService = {
  async users() {
    const res = await api.get("/admin/users");
    return (res.data || []).map((u) => ({
      id: u.id,
      username: u.userName,
      email: u.email,
      role: normalizeRole(u.role),
      isSuspended: u.isSuspended,
      createdAt: u.createdAt,
    }));
  },

  async setSuspension(userId, isSuspended) {
    const res = await api.put(`/admin/users/${userId}/suspension`, { isSuspended });
    return res.data;
  },

  async pendingJobs() {
    const res = await api.get("/admin/jobs/pending");
    return (res.data || []).map(normalizeJob);
  },

  async approveJob(id) {
    return jobService.approve(id);
  },

  async rejectJob(id) {
    return jobService.reject(id);
  },

  async tags() {
    const res = await api.get("/admin/tags");
    return res.data || [];
  },

  async createTag(name) {
    const res = await api.post("/admin/tags", { name });
    return res.data;
  },

  async deleteTag(id) {
    await api.delete(`/admin/tags/${id}`);
    return { ok: true };
  },

  async skills() {
    const res = await api.get("/admin/skills");
    return res.data || [];
  },

  async createSkill(name) {
    const res = await api.post("/admin/skills", { name });
    return res.data;
  },

  async deleteSkill(id) {
    await api.delete(`/admin/skills/${id}`);
    return { ok: true };
  },
};

// ---------------- User Service (Public Search & Profiles) ----------------
export const userService = {
  async freelancer(userId) {
    return profileService.getFreelancer(userId);
  },
  async client(userId) {
    return profileService.getClient(userId);
  },
  async search(q) {
    const res = await api.get(`/home/search?searchTerm=${encodeURIComponent(q || "")}`);
    const data = res.data || {};
    return {
      jobs: (data.jobs || []).map(normalizeJob),
      freelancers: data.freelancers || [],
      clients: data.clients || [],
      categories: data.categories || [],
    };
  },
};

// ---------------- Static & Home Data Service ----------------
export const staticData = {
  async stats() {
    const res = await api.get("/home/stats");
    return res.data;
  },
  async categories() {
    try {
      const stats = await this.stats();
      return stats.topCategories || [];
    } catch {
      return [];
    }
  },
  async skills() {
    return adminService.skills();
  },
  async tags() {
    return adminService.tags();
  },
  async topFreelancers() {
    const stats = await this.stats();
    return (stats.topFreelancers || []).map(f => ({
      ...f,
      id: f.userId,
      name: f.userName,
      avatar: f.imageUrl,
      rating: f.avgRate,
      skills: f.skills || [],
      title: f.title || "Freelancer",
      location: f.location || "Remote",
      bio: f.bio || "",
      completed: f.completed || 0,
      averageRate: f.avgRate || 0
    }));
  },
  async topClients() {
    const stats = await this.stats();
    return (stats.topClients || []).map(c => ({
      ...c,
      userId: c.userId,
      companyName: c.companyName,
      companyDetails: c.companyDetails || "",
      logo: c.logo || c.imageUrl,
      location: c.location || "Worldwide",
      rating: c.rating || 5,
      reviews: c.reviews || 0,
      openJobs: c.openJobs || 0,
      responseTime: c.responseTime || "1 day"
    }));
  },
  async featuredJobs() {
    const stats = await this.stats();
    return (stats.topJobs || []).map(normalizeJob);
  },
  async testimonials() {
    return [
      {
        id: 1,
        text: "Workly has completely transformed how we build our team. The quality of freelancers is unmatched.",
        name: "Sarah Jenkins",
        role: "CTO at TechCorp",
        avatar: ""
      },
      {
        id: 2,
        text: "I found an amazing designer within 2 hours of posting my job. Highly recommended!",
        name: "Michael Chen",
        role: "Founder at StartupX",
        avatar: ""
      },
      {
        id: 3,
        text: "The platform is intuitive and the payment protection gives us great peace of mind.",
        name: "Emily Rodriguez",
        role: "Marketing Director",
        avatar: ""
      }
    ];
  },
};
