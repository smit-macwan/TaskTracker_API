using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TaskManager.Infrastructure.Identity;
using TaskManager.WebApi.Auth;

namespace TaskManager.WebApi.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    TokenService tokenService)
    : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email.Trim().ToLowerInvariant(),
            Email = request.Email.Trim().ToLowerInvariant()
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Registration failed.", errors = result.Errors.Select(e => e.Description) });
        }

        await userManager.AddToRoleAsync(user, "user");

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.CreateToken(user, roles);
        return Ok(new AuthResponse(token));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var ok = await userManager.CheckPasswordAsync(user, request.Password);
        if (!ok)
        {
            return Unauthorized(new { message = "Invalid credentials." });
        }

        var roles = await userManager.GetRolesAsync(user);
        var token = tokenService.CreateToken(user, roles);
        return Ok(new AuthResponse(token));
    }
}

