using FileSharing.WebApi.Entities;
using FileSharing.WebApi.Models;

namespace FileSharing.WebApi.Services;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDTO request);
    Task<string?> LoginAsync(UserDTO request);
}