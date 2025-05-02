using FileSharing.WebApi.Application.Enums;

namespace FileSharing.WebApi.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    UserRole Role { get; }

    CurrentUser GetCurrentUser();
}

public record CurrentUser(Guid Id, UserRole Role);