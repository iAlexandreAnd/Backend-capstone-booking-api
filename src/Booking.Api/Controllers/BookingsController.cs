using Booking.Application;
using Booking.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("bookings")]
public sealed class BookingsController(IBookingQueries bookingQueries)
    : ControllerBase
{
    [HttpGet("confirmed")]
    public async Task<ActionResult<IReadOnlyList<BookingDetails>>> GetConfirmed(
        CancellationToken cancellationToken)
    {
        var bookings = await bookingQueries.GetConfirmedAsync(cancellationToken);
        return Ok(bookings);
    }
}


