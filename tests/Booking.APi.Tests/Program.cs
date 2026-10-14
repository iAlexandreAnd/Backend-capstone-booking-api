using System.Net.Http.Json;
using Booking.Api.DTO;
using Booking.Application;
using Booking.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
    public FakeUserQueries FakeQueries { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, configuration) =>
        {
            // Program requires a connection string during startup, even though
            // the test replaces the query service and doesn't connect to a database.
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:BookingDb"] =
                    "Host=localhost;Database=booking_test;Username=test;Password=test"
            });
        });

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IUserQueries>();
            services.AddSingleton<IUserQueries>(FakeQueries);
        });
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


