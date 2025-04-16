namespace FileSharing.WebApi.Application.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
}