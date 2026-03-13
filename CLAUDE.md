# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build & Run

```bash
dotnet build                          # Build the project
dotnet run                            # Run on http://localhost:5167
```

No test project exists yet. To seed the database via SQL directly:
```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -i Scripts/SeedData.sql
```

## Architecture

ASP.NET Core OData API (.NET 10) with EF Core on SQL Server (LocalDB).

**Request flow:** Client → OData Middleware (parses `$filter`, `$select`, etc.) → `ProductsController` (ODataController) → `AppDbContext` (returns `IQueryable<Product>`) → SQL Server. OData query options are translated to SQL by EF Core — controllers never materialize full collections.

**Key patterns:**
- `[EnableQuery]` on GET methods enables OData query composition over `IQueryable`
- `Delta<T>` in PATCH for partial updates
- `SingleResult.Create()` for single-entity GETs
- `ODataConventionModelBuilder` builds the EDM model, exposed at `/odata/$metadata`
- `db.Database.EnsureCreated()` auto-creates DB + seeds 50 products on first dev run

**OData route prefix:** `/odata` (e.g., `/odata/Products`, `/odata/Products(1)`)

## Configuration

- **Connection string** in `appsettings.json` → defaults to `(localdb)\MSSQLLocalDB`, database `ProductODataDb`
- **OData caps:** `SetMaxTop(100)` limits `$top` to 100 results
- **Swagger UI** at root URL (`/`), JSON at `/swagger/v1/swagger.json`
- Dev server port: `5167` (HTTP only, configured in `Properties/launchSettings.json`)
