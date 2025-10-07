using FileSharing.WebApi.Domain.Entities;
using FileSharing.WebApi.DTO;

namespace FileSharing.WebApi.Application.Interfaces;

/// <summary>
/// Определяет методы сервиса аутентификации.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Реализует логику регистрации нового пользователя.
    /// </summary>
    /// <param name="request">логин и пароль пользователя.</param>
    /// <returns>новый пользователь.</returns>
    Task<User?> RegisterAsync(UserDto request);
    
    /// <summary>
    /// Аутентификация пользователя.
    /// </summary>
    /// <param name="request">логин и пароль пользователя.</param>
    /// <returns>JWT токен в формате строки.</returns>
    Task<AuthResponse?> LoginAsync(UserDto request);
}

public class AuthResponse
{
    public string Token { get; set; }
}