# AGENTS.md — VMS Pro Back-End

## Architecture

Clean Architecture .NET 9 solution (C#) with 4 projects — dependency flows inward:

```
VMS.API → VMS.Application → VMS.Domain
VMS.API → VMS.Infrastructure → VMS.Domain
```

- **VMS.Domain** — Entities, enums, `ICurrentUserProvider`. Zero dependencies.
- **VMS.Infrastructure** — EF Core + Npgsql (PostgreSQL), generic `IRepository<T>`, Specification pattern, Unit of Work, EF configurations, migrations.
- **VMS.Application** — Business logic services, DTOs, AutoMapper profiles (`MappingProfile.cs`), FluentValidation validators, interfaces.
- **VMS.API** — ASP.NET Core Web API. Controllers, JWT auth (Google OAuth + local JWT), middleware, DI wiring in `ServiceExtensions.cs`.

## Build & Run

```bash
cd VMS.API
dotnet build          # build from API project (pulls all references)
dotnet run            # runs on https://localhost:44389
```

Database auto-migrates on startup (`Program.cs` calls `db.Database.Migrate()`). Toggle `"UseInMemory": true` in `appsettings.json` to use in-memory store instead of PostgreSQL.

EF migrations target `VMS.Infrastructure` but run from API:
```bash
dotnet ef migrations add <Name> --project ../VMS.Infrastructure --startup-project .
```

## Key Patterns

### All entities inherit `BaseEntity` (`VMS.Domain/Entities/BaseEntity.cs`)
Provides `Id` (Guid), audit fields (`CreatedAt/By`, `UpdatedAt/By`), and **soft delete** (`IsDeleted`, `DeletedAt/By`). Every EF configuration adds a global query filter: `builder.HasQueryFilter(e => !e.IsDeleted)`.

### Unit of Work + Factory
Services **never** inject `IRepository<T>` directly. They inject `IUnitOfWorkFactory`, call `CreateUnitOfWork()`, and use `uow.Appointments`, `uow.Users`, etc. Writes use `uow.ExecuteTransactionAsync(async () => { ... })` for atomic operations. See `AppointmentService.cs` as the canonical example.

### Specification Pattern for Queries
Complex queries with includes/paging go in `VMS.Infrastructure/Repositories/Specifications/{Entity}/`. Each spec class extends `Specification<T>` and configures `Includes`, `Criteria`, `OrderBy`, `Skip/Take`. Example: `GetAppointmentsPagedSpecification`.

### DTO Convention
Each entity has 3 DTOs: `{Entity}Dto` (read), `Create{Entity}Dto` (create), `Update{Entity}Dto` (update) — all in `VMS.Application/DTOs/{Entity}Dto.cs`. Enums are exposed as both `StatusId` (int) and `StatusLabel` (string) for frontend display.

### API Response Envelope
All controller actions return `ApiResponse<T>` wrapping `Success`, `Message`, `Data`, `Errors`. Use `ApiResponse<T>.SuccessResponse(data)` and `ApiResponse<T>.FailResponse(message)`. Paged endpoints return `ApiResponse<PagedResult<TDto>>`.

### Controller Routing
Routes follow `[Route("api/[controller]")]` with explicit action names: `[HttpGet("GetAll")]`, `[HttpGet("GetById")]`, `[HttpPost("Create")]`, `[HttpPut("Update")]`, `[HttpDelete("Delete")]`. Query params use `[FromQuery]` for ID lookups, `[FromBody]` for payloads.

### Validators
All validators are in a single file: `VMS.Application/Validators/Validators.cs`. Naming: `Create{Entity}DtoValidator`, `Update{Entity}DtoValidator`. Auto-registered via `AddValidatorsFromAssemblyContaining<LoginDtoValidator>()`.

### AutoMapper
Single profile in `VMS.Application/Mappings/MappingProfile.cs`. Entity↔DTO mappings with custom `ForMember` for field name differences (e.g., `FullName` → `Name`, `ScheduledAt` → `Date` + `Time`). Uses `EnumHelper` for label lookups.

### Auth
Google OAuth login via `AuthController.google-login` endpoint → validates Google ID token → looks up user in DB → issues local JWT. Claims include `NameIdentifier` (user ID), `Role`, and custom `organisationId`. Current user flows: `HttpCurrentUserProvider` (for audit in repos) and `UserContext` / `IUserContext` (for service-layer access).

## Adding a New Entity Checklist

1. Entity in `VMS.Domain/Entities/` extending `BaseEntity`
2. EF configuration in `VMS.Infrastructure/Data/Configurations/` (include audit FKs + `HasQueryFilter(!IsDeleted)`)
3. Add `DbSet` to `VmsDbContext`
4. Add property to `IUnitOfWork` and both UoW implementations
5. Specification classes in `VMS.Infrastructure/Repositories/Specifications/{Entity}/`
6. DTOs (`Dto`, `CreateDto`, `UpdateDto`) in `VMS.Application/DTOs/`
7. AutoMapper maps in `MappingProfile.cs`
8. Validators in `Validators.cs`
9. Service interface in `VMS.Application/Interfaces/`, implementation in `VMS.Application/Services/`
10. Controller in `VMS.API/Controllers/`, register service in `ServiceExtensions.AddApplicationServices()`

