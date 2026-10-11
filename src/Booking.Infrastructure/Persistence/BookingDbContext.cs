using Booking.Domain;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class BookingDbContext(DbContextOptions<BookingDbContext> options)
    : DbContext(options)
{
    public DbSet<BookingDetails> Bookings => Set<BookingDetails>();
    public DbSet<User> Users => Set<User>();
}