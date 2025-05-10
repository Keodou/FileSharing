using FileSharing.WebApi.Domain.Entities;
using FileSharing.WebApi.DTO;

namespace FileSharing.WebApi.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDto request);
    Task<string?> LoginAsync(UserDto request);
}