# Product OData API

A RESTful OData API built with ASP.NET Core that supports full CRUD operations for managing products, with OData query capabilities and Entity Framework Core for data persistence.

---

## Table of Contents

- [What is OData?](#what-is-odata)
- [Project Architecture](#project-architecture)
- [How OData is Implemented](#how-odata-is-implemented)
  - [1. NuGet Packages](#1-nuget-packages)
  - [2. EDM Model Configuration](#2-edm-model-configuration)
  - [3. Service Registration](#3-service-registration)
  - [4. OData Controller](#4-odata-controller)
  - [5. Entity Framework Core Setup](#5-entity-framework-core-setup)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
  - [CRUD Operations](#crud-operations)
  - [OData Query Options](#odata-query-options)
- [OData Query Examples](#odata-query-examples)
  - [$filter](#filter)
  - [$select](#select)
  - [$orderby](#orderby)
  - [$top](#top)
  - [$count](#count)
  - [Combined Queries](#combined-queries)
- [Product Model](#product-model)
- [Seed Data](#seed-data)
- [Configuration](#configuration)

---

## What is OData?

**OData (Open Data Protocol)** is a standardized REST-based protocol for building and consuming queryable APIs. It allows clients to query data using URL conventions, reducing the need for custom endpoints.

### Key Benefits

| Feature | Description |
|---------|-------------|
| **Standardized querying** | Clients use URL query parameters (`$filter`, `$select`, `$orderby`, etc.) instead of custom endpoints |
| **Reduced API surface** | One endpoint serves multiple query patterns — no need for `/products/by-category`, `/products/sorted-by-price`, etc. |
| **Client-driven queries** | Consumers decide what data they need, reducing over-fetching and under-fetching |
| **Metadata support** | The `$metadata` endpoint describes the data model, enabling auto-generated client SDKs |
| **Interoperability** | OData is an OASIS standard supported by Microsoft, SAP, and many enterprise tools |

### OData vs Traditional REST

```
Traditional REST:
  GET /api/products/electronics          → Custom endpoint per filter
  GET /api/products/sorted-by-price      → Custom endpoint per sort
  GET /api/products/cheap-electronics    → Custom endpoint per combination

OData:
  GET /odata/Products?$filter=Category eq 'Electronics'
  GET /odata/Products?$orderby=Price
  GET /odata/Products?$filter=Category eq 'Electronics'&$orderby=Price&$top=5
  → One endpoint, flexible querying via URL parameters
```

---

## Project Architecture

```
Client Request
     │
     ▼
┌─────────────────────┐
│   OData Middleware   │  ← Parses $filter, $select, $orderby, $top, $expand
│   (Route: /odata)   │
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│  ProductsController  │  ← ODataController with [EnableQuery] attribute
│  (ODataController)   │
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│    AppDbContext       │  ← EF Core DbContext returns IQueryable<Product>
│  (Entity Framework)  │
└─────────┬───────────┘
          │
          ▼
┌─────────────────────┐
│    SQL Server        │  ← OData query translated to efficient SQL
│   (ProductODataDb)   │
└─────────────────────┘
```

**How it flows:**
1. Client sends a request like `GET /odata/Products?$filter=Price gt 100&$top=5`
2. OData middleware parses the query options from the URL
3. The controller returns `IQueryable<Product>` (not materialized data)
4. OData + EF Core translates the query options into SQL: `SELECT TOP(5) ... WHERE Price > 100`
5. Only the filtered/sorted data is fetched from SQL Server — efficient and performant

---

## How OData is Implemented

### 1. NuGet Packages

```xml
<!-- ProductODataApi.csproj -->
<PackageReference Include="Microsoft.AspNetCore.OData" Version="9.4.1" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="10.0.4" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="10.0.4" />
```

The `Microsoft.AspNetCore.OData` package provides:
- OData routing and middleware
- Query option parsing (`$filter`, `$select`, `$orderby`, `$top`, `$expand`)
- EDM (Entity Data Model) builder
- `ODataController` base class
- `[EnableQuery]` attribute for automatic query composition

### 2. EDM Model Configuration

The **Entity Data Model (EDM)** tells OData what entities and properties are queryable:

```csharp
// Program.cs
static IEdmModel GetEdmModel()
{
    var builder = new ODataConventionModelBuilder();
    builder.EntitySet<Product>("Products");  // Registers "Products" as a queryable entity set
    return builder.GetEdmModel();
}
```

- `ODataConventionModelBuilder` uses conventions to map C# classes to OData entities
- `EntitySet<Product>("Products")` exposes the `Product` class as `/odata/Products`
- The EDM is also exposed at `/odata/$metadata` for client discovery

### 3. Service Registration

OData query options are enabled in `Program.cs` during service configuration:

```csharp
builder.Services.AddControllers()
    .AddOData(options => options
        .Select()       // Enable $select  → choose specific properties
        .Filter()       // Enable $filter  → filter by conditions
        .OrderBy()      // Enable $orderby → sort results
        .SetMaxTop(100) // Enable $top     → limit results (max 100)
        .Count()        // Enable $count   → include total count
        .Expand()       // Enable $expand  → expand related entities
        .AddRouteComponents("odata", GetEdmModel()));  // Mount at /odata prefix
```

| Method | Query Option | Purpose |
|--------|-------------|---------|
| `.Select()` | `$select` | Choose which properties to return |
| `.Filter()` | `$filter` | Filter results by conditions |
| `.OrderBy()` | `$orderby` | Sort results |
| `.SetMaxTop(100)` | `$top` | Limit number of results (capped at 100) |
| `.Count()` | `$count` | Return total count of matching records |
| `.Expand()` | `$expand` | Include related/nested entities |

### 4. OData Controller

The controller inherits from `ODataController` and uses `[EnableQuery]` to activate OData query processing:

```csharp
public class ProductsController : ODataController
{
    private readonly AppDbContext _db;

    // GET /odata/Products
    [EnableQuery]                    // ← This attribute enables OData query options
    public IActionResult Get()
    {
        return Ok(_db.Products);     // ← Returns IQueryable, NOT List
    }                                //   OData composes the query before execution

    // GET /odata/Products(1)
    [EnableQuery]
    public IActionResult Get([FromRoute] int key)
    {
        var product = _db.Products.Where(p => p.Id == key);
        return Ok(SingleResult.Create(product));  // ← SingleResult for single entity
    }

    // POST /odata/Products
    public async Task<IActionResult> Post([FromBody] Product product)
    {
        _db.Products.Add(product);
        await _db.SaveChangesAsync();
        return Created(product);     // ← Returns 201 Created with location header
    }

    // PUT /odata/Products(1)        → Full update (replaces all fields)
    // PATCH /odata/Products(1)      → Partial update (uses Delta<Product>)
    // DELETE /odata/Products(1)     → Deletes the product
}
```

**Key implementation details:**

- **`[EnableQuery]`** — Applied to GET methods. Tells OData to intercept the `IQueryable` and apply URL query parameters as LINQ expressions before executing the query
- **`IQueryable` return** — The controller returns `IQueryable<Product>`, not `List<Product>`. This is critical because OData appends `.Where()`, `.OrderBy()`, `.Take()` etc. to the query before EF Core translates it to SQL
- **`SingleResult.Create()`** — Used for single-entity GET to enable `$select` and `$expand` on individual items
- **`Delta<Product>`** — Used in PATCH to apply only the properties sent by the client
- **`Created()` / `Updated()` / `NoContent()`** — OData-specific response helpers from `ODataController`

### 5. Entity Framework Core Setup

```csharp
// Program.cs — Register DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Ensure database exists on startup (development only)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();  // Creates DB + tables + seeds data if not exists
}
```

The `AppDbContext` defines the schema and seed data:

```csharp
public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            // ...
        });

        modelBuilder.Entity<Product>().HasData(/* 50 seed products */);
    }
}
```

---

## Project Structure

```
ProductODataApi/
├── Controllers/
│   └── ProductsController.cs    # OData CRUD controller (GET, POST, PUT, PATCH, DELETE)
├── Data/
│   └── AppDbContext.cs           # EF Core DbContext with model config + 50 seed products
├── Models/
│   └── Product.cs                # Product entity definition
├── Scripts/
│   └── SeedData.sql              # Standalone SQL script for bulk seeding (50 products)
├── Properties/
│   └── launchSettings.json       # Dev server config (port 5167)
├── Program.cs                    # App startup: OData + EF Core + Swagger configuration
├── appsettings.json              # Connection string and app settings
└── ProductODataApi.csproj        # Project file with package references
```

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- SQL Server (LocalDB is used by default)

### Run the API

```bash
cd ProductODataApi
dotnet run
```

The API starts at `http://localhost:5167`.

### Access Points

| URL | Description |
|-----|-------------|
| `http://localhost:5167/swagger/index.html` | Swagger UI |
| `http://localhost:5167/odata/Products` | OData Products endpoint |
| `http://localhost:5167/odata/$metadata` | OData metadata (EDM schema) |

### Optional: Run SQL Seed Script Directly

```bash
sqlcmd -S "(localdb)\MSSQLLocalDB" -i Scripts/SeedData.sql
```

---

## API Endpoints

### CRUD Operations

| Method | URL | Description |
|--------|-----|-------------|
| `GET` | `/odata/Products` | Get all products (supports OData queries) |
| `GET` | `/odata/Products({id})` | Get a single product by ID |
| `POST` | `/odata/Products` | Create a new product |
| `PUT` | `/odata/Products({id})` | Full update of a product |
| `PATCH` | `/odata/Products({id})` | Partial update of a product |
| `DELETE` | `/odata/Products({id})` | Delete a product |

### POST Request Body Example

```json
{
  "Name": "Wireless Earbuds",
  "Price": 79.99,
  "Category": "Electronics"
}
```

### PUT Request Body Example

```json
{
  "Name": "Wireless Earbuds Pro",
  "Price": 99.99,
  "Category": "Electronics"
}
```

### PATCH Request Body Example (Partial Update)

```json
{
  "Price": 89.99
}
```

---

## OData Query Options

| Option | Description | Example |
|--------|-------------|---------|
| `$filter` | Filter results by condition | `$filter=Price gt 100` |
| `$select` | Choose specific properties | `$select=Name,Price` |
| `$orderby` | Sort results | `$orderby=Price desc` |
| `$top` | Limit number of results | `$top=10` |
| `$count` | Include total count | `$count=true` |
| `$expand` | Include related entities | `$expand=Category` |

---

## OData Query Examples

### $filter

Filter by exact match:
```
GET /odata/Products?$filter=Category eq 'Electronics'
```

Filter by comparison:
```
GET /odata/Products?$filter=Price gt 100
GET /odata/Products?$filter=Price lt 50
GET /odata/Products?$filter=Price ge 50 and Price le 200
```

Filter with string functions:
```
GET /odata/Products?$filter=contains(Name, 'Gaming')
GET /odata/Products?$filter=startswith(Name, 'Wireless')
```

Filter by date:
```
GET /odata/Products?$filter=CreatedDate gt 2025-03-01T00:00:00Z
```

Logical operators:
```
GET /odata/Products?$filter=Category eq 'Electronics' and Price lt 100
GET /odata/Products?$filter=Category eq 'Books' or Category eq 'Gaming'
GET /odata/Products?$filter=not (Category eq 'Clothing')
```

### $select

Return only specific fields:
```
GET /odata/Products?$select=Name,Price
GET /odata/Products?$select=Name,Category
GET /odata/Products(1)?$select=Name,Price
```

### $orderby

Sort ascending (default):
```
GET /odata/Products?$orderby=Price
GET /odata/Products?$orderby=Name
```

Sort descending:
```
GET /odata/Products?$orderby=Price desc
GET /odata/Products?$orderby=CreatedDate desc
```

Multiple sort fields:
```
GET /odata/Products?$orderby=Category,Price desc
```

### $top

Limit results:
```
GET /odata/Products?$top=5
GET /odata/Products?$top=10&$orderby=Price desc
```

### $count

Include total count in response:
```
GET /odata/Products?$count=true
GET /odata/Products?$filter=Category eq 'Electronics'&$count=true
```

### Combined Queries

Top 5 cheapest electronics (name and price only):
```
GET /odata/Products?$filter=Category eq 'Electronics'&$orderby=Price&$top=5&$select=Name,Price
```

All products over $100, sorted by price descending, with count:
```
GET /odata/Products?$filter=Price gt 100&$orderby=Price desc&$count=true
```

Gaming products sorted by name:
```
GET /odata/Products?$filter=Category eq 'Gaming'&$orderby=Name&$select=Name,Price,Category
```

Latest 3 products added:
```
GET /odata/Products?$orderby=CreatedDate desc&$top=3
```

---

## Product Model

| Property | Type | Constraints | Description |
|----------|------|-------------|-------------|
| `Id` | `int` | Primary Key, Auto-increment | Unique identifier |
| `Name` | `string` | Required, Max 200 chars | Product name |
| `Price` | `decimal` | Required, decimal(18,2) | Product price |
| `Category` | `string` | Max 100 chars | Product category |
| `CreatedDate` | `DateTime` | Defaults to UTC now | Creation timestamp |

---

## Seed Data

The application seeds **50 products** across **8 categories** on first run:

| Category | Count | Price Range |
|----------|-------|-------------|
| Electronics | 10 | $29.99 - $1,299.99 |
| Furniture | 7 | $44.99 - $549.99 |
| Clothing | 7 | $19.99 - $129.99 |
| Books | 6 | $35.99 - $54.99 |
| Sports | 5 | $12.99 - $79.99 |
| Kitchen | 5 | $39.99 - $199.99 |
| Stationery | 5 | $7.99 - $49.99 |
| Gaming | 5 | $24.99 - $299.99 |

A standalone SQL seed script is also available at `Scripts/SeedData.sql` for direct database seeding.

---

## Configuration

### Connection String (`appsettings.json`)

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=ProductODataDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

To use a different SQL Server instance, update the `Server` value:

```
Server=localhost              → Local SQL Server (default instance)
Server=localhost\SQLEXPRESS   → SQL Server Express
Server=(localdb)\MSSQLLocalDB → LocalDB (default, no setup needed)
Server=your-server.database.windows.net → Azure SQL
```
