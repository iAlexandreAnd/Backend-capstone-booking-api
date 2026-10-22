using Booking.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Booking.Infrastructure.Persistence;

public sealed class BookingDbContext(
    DbContextOptions<BookingDbContext> options)
    : IdentityDbContext<ApplicationUser, ApplicationRole, int>(options)
{
    public DbSet<BookingDetails> Bookings => Set<BookingDetails>();
    public DbSet<User> DomainUsers => Set<User>();

    public DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();

    public DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Add your app-specific entity configuration here.
    }
}