using System.ComponentModel.DataAnnotations;

namespace Booking.Api.DTO;

/// <summary>Credentials used to obtain an access token.</summary>
public sealed record LoginRequest
{
    [Required, EmailAddress]
    public required string Email { get; init; }

    [Required]
    public required string Password { get; init; }
}
