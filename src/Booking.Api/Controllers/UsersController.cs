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

    [HttpPost]
    public async Task<ActionResult<IReadOnlyList<User>>> CreateUser(
        [FromQuery] string lastName,
        [FromQuery] string firstName,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(lastName))
        {
            return BadRequest("The lastName query parameter is required.");
        }
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return BadRequest("The firstName query parameter is required.");
        }

        var users = await userQueries.CreateUserAsync(
            lastName.Trim(),
            firstName.Trim(),
            cancellationToken);

        return Ok(users);
    }
}



/*

using Booking.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Api.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController(IUserQueries userQueries) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> FindByLastName(
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

        var response = users
            .Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName
            })
            .ToList();

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser(
        [FromBody] CreateUserRequestDto request,
        CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var createdUser = await userQueries.CreateUserAsync(
            request,
            cancellationToken);

        return CreatedAtAction(nameof(FindByLastName), new { lastName = createdUser.LastName }, createdUser);
    }
}
*/