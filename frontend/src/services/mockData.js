// Central mock database for the marketplace frontend.
const IMG = (seed) => `https://i.pravatar.cc/200?u=${seed}`;
const COVER = (seed) => `https://picsum.photos/seed/${seed}/800/400`;

export const categories = [
  { id: "c1", name: "Web Development", slug: "web-development", icon: "Code2", jobs: 428 },
  { id: "c2", name: "Design & Creative", slug: "design", icon: "Palette", jobs: 312 },
  { id: "c3", name: "Writing", slug: "writing", icon: "PenLine", jobs: 187 },
  { id: "c4", name: "Marketing", slug: "marketing", icon: "Megaphone", jobs: 224 },
  { id: "c5", name: "Mobile Development", slug: "mobile", icon: "Smartphone", jobs: 156 },
  { id: "c6", name: "Data Science", slug: "data", icon: "BarChart3", jobs: 98 },
  { id: "c7", name: "DevOps", slug: "devops", icon: "Server", jobs: 74 },
  { id: "c8", name: "Video & Animation", slug: "video", icon: "Video", jobs: 143 },
];

export const skills = [
  "React","Next.js","TypeScript","Node.js","Python","Django","Figma","UI/UX",
  "Tailwind CSS","GraphQL","AWS","Docker","Kubernetes","PostgreSQL","MongoDB",
  "Swift","Kotlin","Flutter","SEO","Copywriting","Illustration","Motion","Rust","Go"
];

export const tags = ["Urgent","Remote","Long-term","Featured","Startup","Enterprise","Fixed price","Hourly"];

export const users = [
  { id: "u1", username: "alexchen", email: "alex@studio.com", role: "Freelancer", imageUrl: IMG("alex"), createdAt: "2024-06-01" },
  { id: "u2", username: "sarahdev", email: "sarah@dev.io", role: "Freelancer", imageUrl: IMG("sarah"), createdAt: "2024-04-11" },
  { id: "u3", username: "acme", email: "hello@acme.co", role: "Client", imageUrl: IMG("acme"), createdAt: "2024-01-22" },
  { id: "u4", username: "northwind", email: "team@northwind.io", role: "Client", imageUrl: IMG("north"), createdAt: "2024-02-15" },
  { id: "u5", username: "admin", email: "admin@marketplace.io", role: "Admin", imageUrl: IMG("admin"), createdAt: "2023-11-01" },
  { id: "u6", username: "mayapatel", email: "maya@designs.co", role: "Freelancer", imageUrl: IMG("maya"), createdAt: "2024-07-18" },
  { id: "u7", username: "davidkim", email: "david@code.dev", role: "Freelancer", imageUrl: IMG("david"), createdAt: "2024-03-04" },
];

export const freelancers = [
  { userId: "u1", name: "Alex Chen", title: "Full-Stack Engineer", bio: "8+ years shipping product with React, Node, and Postgres. Ex-Stripe, ex-Linear.", averageRate: 85, link: "alexchen.dev", skills: ["React","Node.js","TypeScript","PostgreSQL","AWS"], rating: 4.9, reviews: 128, completed: 92, location: "San Francisco" },
  { userId: "u2", name: "Sarah Dev", title: "Backend & DevOps", bio: "Scalable systems, Kubernetes, and observability specialist. Loves clean APIs.", averageRate: 95, link: "sarah.io", skills: ["Node.js","Docker","Kubernetes","AWS","Go"], rating: 4.8, reviews: 76, completed: 54, location: "Berlin" },
  { userId: "u6", name: "Maya Patel", title: "Product Designer", bio: "UX for SaaS and fintech. Design systems, prototyping, research.", averageRate: 72, link: "maya.design", skills: ["Figma","UI/UX","Illustration"], rating: 5.0, reviews: 214, completed: 148, location: "London" },
  { userId: "u7", name: "David Kim", title: "Mobile Engineer", bio: "iOS + Android with Swift, Kotlin, and Flutter. Delivered 30+ apps.", averageRate: 88, link: "davidkim.app", skills: ["Swift","Kotlin","Flutter"], rating: 4.7, reviews: 61, completed: 44, location: "Seoul" },
];

export const clients = [
  { userId: "u3", companyName: "Acme Inc.", companyDetails: "SaaS platform for logistics teams. Series B, 60 people.", logo: IMG("acme"), location: "USA", rating: 4.9, reviews: 47, memberSince: "2024", hireRate: 92, responseTime: "under 2 hours", jobsPosted: 32 },
  { userId: "u4", companyName: "Northwind Labs", companyDetails: "AI-first analytics for e-commerce.", logo: IMG("north"), location: "Berlin, DE", rating: 4.7, reviews: 28, memberSince: "2024", hireRate: 78, responseTime: "under 6 hours", jobsPosted: 19 },
];

const now = new Date();
const daysFromNow = (d) => new Date(now.getTime() + d * 86400000).toISOString();

const projectTypes = ["Fixed price", "Hourly", "Ongoing"];
const experienceLevels = ["Entry", "Intermediate", "Expert"];

export const jobs = Array.from({ length: 18 }).map((_, i) => {
  const cat = categories[i % categories.length];
  const clientId = i % 2 === 0 ? "u3" : "u4";
  const statuses = ["Open","Open","Open","In Progress","Closed"];
  const skillPick = skills.slice(i % 10, (i % 10) + 4);
  return {
    id: `j${i + 1}`,
    title: [
      "Build a React dashboard for logistics analytics",
      "Redesign onboarding flow for SaaS product",
      "Implement Stripe billing + subscription tiers",
      "Migrate monolith to microservices on Kubernetes",
      "Design system for fintech mobile app",
      "SEO overhaul and content strategy",
      "Build native iOS app for booking",
      "ML pipeline for customer churn prediction",
      "Integrate GraphQL API layer",
      "Motion graphics for product launch video",
      "Full brand identity for AI startup",
      "Realtime chat feature with websockets",
      "Framer landing page for a new product",
      "Data warehouse setup with dbt + Snowflake",
      "Copywriting for a series of email campaigns",
      "Automate deployment pipeline with GitHub Actions",
      "Convert Figma designs to production React",
      "Performance audit for large Next.js app",
    ][i],
    description: "We're looking for an experienced professional to help deliver a polished, production-ready result. You'll work directly with our product team, own scope end-to-end, and ship on a tight but reasonable timeline. Prior work with similar teams strongly preferred.",
    budget: 500 + ((i * 350) % 8000),
    deadline: daysFromNow(7 + i * 3),
    status: statuses[i % statuses.length],
    clientId,
    categoryId: cat.id,
    tags: [tags[i % tags.length], tags[(i + 3) % tags.length]],
    requiredSkills: skillPick,
    attachments: [],
    cover: COVER(`job${i}`),
    createdAt: daysFromNow(-i - 1),
    proposals: 3 + ((i * 7) % 30),
    projectType: projectTypes[i % projectTypes.length],
    experienceLevel: experienceLevels[i % experienceLevels.length],
  };
});

export const applications = [
  { id: "a1", jobId: "j1", freelancerId: "u1", coverLetter: "I've built 3 dashboards like this at scale and can start immediately.", bid: 4200, timeline: "3 weeks", status: "Pending", createdAt: daysFromNow(-2) },
  { id: "a2", jobId: "j2", freelancerId: "u6", coverLetter: "Redesigned similar onboarding for two SaaS companies — happy to share case studies.", bid: 2800, timeline: "2 weeks", status: "Shortlisted", createdAt: daysFromNow(-4) },
  { id: "a3", jobId: "j3", freelancerId: "u1", coverLetter: "Deep Stripe experience including tax and metered billing.", bid: 3600, timeline: "10 days", status: "Accepted", createdAt: daysFromNow(-6) },
  { id: "a4", jobId: "j5", freelancerId: "u6", coverLetter: "Design systems for fintech is my niche.", bid: 5400, timeline: "5 weeks", status: "Rejected", createdAt: daysFromNow(-8) },
  { id: "a5", jobId: "j7", freelancerId: "u7", coverLetter: "Native iOS with Swift + SwiftUI — recent portfolio available.", bid: 6200, timeline: "6 weeks", status: "Pending", createdAt: daysFromNow(-1) },
];

export const reviews = [
  { id: "r1", jobId: "j3", from: "u3", to: "u1", rating: 5, text: "Alex delivered incredibly clean code ahead of schedule. Would hire again.", createdAt: daysFromNow(-10) },
  { id: "r2", jobId: "j2", from: "u3", to: "u6", rating: 5, text: "Maya's designs elevated our product. Best hire this year.", createdAt: daysFromNow(-14) },
  { id: "r3", jobId: "j1", from: "u4", to: "u2", rating: 4, text: "Great technical work, communication could be slightly quicker.", createdAt: daysFromNow(-30) },
];

export const bookmarks = [
  { id: "b1", userId: "u1", jobId: "j5", createdAt: daysFromNow(-1) },
  { id: "b2", userId: "u1", jobId: "j8", createdAt: daysFromNow(-3) },
];

export const notifications = [
  { id: "n1", userId: "u1", title: "New job matches your skills", body: "3 new React roles were posted today.", read: false, createdAt: daysFromNow(-0.1), type: "info" },
  { id: "n2", userId: "u1", title: "Application accepted", body: "Acme accepted your bid on 'Stripe billing'.", read: false, createdAt: daysFromNow(-0.5), type: "success" },
  { id: "n3", userId: "u1", title: "Payment received", body: "$3,600 for 'Stripe billing' released.", read: true, createdAt: daysFromNow(-2), type: "success" },
  { id: "n4", userId: "u3", title: "New application", body: "Alex Chen applied to 'React dashboard'.", read: false, createdAt: daysFromNow(-0.2), type: "info" },
];

export const portfolio = [
  { id: "p1", userId: "u1", title: "Realtime analytics dashboard", image: COVER("port1"), tags: ["React","D3"], url: "#" },
  { id: "p2", userId: "u1", title: "Billing platform for SaaS", image: COVER("port2"), tags: ["Node","Stripe"], url: "#" },
  { id: "p3", userId: "u6", title: "Fintech design system", image: COVER("port3"), tags: ["Figma","UI"], url: "#" },
];

export const testimonials = [
  { id: "t1", name: "Emma Roberts", role: "CEO, Northwind", text: "Hired 4 freelancers here — every single one exceeded expectations.", avatar: IMG("emma") },
  { id: "t2", name: "James Liu", role: "CTO, Acme Inc.", text: "The quality of talent and speed of matching is unmatched.", avatar: IMG("james") },
  { id: "t3", name: "Priya Shah", role: "Product Lead, Verve", text: "It's the only marketplace we use now. Frictionless from post to delivery.", avatar: IMG("priya") },
];

export const stats = {
  freelancers: 42800,
  clients: 12600,
  jobsPosted: 158900,
  paidOut: 24500000,
};

export const chartData = {
  jobsCreated: [12, 19, 15, 27, 22, 31, 38, 42, 35, 48, 52, 61],
  applications: [40, 55, 62, 78, 90, 110, 128, 142, 138, 165, 189, 210],
  revenue: [4200, 5100, 4800, 6300, 7100, 8800, 9500, 10200, 9800, 11400, 12800, 14500],
  labels: ["Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec"],
  categoriesPie: categories.slice(0, 6).map((c) => ({ name: c.name, value: c.jobs })),
};
