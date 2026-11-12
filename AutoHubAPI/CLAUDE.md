# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.
Використовуй короткі відповіді, без зайвого коду, лише важлива інформація!

Перед рефакторингом і роботою з кодом завжди читай файл @spec.md

## Overview

AutoHubAPI is an ASP.NET Core Web API (.NET 9) backed by PostgreSQL via EF Core (Npgsql). It serves a commercial-vehicle marketplace (vehicles, brands, body/fuel types, leads) plus a separate "TestHub" quiz module (tests → questions → answer options). Seed data and domain strings are in Ukrainian.

## Commands

Solution file is `AutoHubAPI.slnx` (new XML solution format — requires a recent .NET SDK).

```bash
dotnet build AutoHubAPI.slnx
dotnet run --project AutoHubAPI          # http://localhost:5230, Swagger at /swagger

# EF Core migrations: DbContext + migrations live in Domain, startup project is AutoHubAPI
dotnet ef migrations add <Name> --project Domain --startup-project AutoHubAPI
dotnet ef database update --project Domain --startup-project AutoHubAPI
```

No linter is configured. There is no test project yet — see **Testing rules** below for how to add one. Once it exists:

```bash
dotnet test AutoHubAPI.slnx
dotnet test --filter "FullyQualifiedName~VehicleServiceTests.GetById_ReturnsNull_WhenDeleted"   # single test
```

A full project overview (entities, dependencies, known tech debt) is in `spec.md`.

## Architecture

Four projects, layered by reference chain: `AutoHubAPI` → `Infrastructure` → `Application` → `Domain`.

- **Domain** — entities (`Domain/Entities`), `AppDbContext`, and EF migrations. Entity config is done with Data Annotations (`[Table]`, `[Index]`, `[ForeignKey]`, `[Column(TypeName=...)]`) rather than Fluent API; `AppDbContext` has no `OnModelCreating`.
- **Application** — interfaces, `GenericRepository<TEntity,TKey>`, specifications, services (`SeederService`, `TestService`) and DTO/request models in `Application/Models/<Feature>` (e.g. `Models/Tests/TestItemDto`, `TestSaveRequest`). Note it depends directly on `AppDbContext` (no separate persistence layer).
- **Infrastructure** — `ProgramConfiguration/DependencyInjection.cs` (`AddInfrastructureServices`) is the single place where all services are registered: CORS policy `"AllowAll"`, repositories/services, Quartz, DbContext, controllers, Swagger. Add new DI registrations here, not in `Program.cs`.
- **AutoHubAPI** — thin host: `Program.cs` calls `AddInfrastructureServices` and wires middleware; controllers live here (`TestsController` — `GET /api/tests`, `GET /api/tests/{id}`, `GET /api/tests/{id}/pass`, `POST /api/tests/{id}/submit`; plus the template `WeatherForecastController`).

### Key patterns

- **Base entity & soft delete**: marketplace entities inherit `BaseEntity<T>` (implements `IEntity<T>`: `Id`, `IsDeleted`, `DateCreated`). `GenericRepository` filters `IsDeleted` by default and `DeleteAsync` performs a soft delete. TestHub entities (`Domain/Entities/TestHub`) do **not** inherit `BaseEntity`, so they cannot be used with `IGenericRepository` — `TestService` queries `AppDbContext` directly, projects to DTOs with `Select` + `AsNoTracking`, and deletes physically (questions/answers cascade).
- **Specifications**: derive from `BaseSpecification<T>` (in `Application/Interfaces/ISpecification.cs`) for criteria/includes/ordering and pass to `IGenericRepository.ListAsync`.
- **Startup migration + seeding via Quartz**: `Infrastructure/Jobs/DbSeedJob` runs once at startup (`StartNow` trigger) and calls `ISeederService.UpdateDatabase()` (`Database.MigrateAsync()`), so pending migrations are applied automatically on app run. Seed methods are idempotent (skip if the table already has rows). Currently the job only seeds TestHub data via `SeedTestHubAsync()`, which reads `AutoHubAPI/SeedData/tests.json` from `AppContext.BaseDirectory` (copied on build/publish via `<Content Update>` in `AutoHubAPI.csproj`) and adds only tests (matched by `Title`) and questions (matched by `Text` within the test) that are missing — to change TestHub seed data, edit the JSON, not C#; the vehicle/lookup seed methods exist in `SeederService` but are not invoked — add calls to `DbSeedJob.Execute` (lookups before `SeedVehiclesAsync`, which resolves FKs by name) if needed.
- **Connection string** `DefaultConnection` in `AutoHubAPI/appsettings.json` points to a remote Neon PostgreSQL instance — running the app or `dotnet ef database update` modifies that shared database.

## Coding rules

Follow the conventions already used in the codebase:

- **C# style**: file-scoped namespaces matching the folder path (`namespace Application.Services;`); primary constructors for DI (`public class SeederService(AppDbContext db) : ISeederService`); nullable reference types are on — mark optional fields `?`, initialize required reference properties with `= null!` (or `= string.Empty`) and collections with `= new List<T>()`.
- **Naming**: entities end in `Entity` (`VehicleEntity`); interfaces start with `I`; all async methods end in `Async` and return `Task`/`Task<T>`; DTOs/request models should end in `Dto` / `Request` / `Response`.
- **Async all the way**: use EF Core async APIs (`ToListAsync`, `AnyAsync`, `FirstOrDefaultAsync`); never `.Result` / `.Wait()`.
- **Dates**: store `DateTime.UtcNow` (PostgreSQL `timestamptz` rejects non-UTC `DateTime` via Npgsql).
- **Language**: code identifiers in English; user-facing strings, seed data and explanatory comments may be in Ukrainian, as in the existing code.
- Keep `Program.cs` minimal — new service registrations go into `AddInfrastructureServices`.

## Architecture rules

- **Respect the dependency direction** `AutoHubAPI → Infrastructure → Application → Domain`. Never add a reference that points the other way (e.g. `Domain` must not reference `Application`).
- **New entity** (marketplace): put it in `Domain/Entities`, inherit `BaseEntity<int>`, set a snake_case table name via `[Table("...")]`, configure with Data Annotations (`[Required]`, `[MaxLength]`, `[ForeignKey]`, `[Index]` on FK/filter columns, `[Column(TypeName = "numeric(p,s)")]` for money/decimals), add a `DbSet` to `AppDbContext`, then create a migration. Never edit an already-applied migration — add a new one.
- **Data access**: for entities implementing `IEntity<T>` use `IGenericRepository<TEntity,TKey>`; express non-trivial queries as a `BaseSpecification<T>` subclass instead of building LINQ in controllers. Deletes are soft (`IsDeleted = true`) — any hand-written query against `AsQurable(isSoft: true)` or `AppDbContext` must filter `!IsDeleted` itself, since there is no global query filter.
- **Business logic lives in Application services** (`Application/Services`, interface in `Application/Interfaces`), registered as scoped in DI. Controllers in `AutoHubAPI/Controllers` stay thin: validate input, call a service, map to DTOs. Do not return EF entities directly from endpoints (navigation properties cause cycles and leak the schema).
- **Background / startup work** goes into a Quartz job in `Infrastructure/Jobs`, registered in the `AddQuartz` block. Seed methods must stay idempotent (check `AnyAsync()` first) and must be called in FK order (lookups → vehicles → leads; tests → questions → answer options).
- **Secrets**: do not add new credentials to `appsettings.json`; use `dotnet user-secrets` or environment variables (`ConnectionStrings__DefaultConnection`).

## Testing rules

- Test project: `AutoHubAPI.Tests` at the repo root, xUnit, referencing the projects under test, and added to `AutoHubAPI.slnx` (`dotnet new xunit -n AutoHubAPI.Tests && dotnet sln AutoHubAPI.slnx add AutoHubAPI.Tests`). Mirror the source folder structure (`Application/Services/SeederServiceTests.cs`).
- Test naming: `Method_ExpectedResult_WhenCondition`; structure each test as Arrange / Act / Assert.
- **Never run tests against the Neon database** from `appsettings.json`. Unit tests of repositories/services use a fresh `AppDbContext` per test backed by SQLite in-memory (open a `SqliteConnection("DataSource=:memory:")` and keep it open for the test's lifetime, pass it to `UseSqlite(connection)`, then `EnsureCreated()`), which enforces FKs and unique indexes unlike the EF InMemory provider.
- **Integration tests** (`WebApplicationFactory<Program>`) must replace the `AppDbContext` registration with a test database **and remove the Quartz hosted service**, otherwise `DbSeedJob` runs `MigrateAsync()` on startup. `Program` needs `public partial class Program;` at the end of `Program.cs` to be visible to the test project.
- What to cover for each feature: the happy path, soft-delete behavior (deleted records are not returned by default), not-found cases, and validation failures. Seeder tests should assert idempotency (running twice does not duplicate rows).
