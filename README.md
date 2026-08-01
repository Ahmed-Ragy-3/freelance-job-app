# Workly

Workly is a full-stack freelance marketplace that connects clients with skilled freelancers. The platform supports job discovery, proposal submission, contract lifecycle management, reviews, and administrative oversight through role-based dashboards for freelancers, clients, and administrators.

The application consists of an ASP.NET Core REST API with real-time notifications and a React single-page frontend built with TanStack Router.

---

## Key Features

### Public Marketplace
- Landing page with platform statistics, category browsing, and global search
- Paginated job listings with filters for keyword, category, status, budget range, skills, and sort order
- Job detail pages with required skills, attachments, client information, and similar job recommendations
- Freelancer and company profile pages
- User registration and JWT-based authentication for Freelancer and Client roles

### Freelancer Workspace
- Dashboard with application metrics, earnings summary, and activity charts
- Job application submission with bid amount, timeline, cover letter, and optional file attachments
- Application tracking with status filters (Pending, Shortlisted, Accepted, Rejected, Withdrawn)
- Profile and portfolio management
- Saved job bookmarks
- Review history and bidirectional review submission after job completion

### Client Workspace
- Dashboard overview for posted jobs and hiring activity
- Job creation with categories, tags, required skills, budget, and deadline
- Job management table with edit, delete, and application review actions
- Application review and acceptance workflow for individual job postings
- Company profile management
- Post-completion review submission for freelancers

### Administration
- Platform overview with user, job, application, and revenue statistics
- User management with account suspension controls
- Job moderation workflow (approve, reject, delete pending postings)
- CRUD management for categories, skills, and tags
- Analytics reports with charts for activity, revenue, category distribution, and job creation trends

### Platform Services
- JWT authentication with BCrypt password hashing and role-based authorization policies
- Real-time notifications delivered via SignalR (`/notificationHub`)
- File uploads for job attachments and application proposals via Cloudinary
- Background service for automatic job status updates on overdue or expired deadlines
- Database seeding with sample users, jobs, applications, and taxonomy data on startup
- Swagger UI available at the API root during development

---

## Tech Stack and System Requirements

### Backend
| Component | Technology |
|-----------|------------|
| Runtime | .NET 9.0 |
| Framework | ASP.NET Core Web API |
| ORM | Entity Framework Core 9 |
| Database | Microsoft SQL Server (LocalDB supported) |
| Authentication | JWT Bearer (BCrypt password hashing) |
| Real-time | ASP.NET Core SignalR |
| File Storage | Cloudinary |
| API Documentation | Swashbuckle (Swagger) |

### Frontend
| Component | Technology |
|-----------|------------|
| UI Library | React 19 |
| Routing | TanStack Router / TanStack Start |
| Build Tool | Vite 8 |
| Styling | Tailwind CSS 4 |
| Components | Radix UI, shadcn/ui patterns |
| Data Fetching | TanStack Query, Axios |
| Real-time Client | @microsoft/signalr |
| Charts | Chart.js, Recharts |
| Forms | React Hook Form, Zod |

### Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (LTS recommended) with npm
- SQL Server or SQL Server LocalDB (included with Visual Studio on Windows)
- Cloudinary account (required for file upload features; configure via user secrets or environment variables)

---

## Project Architecture

```
freelance-job-app/
├── backend/                    # ASP.NET Core Web API
│   ├── Auth/                   # JWT helpers and SignalR user ID provider
│   ├── Controllers/            # REST API endpoints
│   ├── Data/                   # EF Core DbContext and database seeder
│   ├── DTOs/                   # Request and response data transfer objects
│   ├── FileUpload/             # Cloudinary integration and file controller
│   ├── Migrations/             # EF Core database migrations
│   ├── Model/                  # Domain entities and enums
│   ├── NotificationBuilders/   # Notification message builders
│   ├── Options/                # Configuration option classes
│   ├── Repositories/           # Data access layer
│   ├── Services/               # Business logic and background services
│   ├── Program.cs              # Application entry point and DI configuration
│   └── appsettings.json        # Default configuration
├── frontend/                   # React SPA
│   ├── src/
│   │   ├── components/         # Reusable UI, layout, and feature components
│   │   ├── context/            # Auth and notification React contexts
│   │   ├── hooks/              # Custom React hooks
│   │   ├── routes/             # File-based TanStack Router pages
│   │   ├── services/           # API client and service modules
│   │   └── utils/              # Formatting and helper utilities
│   ├── vite.config.ts          # Vite dev server and API proxy configuration
│   └── package.json
└── demo_images/                # Application screenshots for documentation
```

### API Modules

| Route Prefix | Purpose |
|--------------|---------|
| `/api/auth` | Registration and login |
| `/api/home` | Landing page stats, global search, categories, skills, tags |
| `/api/jobs` | Job CRUD, filtering, client job management, status transitions |
| `/api/freelancer/applications` | Freelancer application submission and management |
| `/api/freelancer/dashboard` | Freelancer dashboard analytics |
| `/api/bookmark` | Saved job bookmarks |
| `/api/reviews` | Job review submission and retrieval |
| `/api/notifications` | Notification CRUD and read status |
| `/api/users` | User profile operations |
| `/api/admin` | Administrative management and reporting |
| `/notificationHub` | SignalR hub for real-time notifications |

### User Roles

| Role | Access |
|------|--------|
| **Freelancer** | Browse jobs, submit applications, manage profile and portfolio, leave reviews |
| **Client** | Post and manage jobs, review applications, manage company profile, leave reviews |
| **Admin** | Moderate jobs, manage users and taxonomy, view platform analytics |

---

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd freelance-job-app
```

### 2. Configure the Backend

Navigate to the backend directory and restore dependencies:

```bash
cd backend
dotnet restore
```

Update the database connection string in `appsettings.json` or `appsettings.Development.json` if needed. The default configuration uses SQL Server LocalDB:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FreelanceJobDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;"
}
```

Configure JWT settings in `appsettings.json`. Replace the default secret key before deploying to production.

For file uploads, add Cloudinary credentials using .NET user secrets:

```bash
dotnet user-secrets set "Cloudinary:NAME" "your-cloud-name"
dotnet user-secrets set "Cloudinary:API_KEY" "your-api-key"
dotnet user-secrets set "Cloudinary:API_SECRET" "your-api-secret"
```

Apply database migrations (optional; the application also calls `EnsureCreated` and seeds data on startup):

```bash
dotnet ef database update
```

Start the API:

```bash
dotnet run
```

The API runs at `http://localhost:5140`. In development, Swagger UI is served at the root URL.

### 3. Configure and Run the Frontend

Open a second terminal:

```bash
cd frontend
npm install
npm run dev
```

The frontend runs at `http://localhost:5173`. During local development, API requests to `/api` and WebSocket connections to `/notificationHub` are proxied to the backend automatically via `vite.config.ts`.

Optional: create a `.env` file based on `.env.example` to override the API base URL for non-proxied deployments:

```env
VITE_API_BASE_URL=http://localhost:5140/api
```

### 4. Seed Account Credentials

The database seeder creates test accounts on first startup. All seeded accounts use the password `Password123!`.

| Role | Email | Username |
|------|-------|----------|
| Admin | admin@freelance.test | admin |
| Client | client1@freelance.test | client1 |
| Freelancer | freelancer1@freelance.test | freelancer1 |

Additional seeded clients (`client2` through `client8`) and freelancers (`freelancer2` through `freelancer12`) follow the same email and password pattern.

---

## Usage and Demo

### Public Experience

The landing page presents platform statistics, category navigation, and a job search entry point.

![Landing page](demo_images/Screenshot%202026-07-30%20154227.png)

![Landing page dark mode](demo_images/Screenshot%202026-07-30%20154736.png)

The job browse page supports advanced filtering by category, status, budget, skills, and sort order.

![Browse jobs](demo_images/Screenshot%202026-07-30%20154238.png)

Individual job pages display project details, required skills, attachments, client information, and an apply action.

![Job detail](demo_images/Screenshot%202026-07-30%20154248.png)

### Freelancer Dashboard

Freelancers access an overview with application counts, earnings, activity charts, and recent notifications.

![Freelancer dashboard overview](demo_images/Screenshot%202026-07-30%20154301.png)

The applications page tracks all submitted proposals with status-based filtering and withdrawal support.

![Freelancer applications](demo_images/Screenshot%202026-07-30%20154311.png)

### Client Dashboard

Clients manage posted jobs, review proposals, and transition jobs through their lifecycle.

![Client manage jobs](demo_images/Screenshot%202026-07-30%20154339.png)

The job posting form supports title, description, budget, deadline, categories, tags, and required skills.

![Post a job](demo_images/Screenshot%202026-07-30%20154346.png)

### Admin Panel

Administrators monitor platform health through summary metrics and trend charts.

![Admin overview](demo_images/Screenshot%202026-07-30%20154511.png)

Pending job postings can be approved or rejected before they become visible to freelancers.

![Admin job moderation](demo_images/Screenshot%202026-07-30%20154713.png)

Taxonomy management covers categories, skills, and tags used across job listings.

![Admin categories](demo_images/Screenshot%202026-07-30%20154719.png)

The reports section provides visual analytics for activity, revenue, and job creation over time.

![Admin reports](demo_images/Screenshot%202026-07-30%20154724.png)

---

## Authors and Acknowledgments

This project was developed as a final academic submission by:

- **Ahmed Alaa**
- **Omar Waleed**
- **Ahmed Ragy**

### Acknowledgments

- [TanStack](https://tanstack.com/) for Router, Query, and Start tooling
- [Radix UI](https://www.radix-ui.com/) and the shadcn/ui component patterns
- [Cloudinary](https://cloudinary.com/) for cloud-based media storage
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/) and [ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/) documentation
- [Lovable](https://lovable.dev/) for frontend development tooling integration
