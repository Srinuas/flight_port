using System.Security.Claims;
using FlightBooking.Api.Data;
using FlightBooking.Api.Dtos;
using FlightBooking.Api.Models;
using FlightBooking.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace FlightBooking.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;
    private readonly CaptchaService _captcha;

    public AuthController(AppDbContext db, TokenService tokens, IMemoryCache cache)
    {
        _db = db;
        _tokens = tokens;
        _captcha = new CaptchaService(cache);
    }

    [HttpGet("captcha")]
    public IActionResult Captcha()
    {
        var result = _captcha.Create();
        return Ok(result);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (!ValidPassword(request.Password)) return BadRequest("Password must be 6-12 characters.");
        if (!IsValidEmail(request.Email)) return BadRequest("Use a valid Gmail address.");
        if (!request.Email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase))
            return BadRequest("Registration currently accepts Gmail addresses only.");
        if (!request.Phone.All(char.IsDigit) || request.Phone.Length < 10 || request.Phone.Length > 15)
            return BadRequest("Enter a valid phone number.");
        if (!_captcha.Validate(request.CaptchaId, request.CaptchaAnswer)) return BadRequest("Invalid or expired CAPTCHA.");

        if (await _db.Users.AnyAsync(x => x.Username == request.Username))
            return Conflict("Username is already registered.");
        if (await _db.Users.AnyAsync(x => x.Email == request.Email.ToLower()))
            return Conflict("Email is already registered.");

        var user = new User
        {
            Username = request.Username.Trim(),
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Dob = request.Dob,
            Phone = request.Phone.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return Ok(ToResponse(user));
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        if (!ValidPassword(request.Password)) return BadRequest("Password must be 6-12 characters.");
        if (!_captcha.Validate(request.CaptchaId, request.CaptchaAnswer)) return BadRequest("Invalid or expired CAPTCHA.");

        var identifier = request.Identifier.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(x =>
            x.Username.ToLower() == identifier || x.Email.ToLower() == identifier);

        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return Unauthorized("Invalid username/email or password.");

        return Ok(ToResponse(user));
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
    {
        if (!ValidPassword(request.NewPassword)) return BadRequest("New password must be 6-12 characters.");
        if (!_captcha.Validate(request.CaptchaId, request.CaptchaAnswer)) return BadRequest("Invalid or expired CAPTCHA.");

        var identifier = request.Identifier.Trim().ToLowerInvariant();
        var user = await _db.Users.FirstOrDefaultAsync(x =>
            (x.Username.ToLower() == identifier || x.Email.ToLower() == identifier)
            && x.Phone == request.Phone.Trim());

        if (user is null) return BadRequest("Account details could not be verified.");

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _db.SaveChangesAsync();
        return Ok("Password reset successfully.");
    }

    private AuthResponse ToResponse(User user)
        => new(_tokens.Create(user), user.Id, user.Username, user.FirstName, user.Email);

    private static bool ValidPassword(string password) =>
        !string.IsNullOrWhiteSpace(password) && password.Length >= 6 && password.Length <= 12;

    private static bool IsValidEmail(string email) =>
        !string.IsNullOrWhiteSpace(email) &&
        email.Contains('@') &&
        email.EndsWith("@gmail.com", StringComparison.OrdinalIgnoreCase);
}
