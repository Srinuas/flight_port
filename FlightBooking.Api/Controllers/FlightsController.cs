using FlightBooking.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Api.Controllers;

[ApiController]
[Route("api/flights")]
public class FlightsController : ControllerBase
{
    private readonly AppDbContext _db;

    public FlightsController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string? origin, [FromQuery] string? destination)
    {
        var query = _db.Flights.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(origin))
            query = query.Where(x => x.OriginCode == origin.ToUpper() || x.Origin.ToLower().Contains(origin.ToLower()));

        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(x => x.DestinationCode == destination.ToUpper() || x.Destination.ToLower().Contains(destination.ToLower()));

        var flights = await query.OrderBy(x => x.DepartureUtc).ToListAsync();
        return Ok(flights);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var flight = await _db.Flights.FindAsync(id);
        return flight is null ? NotFound() : Ok(flight);
    }
}
