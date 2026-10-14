using Booking.Domain;

namespace Booking.Application;

public interface IUserQueries
{
    Task<IReadOnlyList<User>> FindByLastNameAsync(
        string lastName,
        CancellationToken cancellationToken);

    Task<User> CreateUserAsync(
        string lastName,
        string firstName,
        CancellationToken cancellationToken);
}