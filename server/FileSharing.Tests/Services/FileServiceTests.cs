using FileSharing.Tests.Helpers;
using FileSharing.WebApi.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Moq;

namespace FileSharing.Tests.Services;

public class FileServiceTests
{
    [Fact]
    public async Task GetUserFilesAsync_WhenCalled_ReturnsOnlyUsersFiles()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var testData = TestFileData.GetTestFiles(userId);
        var dbContext = TestDbContextFactory.Create();
        dbContext.Files.AddRange(testData);
        await dbContext.SaveChangesAsync();
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.WebRootPath).Returns("C:\\uploads");
        var fileService = new FileService(dbContext, mockEnv.Object);
        
        // Act
        var result = await fileService.GetUserFilesAsync(userId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.All(result, f => Assert.Equal(userId, f.OwnerId));
    }

    [Fact]
    public async Task GetUserFileAsync_WhenNoFiles_ReturnsEmptyList()
    {
        var userId = Guid.NewGuid();
        var unknownId = Guid.NewGuid();
        var testData = TestFileData.GetTestFiles(unknownId);
        var dbContext = TestDbContextFactory.Create();
        dbContext.Files.AddRange(testData);
        await dbContext.SaveChangesAsync();
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.WebRootPath).Returns("C:\\uploads");
        var fileService = new FileService(dbContext, mockEnv.Object);
        
        // Act
        var result = await fileService.GetUserFilesAsync(userId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}