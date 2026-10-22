using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using Booking.Api.DTO;
using Booking.Application;
using Booking.Domain;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Booking.Api.Tests;

public class UsersControllerTests
{
    [Fact]
    public async Task CreateUserWithDTOTestCase()
    {
        using var factory = new UsersApiFactory();
        using var client = factory.CreateClient();

        using var response = await client.PostAsJsonAsync(
            "/users",
            new CreateUserRequestDto
            {
                FirstName = "Ada",
                LastName = "Lovelace"
            });

        response.EnsureSuccessStatusCode();

        Assert.Equal("Ada", factory.FakeQueries.ReceivedFirstName);
        Assert.Equal("Lovelace", factory.FakeQueries.ReceivedLastName);
    }
}

internal sealed class UsersApiFactory : WebApplicationFactory<global::Program>
{
    private readonly Dictionary<string, string?> originalEnvironmentVariables = [];

    public FakeUserQueries FakeQueries { get; } = new();

    public UsersApiFactory()
    {
        SetEnvironmentVariable("DOTNET_ENVIRONMENT", "Testing");
        SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Testing");
        SetEnvironmentVariable(
            "ConnectionStrings__BookingDb",
            "Host=localhost;Database=booking_test;Username=test;Password=test");
        SetEnvironmentVariable("Jwt__SigningKey", "test-signing-key-that-is-at-least-32-bytes");
        SetEnvironmentVariable("Admin__Email", "admin@example.com");
        SetEnvironmentVariable("Admin__Password", "Test-password-123!");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            // Program requires a connection string during startup, even though
            // the test replaces the query service and doesn't connect to a database.
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:BookingDb"] =
                    "Host=localhost;Database=booking_test;Username=test;Password=test"
            });

            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Issuer"] = "Booking.Api.Tests",
                ["Jwt:Audience"] = "Booking.Api.Tests",
                ["Jwt:SigningKey"] = "test-signing-key-that-is-at-least-32-bytes",
                ["Jwt:AccessTokenMinutes"] = "60",
                ["Admin:Email"] = "admin@example.com",
                ["Admin:Password"] = "Test-password-123!"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.AddAuthentication("Test")
                .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>(
                    "Test",
                    _ => { });
            services.RemoveAll<IUserQueries>();
            services.AddSingleton<IUserQueries>(FakeQueries);
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            foreach (var (name, value) in originalEnvironmentVariables)
            {
                Environment.SetEnvironmentVariable(name, value);
            }
        }
    }

    private void SetEnvironmentVariable(string name, string value)
    {
        originalEnvironmentVariables[name] = Environment.GetEnvironmentVariable(name);
        Environment.SetEnvironmentVariable(name, value);
    }
}

internal sealed class TestAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Role, "Admin")],
            Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}

internal sealed class FakeUserQueries : IUserQueries
{
    public string? ReceivedFirstName { get; private set; }
    public string? ReceivedLastName { get; private set; }

    public Task<IReadOnlyList<User>> FindByLastNameAsync(
        string lastName,
        CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyList<User>>(Array.Empty<User>());

    public Task<User> CreateUserAsync(
        string lastName,
        string firstName,
        CancellationToken cancellationToken)
    {
        ReceivedLastName = lastName;
        ReceivedFirstName = firstName;

        return Task.FromResult(new User
        {
            Id = 42,
            LastName = lastName,
            FirstName = firstName,
            Bookings = []
        });
    }
}
