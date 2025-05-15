using FileSharing.Tests.Helpers;
using FileSharing.WebApi.Application.Enums;
using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.Application.Services;
using FileSharing.WebApi.Domain.Entities;
using FileSharing.WebApi.DTO;
using Microsoft.AspNetCore.Identity;
using Moq;

namespace FileSharing.Tests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task RegisterAsync_WhenUserDoesNotExist_ShouldReturnNewUser()
    {
        // Arrange
        var dbContext = TestDbContextFactory.Create();
        var mockTokenService = new Mock<ITokenService>();
        var authService = new AuthService(dbContext, mockTokenService.Object);
        var request = new UserDto
        {
            Username = "testuser",
            Password = "password"
        };

        // Act
        var result = await authService.RegisterAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);
        Assert.Equal(UserRole.User, result.Role);
    }

    [Fact]
    public async Task RegisterAsync_WhenUsernameExists_ShouldReturnNull()
    {
        // Arrange
        var dbContext = TestDbContextFactory.Create();

        var existUser = new User()
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = "hashedpassword",
            Role = UserRole.User
        };
        dbContext.Users.Add(existUser);
        await dbContext.SaveChangesAsync();

        var mockTokenService = new Mock<ITokenService>();
        var authService = new AuthService(dbContext, mockTokenService.Object);
        var request = new UserDto
        {
            Username = "testuser",
            Password = "password"
        };
        
        // Act
        var result = await authService.RegisterAsync(request);
        
        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ShouldReturnToken()
    {
        // Arrange
        var dbContext = TestDbContextFactory.Create();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "password"),
            Role = UserRole.User
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var mockTokenService = new Mock<ITokenService>();
        mockTokenService.Setup(t => t.CreateToken(It.IsAny<User>()))
            .Returns("fake-jwt-token");
        var authService = new AuthService(dbContext, mockTokenService.Object);
        var request = new UserDto
        {
            Username = "testuser",
            Password = "password"
        };
        
        // Act
        var result = await authService.LoginAsync(request);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal("fake-jwt-token", result);
    }

    [Fact]
    public async Task LoginAsync_WithInvalidPassword_ShouldReturnNull()
    {
        // Arrange
        var dbContext = TestDbContextFactory.Create();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            PasswordHash = new PasswordHasher<User>().HashPassword(null!, "password"),
            Role = UserRole.User
        };
        dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();

        var mockTokenService = new Mock<ITokenService>();
        var authService = new AuthService(dbContext, mockTokenService.Object);
        var request = new UserDto
        {
            Username = "testuser",
            Password = "wrongpassword"
        };
        
        // Act
        var result = await authService.LoginAsync(request);
        
        // Assert
        Assert.Null(result);
    }
}