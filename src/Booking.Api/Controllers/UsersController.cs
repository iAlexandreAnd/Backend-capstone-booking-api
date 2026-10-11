using Booking.Application;
using Booking.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController(IUserQueries userQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<User>>> FindByLastName(
        [FromQuery] string lastName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return BadRequest("The lastName query parameter is required.");
        }

        var users = await userQueries.FindByLastNameAsync(
            lastName.Trim(),
            cancellationToken);

        return Ok(users);
    }
}