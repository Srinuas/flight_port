using FlightBooking.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlightBooking.Api.Controllers;

[ApiController]
[Route("api/health")]
public class HealthController : ControllerBase
{
    private readonly AppDbContext _db;

    public HealthController(AppDbContext db) => _db = db;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var dbOk = await _db.Database.CanConnectAsync();
        return Ok(new { status = dbOk ? "healthy" : "degraded", database = dbOk ? "connected" : "unavailable" });
    }
}
