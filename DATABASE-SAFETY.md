# Database Safety Rules

## Deployment behavior

A Render Docker redeploy replaces the application container. It does NOT recreate the Aiven MySQL database.

The application startup must remain non-destructive.

### Allowed

- Connect to existing Aiven MySQL
- Create missing schema on a brand-new empty database
- Insert seed data only when the relevant table is empty
- Use reviewed EF Core migrations for schema evolution

### Forbidden in startup/deployment

- `EnsureDeleted()` / `EnsureDeletedAsync()`
- `DROP DATABASE`
- `DROP TABLE`
- `TRUNCATE TABLE`
- blanket `DELETE` statements
- deleting/recreating the Aiven service as part of an application deployment

## Current project behavior

`Program.cs` calls `EnsureCreatedAsync()` only.

`DatabaseSeeder.cs` checks whether flights already exist:

```csharp
if (await _db.Flights.AnyAsync()) return;
```

Therefore an existing populated database is not cleared or reseeded during a normal Render deployment.

## Future schema changes

When the application's database model changes, introduce an EF Core migration and review the generated SQL before production deployment.

Back up the Aiven database before important production schema changes.

## Backup recommendation

Use Aiven's backup/restore capabilities according to the selected service plan and keep a separate recovery strategy for production data.
