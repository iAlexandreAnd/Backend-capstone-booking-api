using Booking.Application;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var connectionString = builder.Configuration.GetConnectionString("BookingDb")
    ?? throw new InvalidOperationException(
        "Connection string 'BookingDb' is missing.");

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentityCore<ApplicationUser>()
    .AddRoles<ApplicationRole>()
    .AddEntityFrameworkStores<BookingDbContext>();

builder.Services.AddScoped<IUserQueries, UserQueries>();
builder.Services.AddScoped<IBookingQueries, BookingQueries>();

var app = builder.Build();

await SeedRolesAsync(app);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGet("/health", () => Results.Ok("Healthy"));

app.Run();



//creates the application user roles only once
static async Task SeedRolesAsync(WebApplication app)
{
    await using var scope = app.Services.CreateAsyncScope();
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<ApplicationRole>>();

    foreach (var roleName in new[] { "Admin", "User" })
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            continue;
        }

        var result = await roleManager.CreateAsync(
            new ApplicationRole { Name = roleName });

        if (!result.Succeeded)
        {
            var errors = string.Join(
                "; ",
                result.Errors.Select(error => error.Description));
            throw new InvalidOperationException(
                $"Could not create role '{roleName}': {errors}");
        }
    }
}

public partial class Program { }