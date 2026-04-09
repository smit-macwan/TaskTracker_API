using System.Security.Claims;
using TaskManager.Application.Abstractions;

namespace TaskManager.WebApi.Auth;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public ClaimsPrincipal Principal =>
        accessor.HttpContext?.User ?? new ClaimsPrincipal(new ClaimsIdentity());

    public string UserId =>
        Principal.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new InvalidOperationException("UserId claim missing.");

    public bool IsAdmin =>
        Principal.IsInRole("admin");
}

