using FileSharing.WebApi.Domain.Entities;
using FileSharing.WebApi.Models;

namespace FileSharing.WebApi.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterAsync(UserDTO request);
    Task<string?> LoginAsync(UserDTO request);
}