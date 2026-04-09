using System.Security.Claims;

namespace TaskManager.Application.Abstractions;

public interface ICurrentUser
{
    string UserId { get; }
    bool IsAdmin { get; }
    ClaimsPrincipal Principal { get; }
}

