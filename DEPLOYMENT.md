# Flight Booking App — Render + Docker + Aiven MySQL

## Architecture

Browser → ASP.NET Core 8 web service → EF Core/Pomelo → Aiven MySQL

The ASP.NET Core app serves the separate HTML/CSS/JS frontend from `wwwroot` and exposes JSON APIs under `/api`.

## 1. Create the Aiven MySQL database

1. Create an Aiven for MySQL service.
2. Open the service's **Overview → Connection information**.
3. Record:
   - Host
   - Port
   - Database name
   - Username
   - Password
4. Keep SSL enabled. The application uses `DB_SSL_MODE=Required`.

Do not commit the database password to Git.

## 2. Create the Git repository

From the project root:

```bash
git init
git add .
git commit -m "Initial Flight Booking App"
git branch -M main
git remote add origin YOUR_GITHUB_REPOSITORY
git push -u origin main
```

## 3. Deploy to Render

1. Open Render.
2. Choose **New → Web Service**.
3. Connect the GitHub repository.
4. Set **Language/Runtime = Docker**.
5. Set Dockerfile path to:
   `FlightBooking.Api/Dockerfile`
6. Use `/api/health` as the health-check path.
7. Add these environment variables in Render:

```text
DB_HOST=<Aiven host>
DB_PORT=<Aiven port>
DB_NAME=<Aiven database>
DB_USER=<Aiven user>
DB_PASSWORD=<Aiven password>
DB_SSL_MODE=Required
JWT_KEY=<long random secret>
```

Do not put secrets in the repository.

Render supplies the runtime `PORT`; the application binds to `0.0.0.0:<PORT>`.

## 4. Database persistence — IMPORTANT

**Redeploying the application does NOT delete the database data.**

The MySQL database is hosted by Aiven, separately from the Render Docker container. Therefore:

```text
Render code deployment
        ↓
new Docker container
        ↓
connects to the SAME Aiven MySQL database
        ↓
existing users / orders / favorites / payments remain
```

The application contains **no `EnsureDeleted()`**, `DropDatabase()`, `DELETE FROM`, `TRUNCATE`, or database-reset operation during startup.

At startup it calls:

```csharp
await db.Database.EnsureCreatedAsync();
```

`EnsureCreatedAsync()` creates the database/schema only when the database does not already have the schema. It does **not** delete an existing database.

The flight seeder is also non-destructive: it only inserts demo flights when the `Flights` table is empty.

### Important rule for future code changes

Do **not** add any of these to deployment/startup code:

```csharp
Database.EnsureDeleted();
Database.EnsureDeletedAsync();
context.Database.ExecuteSqlRaw("DROP ...");
context.Database.ExecuteSqlRaw("TRUNCATE ...");
```

Also do not manually recreate the Aiven MySQL service when deploying a new application version.

### Schema changes later

For a production application with frequent schema changes, use **EF Core migrations** instead of `EnsureCreatedAsync()`. Migrations should contain additive/controlled changes such as adding a column or table and must be reviewed before deployment.

Do not use a destructive migration that drops user/order/payment data unless you have explicitly backed up the database and intentionally approved that data change.

## 5. Test after deployment

Open:

```text
https://YOUR-SERVICE.onrender.com/
https://YOUR-SERVICE.onrender.com/api/health
https://YOUR-SERVICE.onrender.com/swagger
```

Expected health response:

```json
{"status":"healthy","database":"connected"}
```

## 6. Important production hardening

This project is a realistic full-stack demo, not a production payment gateway.

Before accepting real money:
- Integrate Razorpay/Stripe/PayU or another PCI-compliant provider.
- Never store CVV.
- Never store complete card numbers.
- Use provider-hosted/tokenized card entry.
- Add email OTP for password recovery.
- Add rate limiting and account lockout.
- Add CSRF protection where cookie authentication is used.
- Use real CAPTCHA such as reCAPTCHA/Turnstile.
- Use EF Core migrations.
- Add structured logging and monitoring.
- Add a transactional outbox for booking/payment events.
- Add seat inventory locking appropriate for your expected concurrency.
- Add HTTPS-only production configuration and security headers.
- Add proper GDPR/privacy/retention controls if applicable.

## Demo account flow

There is no pre-created user. Register from the UI, then log in.

Password rule: 6–12 characters.

Login accepts either username or Gmail address.

Password recovery verifies username/email + phone + CAPTCHA and then sets a new 6–12 character password. For a real deployment, add a one-time OTP sent to the verified contact method.
