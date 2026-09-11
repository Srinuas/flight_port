# Flight Booking App

A colourful, realistic flight-booking demo built with:

- ASP.NET Core 8
- EF Core + Pomelo MySQL
- Aiven MySQL
- Docker
- Render Web Service
- HTML + CSS + vanilla JavaScript frontend
- JWT authentication
- BCrypt password hashing
- CAPTCHA
- Favorites
- Flight search
- Checkout/payment UI
- UPI, credit/debit card and net banking demo flows
- Order history

## Main folders

```text
FlightBookingApp/
├── FlightBooking.Api/
│   ├── Controllers/
│   ├── Data/
│   ├── Dtos/
│   ├── Models/
│   ├── Services/
│   ├── wwwroot/
│   │   ├── css/
│   │   └── js/
│   ├── Dockerfile
│   ├── Program.cs
│   └── FlightBooking.Api.csproj
├── .env.example
├── render.yaml
├── DEPLOYMENT.md
└── README.md
```

See `DEPLOYMENT.md` for Render and Aiven setup.


## Database persistence guarantee

Application redeployments are designed to preserve Aiven MySQL data.

The Render container is replaceable; the Aiven database is external and persistent. Startup does not delete or truncate data. Existing users, favorites, orders and payment records remain across code deployments.

For future schema changes, use reviewed EF Core migrations and never add destructive database-reset commands to application startup.
