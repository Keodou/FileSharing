using FileSharing.WebApi.Domain.Entities;
using FileSharing.WebApi.DTO;

namespace FileSharing.WebApi.Application.Interfaces;

/// <summary>
/// Определяет методы сервиса аутентификации.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Регистрация нового пользователя.
    /// </summary>
    /// <param name="request">логин и пароль пользователя.</param>
    /// <returns>новый пользователь.</returns>
    Task<User?> RegisterAsync(UserDto request);
    
    /// <summary>
    /// Аутентификация пользователя.
    /// </summary>
    /// <param name="request">логин и пароль пользователя.</param>
    /// <returns></returns>
    Task<string?> LoginAsync(UserDto request);
}