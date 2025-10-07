using FileSharing.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.Tests.Helpers;

public static class TestDbContextFactory
{
    public static FileSharingDbContext Create()
    {
        var options = new DbContextOptionsBuilder<FileSharingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new FileSharingDbContext(options);
    }
}