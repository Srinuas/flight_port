using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FlightBooking.Api.Models;
using Microsoft.IdentityModel.Tokens;

namespace FlightBooking.Api.Services;

public class TokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration) => _configuration = configuration;

    public string Create(User user)
    {
        var key = _configuration["JWT_KEY"]
                  ?? Environment.GetEnvironmentVariable("JWT_KEY")
                  ?? throw new InvalidOperationException("JWT_KEY is missing.");

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
