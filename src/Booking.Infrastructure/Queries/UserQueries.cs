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

    public async Task<IReadOnlyList<User>> CreateUserAsync(
        string lastName,
        string firstName,
        CancellationToken cancellationToken)
    {
        var user = new User
        {
            LastName = lastName,
            FirstName = firstName,
            Bookings = new List<BookingDetails>()
        };

        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new[] { user };
    }
}