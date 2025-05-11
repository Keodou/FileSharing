using System.Security.Claims;
using FileSharing.WebApi.Application.Enums;
using FileSharing.WebApi.Application.Interfaces;

namespace FileSharing.WebApi.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid? UserId { get; }
    public UserRole? Role { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userIdStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleStr = user?.FindFirst(ClaimTypes.Role)?.Value;

        if (Guid.TryParse(userIdStr, out var userId))
            UserId = userId;
        
        if (Enum.TryParse<UserRole>(roleStr, ignoreCase: true, out var userRole))
            Role = userRole;
    }

    public bool IsAuthenticated => UserId.HasValue;
    public bool IsAdmin => Role == UserRole.Admin;

    public CurrentUser? GetCurrentUser()
    {
        return !IsAuthenticated ? null : new CurrentUser(UserId.Value, Role ?? UserRole.User);
    }
}