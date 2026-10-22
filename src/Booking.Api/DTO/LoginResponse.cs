namespace Booking.Api.DTO;

/// <summary>An access token and its expiration time.</summary>
public sealed record LoginResponse(
    string AccessToken,
    DateTimeOffset ExpiresAt);
