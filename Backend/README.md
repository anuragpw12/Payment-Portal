# Payments Portal - Backend (API)

.NET 8 Web API for the Payments module using Clean Architecture, Dapper, SQL Server stored procedures, and Swagger.

## Projects

- `Payments.Api` - Controllers, API startup, Swagger
- `Payments.Application` - DTOs, interfaces, business services
- `Payments.Domain` - Entities and domain validation
- `Payments.Infrastructure` - Dapper repositories and DB connection factory
- `sql/payments/payments_module.sql` - Schema + indexes + stored procedures

## Prerequisites

- .NET SDK 8+
- SQL Server 2019+ (or Express)

## Database Setup

1. Create database:

```sql
CREATE DATABASE PaymentsDb;
GO
```

2. Run SQL script:

- `sql/payments/payments_module.sql`

## API Configuration

Update connection string in `Payments.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "PaymentsDb": "Server=.;Database=PaymentsDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

## Run API

```bash
dotnet restore Payments.sln
dotnet build Payments.sln
dotnet run --project Payments.Api
```

## Swagger

Open:

- `https://localhost:<port>/swagger`

## Endpoints

- `POST /api/payments`
- `GET /api/payments`
- `PUT /api/payments/{id}`
- `DELETE /api/payments/{id}`

## Idempotency

`clientRequestId` is unique and idempotent:

- Duplicate retries do not create new records
- Existing payment is returned

## Validation Rules

- `Amount > 0`
- Currency must be one of: `USD`, `EUR`, `INR`, `GBP`

## Notes

- Clean Architecture dependency flow is enforced.
- Dapper repository uses stored procedures.
- Swagger is enabled for API testing.
