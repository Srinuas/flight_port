using FlightBooking.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Api.Data;

public class DatabaseSeeder
{
    private readonly AppDbContext _db;

    public DatabaseSeeder(AppDbContext db) => _db = db;

    public async Task SeedAsync()
    {
        if (await _db.Flights.AnyAsync()) return;

        var today = DateTime.UtcNow.Date.AddDays(1);

        var flights = new[]
        {
            Flight.Create("FB101", "Hyderabad", "HYD", "Delhi", "DEL", today.AddHours(3), today.AddHours(5.2), "SkyVista Airways", "AI", 6499, 32, "https://images.unsplash.com/photo-1436491865332-7a61a109cc05?auto=format&fit=crop&w=1200&q=80"),
            Flight.Create("FB202", "Mumbai", "BOM", "Bengaluru", "BLR", today.AddHours(7), today.AddHours(8.8), "AeroBlue", "6E", 4599, 24, "https://images.unsplash.com/photo-1517479149777-5f3e1517d58f?auto=format&fit=crop&w=1200&q=80"),
            Flight.Create("FB303", "Delhi", "DEL", "Dubai", "DXB", today.AddDays(1).AddHours(4), today.AddDays(1).AddHours(8), "SkyVista Airways", "SV", 18999, 18, "https://images.unsplash.com/photo-1540962351504-03099e0a754b?auto=format&fit=crop&w=1200&q=80"),
            Flight.Create("FB404", "Chennai", "MAA", "Singapore", "SIN", today.AddDays(2).AddHours(2), today.AddDays(2).AddHours(8), "AeroBlue", "AB", 21999, 14, "https://images.unsplash.com/photo-1530521954074-e64f6810b32d?auto=format&fit=crop&w=1200&q=80"),
            Flight.Create("FB505", "Kochi", "COK", "Mumbai", "BOM", today.AddDays(1).AddHours(10), today.AddDays(1).AddHours(12.2), "Deccan Wings", "DW", 5299, 28, "https://images.unsplash.com/photo-1474302770737-173ee21bab63?auto=format&fit=crop&w=1200&q=80"),
            Flight.Create("FB606", "Bengaluru", "BLR", "Goa", "GOI", today.AddHours(11), today.AddHours(12.2), "Deccan Wings", "DW", 3999, 40, "https://images.unsplash.com/photo-1500530855697-b586d89ba3ee?auto=format&fit=crop&w=1200&q=80")
        };

        await _db.Flights.AddRangeAsync(flights);
        await _db.SaveChangesAsync();
    }
}
