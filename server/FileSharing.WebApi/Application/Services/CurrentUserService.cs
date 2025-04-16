using System.Security.Claims;
using FileSharing.WebApi.Application.Interfaces;

namespace FileSharing.WebApi.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; }

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        var userIdStr = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrWhiteSpace(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            throw new UnauthorizedAccessException("Пользователь не авторизован или идентификатор недействителен");
        }

        UserId = userId;
    }
}