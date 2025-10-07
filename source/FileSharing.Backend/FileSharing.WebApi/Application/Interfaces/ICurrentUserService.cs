using FileSharing.WebApi.Application.Enums;

namespace FileSharing.WebApi.Application.Interfaces;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    UserRole? Role { get; }
    CurrentUser? GetCurrentUser();
    bool IsAuthenticated { get; }
    bool IsAdmin { get; }
}

public record CurrentUser(Guid Id, UserRole Role);