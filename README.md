# 🦕 Case-O-Saurus

**Case-O-Saurus** is an enterprise‑grade, modular **Case Management Module** designed for transversal government e‑services.  
It follows **Clean Architecture**, **Custom CQRS**, and **Docker‑first** principles, delivering a secure, auditable, and scalable solution for managing service requests from creation through to closure.

---

## Architecture Overview

```text
Case-O-Saurus/
├── src/
│   ├── CaseOSaurus.Domain/          # Entities, Enums, Business Rules
│   ├── CaseOSaurus.Application/     # CQRS Commands/Queries, Handlers, DTOs, Pipeline Behaviors
│   ├── CaseOSaurus.Infrastructure/  # EF Core DbContext, Migrations, External Services
│   ├── CaseOSaurus.API/             # FastEndpoints (Presentation Layer)
│   └── CaseOSaurus.Web/             # Blazor WASM (Client UI)
├── tests/
│   ├── CaseOSaurus.UnitTests/       # Unit tests for Handlers & Domain logic
│   └── CaseOSaurus.IntegrationTests/# Integration tests with Testcontainers
├── docker-compose.yml               # Orchestrates SQL Server and API containers
└── README.md
```

## Tech Stack

| Layer | Technology |
| --- | --- |
| **Backend API** | .NET 10, FastEndpoints |
| **Application Core** | Custom CQRS Mediator, FluentValidation |
| **Persistence** | SQL Server / Azure SQL Edge (Docker), Entity Framework Core 10 |
| **Frontend** | Blazor WebAssembly (WASM) |
| **Authentication** | JWT (Azure AD B2C / custom local issuer) |
| **Cross‑cutting** | Serilog (structured logging), Health Checks |
| **Containerisation** | Docker, Docker Compose |
| **Testing** | xUnit, Moq, FluentAssertions, Testcontainers.MsSql |

---

## Features (Implementation Status)

| Feature | User Story | Status |
| --- | --- | --- |
| Create a new case | US‑1 | ✅ Planned |
| Assign case to user/role | US‑2 | ✅ Planned |
| Update case status (Open → In Progress → Closed) | US‑3 | ✅ Planned |
| View case list with filters & sorting | US‑4 | ✅ Planned |
| View case history / audit trail | US‑5 | ✅ Planned |
| Role‑Based Access Control (RBAC) | US‑6 | ✅ Planned |
| Audit logging & concurrency handling | US‑7 | ✅ Planned |
| Cloud readiness & Docker setup | US‑8 | ✅ Planned |
| Event‑driven architecture (Domain Events) | US‑9 (Stretch) | ⏳ Optional |
| Notifications (Email/SMS) | US‑10 (Stretch) | ⏳ Optional |
| Dashboard metrics | US‑11 (Stretch) | ⏳ Optional |
| Offline‑ready (Blazor WASM + IndexedDB) | US‑12 (Stretch) | ⏳ Optional |

---

## Getting Started (Local Development)

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for SQL Server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/) with C# Dev Kit

### Step 1: Clone the repository

```bash
git clone <your-repo-url>
cd Case-O-Saurus
```

### Step 2: Start SQL Server via Docker

```bash
docker-compose up -d sqlserver
```

### Step 3: Apply EF Core Migrations

```bash
dotnet ef database update -p src/CaseOSaurus.Infrastructure -s src/CaseOSaurus.API
```

### Step 4: Run the API

```bash
dotnet run --project src/CaseOSaurus.API
```

### Step 5: Run the Blazor WASM Client

```bash
dotnet run --project src/CaseOSaurus.Web
```

### Step 6: Run everything with one command (optional)

```bash
docker-compose up -d
```
