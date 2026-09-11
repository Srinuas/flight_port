using System.Security.Claims;
using FlightBooking.Api.Data;
using FlightBooking.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/favorites")]
public class FavoritesController : ControllerBase
{
    private readonly AppDbContext _db;
    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public FavoritesController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _db.Favorites.Where(x => x.UserId == UserId).Include(x => x.Flight).ToListAsync());

    [HttpPost("{flightId:int}")]
    public async Task<IActionResult> Add(int flightId)
    {
        if (!await _db.Flights.AnyAsync(x => x.Id == flightId)) return NotFound("Flight not found.");
        if (!await _db.Favorites.AnyAsync(x => x.UserId == UserId && x.FlightId == flightId))
        {
            _db.Favorites.Add(new Favorite { UserId = UserId, FlightId = flightId });
            await _db.SaveChangesAsync();
        }
        return Ok();
    }

    [HttpDelete("{flightId:int}")]
    public async Task<IActionResult> Remove(int flightId)
    {
        var fav = await _db.Favorites.FirstOrDefaultAsync(x => x.UserId == UserId && x.FlightId == flightId);
        if (fav is null) return NotFound();
        _db.Favorites.Remove(fav);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
