using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Booking.Api.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Booking.Api.Controllers;

[ApiController]
[Route("auth")]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    IConfiguration configuration) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Unauthorized();
        }

        var jwtSection = configuration.GetSection("Jwt");
        var issuer = jwtSection["Issuer"]!;
        var audience = jwtSection["Audience"]!;
        var signingKey = jwtSection["SigningKey"]!;
        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(
            configuration.GetValue<int>("Jwt:AccessTokenMinutes"));
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id.ToString()),
            new(ClaimTypes.Email, user.Email ?? string.Empty)
        };

        var roles = await userManager.GetRolesAsync(user);
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: issuedAt.UtcDateTime,
            expires: expiresAt.UtcDateTime,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey)),
                SecurityAlgorithms.HmacSha256));

        var response = new LoginResponse(
            new JwtSecurityTokenHandler().WriteToken(token),
            expiresAt);

        return Ok(response);
    }
}
