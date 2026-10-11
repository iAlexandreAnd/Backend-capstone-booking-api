using Booking.Domain;

namespace Booking.Application;

public interface IBookingQueries
{
    Task<IReadOnlyList<BookingDetails>> GetConfirmedAsync(
        CancellationToken cancellationToken);
}