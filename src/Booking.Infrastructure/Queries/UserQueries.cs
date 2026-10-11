using Booking.Application;
using Booking.Domain;
using Booking.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Queries;

public sealed class UserQueries(BookingDbContext dbContext) : IUserQueries
{
    public async Task<IReadOnlyList<User>> FindByLastNameAsync(
        string lastName,
        CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(user => user.LastName == lastName)
            .ToListAsync(cancellationToken);
    }
}