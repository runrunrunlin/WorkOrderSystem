# WorkOrderSystem

A full-stack web application for managing maintenance work orders in manufacturing environments. Built with ASP.NET Core 8 and a vanilla JavaScript frontend, it supports role-based access for administrators and technicians to report, assign, and resolve equipment maintenance requests.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Backend | ASP.NET Core 8 Web API |
| ORM | Entity Framework Core 8 |
| Database | SQLite |
| Authentication | JWT Bearer Tokens |
| Password Hashing | BCrypt.Net |
| Frontend | HTML5 / CSS3 / Vanilla JavaScript |
| Runtime | .NET 8 |

---

## Features

- **JWT Authentication** — stateless token-based login with 8-hour session expiry
- **Role-Based Access Control** — two roles with distinct permissions:
  - **Admin** — full access: manage equipment, assign work orders, cancel or close any order
  - **Technician** — view assigned orders, submit completion reports
- **Work Order Lifecycle** — four-stage workflow: `Pending → InProgress → Completed / Cancelled`
- **Priority Levels** — Low, Medium, and High priority classification per work order
- **Equipment Management** — CRUD for plant equipment with status tracking (`Normal / UnderRepair`)
- **Automatic Status Sync** — equipment status updates automatically when a work order is opened or completed
- **Filtered Views** — query work orders by status or equipment; technicians see only their own orders
- **Browser-Based UI** — no build step required; served as static files from the same host

---

## Project Structure

```
WorkOrderSystem/
├── Controllers/
│   ├── AuthController.cs        # Login endpoint, JWT issuance
│   ├── WorkOrderController.cs   # Work order CRUD and lifecycle actions
│   ├── EquipmentController.cs   # Equipment CRUD
│   └── UsersController.cs       # User listing (Admin only)
├── Models/                      # EF Core entity classes
├── DTOs/                        # Request/response data transfer objects
├── Data/
│   ├── AppDbContext.cs          # EF Core DbContext
│   └── DbSeeder.cs              # Seed data for first run
├── wwwroot/                     # Static frontend (HTML + JS)
│   ├── index.html               # Login page
│   ├── dashboard.html           # Work order dashboard
│   ├── equipment.html           # Equipment management page
│   └── js/api.js                # Shared API client
├── Program.cs                   # App bootstrap and middleware pipeline
├── appsettings.json             # Connection string and JWT config
└── WorkOrderSystem.csproj
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Run Locally

```bash
# Clone the repository
git clone https://github.com/runrunrunlin/WorkOrderSystem.git
cd WorkOrderSystem

# Restore dependencies and start the server
dotnet run
```

The application will start on **http://localhost:5000** (or the port shown in the terminal). The SQLite database (`workorders.db`) and seed data are created automatically on first run — no database setup is required.


---

## API Overview

| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/login` | Public | Authenticate and receive JWT |
| GET | `/api/workorder` | Any | List work orders (filtered by role) |
| POST | `/api/workorder` | Any | Submit a new work order |
| PUT | `/api/workorder/{id}/assign` | Admin | Assign order to a technician |
| PUT | `/api/workorder/{id}/complete` | Any | Mark order as completed |
| PUT | `/api/workorder/{id}/cancel` | Admin | Cancel an open order |
| GET | `/api/equipment` | Any | List all equipment |
| POST | `/api/equipment` | Admin | Add new equipment |
| PUT | `/api/equipment/{id}` | Admin | Update equipment details |
| DELETE | `/api/equipment/{id}` | Admin | Delete equipment (blocked if open orders exist) |

