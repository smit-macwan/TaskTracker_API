# Task Tracker Backend

A full-stack application for a Task Tracking implemented in ASP.NET Core Web API with Clean Architecture boundaries and JWT role-based access control.

---

## Application Architecture

The solution is split by responsibility and dependency direction:

- `TaskManager.Domain`
  - Business entities and enums (`TaskItem`, `TaskItemStatus`).
  - No dependency on frameworks or infrastructure.
- `TaskManager.Application`
  - Use cases and business policy orchestration (`TaskService`, `TaskRules`).
  - Contracts for infrastructure (`ITaskRepository`, `ICurrentUser`).
  - Application-level exceptions and DTOs.
- `TaskManager.Infrastructure`
  - EF Core + SQLite persistence.
  - ASP.NET Core Identity user/role store integration.
  - Concrete repository implementation and DI registration.
- `TaskManager.WebApi`
  - API controllers, JWT auth wiring, middleware, startup composition root.
  - Database migration and seed bootstrap logic at startup.

### Architectural Intent

- Keep core policy in `Application` and `Domain`.
- Isolate framework concerns in `Infrastructure` and `WebApi`.
- Depend on abstractions inward; implement outward.
- Enforce authorization and workflow invariants at service boundary (not only UI).

---

## High-Level Design

### Core Use Cases

- Register/login users with JWT issuance.
- Create/read/update task metadata for owner.
- Change task status with workflow validation.
- Admin-only endpoint for global task visibility.

### Security Model

- Authentication: JWT bearer.
- Authorization:
  - Standard users can only access their own tasks.
  - Admin role can list all tasks.
- Identity:
  - Backed by ASP.NET Core Identity tables in same SQLite database.

### Domain/Business Rules

- Valid status transitions:
  - `ToDo -> InProgress`
  - `InProgress -> Done`
- Invalid transitions are rejected with validation errors.
- `Done` is disallowed when description is empty.
- `DueDate`, if provided on create, cannot be in the past.

### Persistence Design

- EF Core migrations tracked in `Infrastructure/Persistence/Migrations`.
- Startup applies migrations automatically (`MigrateAsync`).
- SQLite path is normalized to a deterministic location under Web API content root:
  - `src/TaskManager.WebApi/Data/taskmanager.db`

---

## How To Run The Solution

### Prerequisites

- .NET SDK 10.x
- Node.js + npm (only if running the React frontend)

### 1) Run backend API

From repo root:

```bash
dotnet restore
dotnet run --project .\src\TaskManager.WebApi\TaskManager.WebApi.csproj
```

What startup does:

- Applies EF Core migrations.
- Ensures roles (`user`, `admin`) exist.
- Seeds default admin user (if configured).

Swagger endpoints:

- `http://localhost:5134/swagger`
- `https://localhost:7021/swagger` (launch profile dependent)

Default seeded admin:

- Email: `admin@local`
- Password: `Admin123!`

Configured via:

- `src/TaskManager.WebApi/appsettings.json` -> `SeedAdmin`

### 2) Run tests

```bash
dotnet test .\TaskManager.slnx
```

### 3) Run frontend (optional)

From `frontend`:

```bash
npm install
npm run dev
```

Set API URL (if required):

```bash
set VITE_API_BASE_URL=http://localhost:5134
npm run dev
```

---

## Assumptions

- Primary target is local/demo and interview-style evaluation scenarios.
- SQLite is acceptable for current data volume and concurrency expectations.
- Applying migrations on startup is acceptable in this deployment model.
- Two-role model (`user`, `admin`) is sufficient for current authorization scope.
- JWT secret in appsettings is development-only and replaced per environment in real deployments.

---

## Trade-Offs / Limitations

- **Database choice:** SQLite simplifies setup but is not ideal for high write concurrency or multi-node scaling.
- **API maturity:** No pagination/filtering/sorting contracts for task list endpoints yet.
- **Observability:** Limited telemetry/correlation/audit logging for production incident analysis.
- **Operational hardening:** No rate limiting, rotation strategy for JWT keys, or secrets management integration in current baseline.

