using FileSharing.WebApi.Application.Enums;
using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.Domain.Entities;
using FileSharing.WebApi.DTO;
using FileSharing.WebApi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.WebApi.Application.Services;

/// <summary>
/// Реализация логики аутентификации.
/// </summary>
/// <param name="context">контекст базы данных.</param>
/// <param name="tokenService">сервис для определения JWT токена.</param>
public class AuthService(FileSharingDbContext context, ITokenService tokenService) : IAuthService
{
    /// <inheritdoc />
    public async Task<User?> RegisterAsync(UserDto request)
    {
        if (await context.Users.AnyAsync(u => u.Username == request.Username))
            return null;

        var user = new User();
        var hashedPassword = new PasswordHasher<User>()
            .HashPassword(user, request.Password);

        user.Username = request.Username;
        user.PasswordHash = hashedPassword;
        user.Role = UserRole.User;

        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    /// <inheritdoc />
    public async Task<string?> LoginAsync(UserDto request)
    {
        var user = await context.Users.FirstOrDefaultAsync(u => u.Username == request.Username);
        
        if (user is null)
            return null;
        
        if (new PasswordHasher<User>().VerifyHashedPassword(user, user.PasswordHash, request.Password)
            == PasswordVerificationResult.Failed)
        {
            return null;
        }

        var token = tokenService.CreateToken(user);
        return token;
    }
}