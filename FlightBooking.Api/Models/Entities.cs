namespace FlightBooking.Api.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public DateTime Dob { get; set; }
    public string Phone { get; set; } = "";
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}

public class Flight
{
    public int Id { get; set; }
    public string FlightNumber { get; set; } = "";
    public string Origin { get; set; } = "";
    public string OriginCode { get; set; } = "";
    public string Destination { get; set; } = "";
    public string DestinationCode { get; set; } = "";
    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }
    public string Airline { get; set; } = "";
    public string AirlineCode { get; set; } = "";
    public decimal Price { get; set; }
    public int AvailableSeats { get; set; }
    public string ImageUrl { get; set; } = "";

    public static Flight Create(string no, string origin, string originCode, string dest, string destCode,
        DateTime dep, DateTime arr, string airline, string airlineCode, decimal price, int seats, string imageUrl)
        => new()
        {
            FlightNumber = no, Origin = origin, OriginCode = originCode, Destination = dest, DestinationCode = destCode,
            DepartureUtc = dep, ArrivalUtc = arr, Airline = airline, AirlineCode = airlineCode,
            Price = price, AvailableSeats = seats, ImageUrl = imageUrl
        };
}

public class Favorite
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FlightId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Flight? Flight { get; set; }
}

public class Order
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string OrderNumber { get; set; } = "";
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Confirmed";
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public List<OrderItem> Items { get; set; } = new();
    public Payment? Payment { get; set; }
}

public class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int FlightId { get; set; }
    public string PassengerName { get; set; } = "";
    public int SeatCount { get; set; }
    public decimal UnitPrice { get; set; }
    public Flight? Flight { get; set; }
    public Order? Order { get; set; }
}

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public string Method { get; set; } = "";
    public string Status { get; set; } = "Paid";
    public string Reference { get; set; } = "";
    public string LastFour { get; set; } = "";
    public DateTime PaidAtUtc { get; set; } = DateTime.UtcNow;
    public Order? Order { get; set; }
}
