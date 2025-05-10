using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.Application.Services;
using FileSharing.WebApi.Infrastructure.Persistence;
using FileSharing.WebApi.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.WebApi.Extensions;

public static class DependencyInjection
{
    public static void AddAppServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddDbContext<FileSharingDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("FileSharingConnection")));
        services.AddSingleton<StorageInitializer>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IFileService, FileService>();
    }
}