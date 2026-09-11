using System.Security.Claims;
using FlightBooking.Api.Data;
using FlightBooking.Api.Dtos;
using FlightBooking.Api.Models;
using FlightBooking.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FlightBooking.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/orders")]
public class OrdersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly CaptchaService _captcha;

    private int UserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public OrdersController(AppDbContext db, IMemoryCache cache)
    {
        _db = db;
        _captcha = new CaptchaService(cache);
    }

    [HttpGet]
    public async Task<IActionResult> History()
    {
        var orders = await _db.Orders
            .Where(x => x.UserId == UserId)
            .Include(x => x.Items).ThenInclude(x => x.Flight)
            .Include(x => x.Payment)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(o => new
            {
                o.Id,
                o.OrderNumber,
                o.TotalAmount,
                o.Status,
                o.CreatedAtUtc,
                Items = o.Items.Select(i => new
                {
                    i.Id,
                    i.PassengerName,
                    i.SeatCount,
                    i.UnitPrice,
                    Flight = i.Flight == null ? null : new
                    {
                        i.Flight.Id,
                        i.Flight.FlightNumber,
                        i.Flight.Origin,
                        i.Flight.OriginCode,
                        i.Flight.Destination,
                        i.Flight.DestinationCode,
                        i.Flight.DepartureUtc,
                        i.Flight.ArrivalUtc,
                        i.Flight.Airline,
                        i.Flight.Price
                    }
                }),
                Payment = o.Payment == null ? null : new
                {
                    o.Payment.Method,
                    o.Payment.Status,
                    o.Payment.Reference,
                    o.Payment.LastFour,
                    o.Payment.PaidAtUtc
                }
            })
            .ToListAsync();

        return Ok(orders);
    }

    [HttpPost]
    public async Task<IActionResult> Book(BookingRequest request)
    {
        if (request.SeatCount < 1 || request.SeatCount > 9)
            return BadRequest("Passengers must be between 1 and 9.");

        if (!_captcha.Validate(request.CaptchaId, request.CaptchaAnswer))
            return BadRequest("Invalid or expired payment CAPTCHA.");

        var flight = await _db.Flights.FirstOrDefaultAsync(x => x.Id == request.FlightId);
        if (flight is null) return NotFound("Flight not found.");
        if (flight.AvailableSeats < request.SeatCount) return BadRequest("Not enough seats available.");

        var total = flight.Price * request.SeatCount;

        await using var transaction = await _db.Database.BeginTransactionAsync();

        flight.AvailableSeats -= request.SeatCount;

        var order = new Order
        {
            UserId = UserId,
            OrderNumber = $"FB-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(100000, 999999)}",
            TotalAmount = total,
            Status = "Confirmed",
            Items = new List<OrderItem>
            {
                new()
                {
                    FlightId = flight.Id,
                    PassengerName = request.PassengerName.Trim(),
                    SeatCount = request.SeatCount,
                    UnitPrice = flight.Price
                }
            },
            Payment = new Payment
            {
                Method = request.PaymentMethod,
                Status = "Paid",
                Reference = $"PAY-{Guid.NewGuid().ToString("N")[..10].ToUpperInvariant()}",
                LastFour = request.CardLastFour?.Length == 4 ? request.CardLastFour : "",
                PaidAtUtc = DateTime.UtcNow
            }
        };

        _db.Orders.Add(order);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return Ok(order.OrderNumber);
    }
}
