namespace FlightBooking.Api.Dtos;

public record RegisterRequest(
    string Username, string FirstName, string LastName, DateTime Dob,
    string Phone, string Email, string Password, string CaptchaId, string CaptchaAnswer);

public record LoginRequest(string Identifier, string Password, string CaptchaId, string CaptchaAnswer);

public record ForgotPasswordRequest(string Identifier, string Phone, string NewPassword, string CaptchaId, string CaptchaAnswer);

public record AuthResponse(string Token, int UserId, string Username, string FirstName, string Email);
