namespace FlightBooking.Api.Dtos;

public record BookingRequest(int FlightId, string PassengerName, int SeatCount, string PaymentMethod,
    string? CardLastFour, string CaptchaId, string CaptchaAnswer);
