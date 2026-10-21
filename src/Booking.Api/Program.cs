using Booking.Application;
using Booking.Infrastructure.Persistence;
using Booking.Infrastructure.Queries;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

DotNetEnv.Env.Load();

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
await CreateAdminUser(app, builder);

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

static async Task CreateAdminUser(WebApplication app, WebApplicationBuilder builder)
{
    await using var scope = app.Services.CreateAsyncScope();
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<ApplicationRole>>();
    
    const string roleName = "Admin";
    if (!await roleManager.RoleExistsAsync(roleName))
    {
        throw new InvalidOperationException($"Role '{roleName}' does not exist.");
    }

    var email = builder.Configuration["Admin:Email"]
    ?? throw new InvalidOperationException("Admin:Email is missing.");
    var password = builder.Configuration["Admin:Password"]
    ?? throw new InvalidOperationException("Admin:Password is missing.");

    var user = await userManager.FindByEmailAsync(email);

    if (user is null)
    {
        user = new ApplicationUser
        {
            UserName = email,
            Email = email,
            FirstName = "Admin",
            LastName = "Admin"
        };

        var createResult = await userManager.CreateAsync(user, password);
        if (!createResult.Succeeded)
        {
            var errors = string.Join("; ",
                createResult.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Could not create admin user: {errors}");
        }
    }

    if (!await userManager.IsInRoleAsync(user, roleName))
    {
        var roleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            var errors = string.Join("; ",
                roleResult.Errors.Select(error => error.Description));
            throw new InvalidOperationException($"Could not add admin to role: {errors}");
        }
    }
}



public partial class Program { }