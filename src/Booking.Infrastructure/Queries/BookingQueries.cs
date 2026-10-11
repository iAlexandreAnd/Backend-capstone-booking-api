using Booking.Application;
using Booking.Domain;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Queries;

public sealed class BookingQueries(BookingDbContext dbContext) : IBookingQueries
{
    public async Task<IReadOnlyList<BookingDetails>> GetConfirmedAsync(
        CancellationToken cancellationToken)
    {
        return await dbContext.Bookings
            .AsNoTracking()
            .Where(booking => booking.Status == "Confirmed")
            .ToListAsync(cancellationToken);
    }

}