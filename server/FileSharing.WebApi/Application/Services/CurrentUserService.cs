using System.Security.Claims;
using FileSharing.WebApi.Application.Enums;
using FileSharing.WebApi.Application.Interfaces;

namespace FileSharing.WebApi.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; }
    public UserRole Role { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var user = httpContextAccessor.HttpContext?.User;
        var userIdStr = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var roleStr = user?.FindFirst(ClaimTypes.Role)?.Value;
        
        // TODO: переписать условие, вознкиает конфликт, если человек хочет скачать паблик файл
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            throw new UnauthorizedAccessException("Пользователь не авторизован или идентификатор недействителен");
        }
        if (!Enum.TryParse<UserRole>(roleStr, ignoreCase: true, out var userRole))
        {
            throw new UnauthorizedAccessException("Роль пользователя не распознана");
        }

        UserId = userId; 
        Role = userRole;
    }

    public CurrentUser GetCurrentUser()
    {
        return new CurrentUser(UserId, Role);
    }
}