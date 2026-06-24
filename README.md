# VideoGamesCatalogue

A full-stack video game catalogue application — browse and edit video game entries.

**Backend:** ASP.NET Core Web API + EF Core (Code First) on SQL Server
**Frontend:** Angular + ng-bootstrap

---

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (or later)
- [Node.js](https://nodejs.org/) + npm (for the Angular frontend)
- **Docker Desktop** or **Rancher Desktop** (for running SQL Server locally)

---

## 1. Start SQL Server

This project uses Docker Compose to run SQL Server locally — no local SQL Server install required.

```bash
docker compose up -d
```

This starts a SQL Server 2022 container on `localhost:1433`. Data persists in a named Docker volume (`sql-data`) between restarts.

To stop it:

```bash
docker compose down
```

To stop it **and wipe the database** (useful if you want a clean reseed):

```bash
docker compose down -v
```

---

## 2. Configure connection string

The connection string (including the SA password) should be supplied via **.NET user secrets**, not committed to source control.

From `src/API`:

```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DBConnection" "Data Source=localhost;Database=VGCatalogueDb;User ID=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;"
```

> Note: the password above must match the `SA_PASSWORD` set in `docker-compose.yml`.

---

## 3. Run the backend

From `src/API`:

```bash
dotnet ef database update
dotnet run
```

The API will apply migrations and seed sample data (from `src/Infrastructure/Data/SeedData`) on first run. Swagger UI is available at `https://localhost:<port>/swagger`.

---

## 4. Run the tests

> **Requires the SQL Server container from step 1 to be running.** Integration tests run against a real SQL Server instance (not an in-memory provider) to validate actual EF Core/SQL Server behavior, including database-level constraints.

From the repo root:

```bash
dotnet test
```

Each test run creates and tears down its own database (`EnsureDeleted` / `Migrate` in test setup/teardown), so it's safe to run repeatedly against the same container.

---

## 5. Run the frontend

From the Angular project folder:

```bash
npm install
ng serve
```

Navigate to `http://localhost:4200`.

---

## Project structure

```
src/
  API/             ASP.NET Core Web API — controllers, Program.cs
  Core/            Entities, DTOs, service interfaces (no EF Core/ASP.NET dependency)
  Infrastructure/  EF Core DbContext, configurations, migrations, seed data
tests/
  API.Tests/       Integration tests via WebApplicationFactory
```

## Notes on design decisions

- **Code First** EF Core workflow, per assignment requirements.
- **No repository pattern** — `DbContext`/`DbSet` already provides repository and unit-of-work behavior; an additional interface would add abstraction without a second implementation.
- **DTOs** are used at the API boundary instead of exposing EF entities directly, to avoid over-posting risk and to decouple the wire contract from the persistence model.
- Entity property names follow [schema.org's VideoGame vocabulary](https://schema.org/VideoGame) (`name`, `datePublished`, `author`, `gamePlatform`, `genre`, `aggregateRating`) for a standardized, recognizable data model.
