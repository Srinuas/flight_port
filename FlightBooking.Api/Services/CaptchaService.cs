using Microsoft.Extensions.Caching.Memory;

namespace FlightBooking.Api.Services;

public class CaptchaService
{
    private readonly IMemoryCache _cache;

    public CaptchaService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public CaptchaResponse Create()
    {
        var random = Random.Shared;

        var a = random.Next(2, 10);
        var b = random.Next(2, 10);

        var id = Guid.NewGuid().ToString("N");

        _cache.Set(
            $"captcha:{id}",
            a + b,
            TimeSpan.FromMinutes(5)
        );

        return new CaptchaResponse(
            id,
            $"{a} + {b} = ?"
        );
    }

    public bool Validate(string id, string answer)
    {
        if (string.IsNullOrWhiteSpace(id) ||
            string.IsNullOrWhiteSpace(answer))
        {
            return false;
        }

        if (!_cache.TryGetValue<int>(
                $"captcha:{id}",
                out var expected))
        {
            return false;
        }

        // CAPTCHA can only be used once.
        _cache.Remove($"captcha:{id}");

        return int.TryParse(
            answer.Trim(),
            out var actual
        ) && actual == expected;
    }
}

public record CaptchaResponse(
    string Id,
    string Question
);
