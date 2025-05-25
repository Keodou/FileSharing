using FileSharing.Tests.Helpers;
using FileSharing.Tests.Utils;
using FileSharing.WebApi.Application.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit.Sdk;

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
        // Arrange
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

    [Fact]
    public async Task GetUserFileAsync_WhenDatabaseEmpty_ReturnsEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestDbContextFactory.Create();
        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.WebRootPath).Returns("C:\\uploads");
        var fileService = new FileService(dbContext, mockEnv.Object);
        
        // Act
        var result = await fileService.GetUserFilesAsync(userId);
        
        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task UploadFileAsync_WhenValidFile_ShouldSave()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var dbContext = TestDbContextFactory.Create();
        
        var uploadsRoot = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var uploadsFolder = Path.Combine(uploadsRoot, "Uploads");
        Directory.CreateDirectory(uploadsFolder);

        var mockEnv = new Mock<IWebHostEnvironment>();
        mockEnv.Setup(e => e.WebRootPath).Returns(uploadsRoot);
        var fileService = new FileService(dbContext, mockEnv.Object);
        var testFile = FileTestUtils.CreateMockFile("Some content", "example.txt");
        
        // Act
        var (success, message, result) = await fileService.UploadFileAsync(userId, testFile);
        
        // Assert
        Assert.True(success);
        Assert.Equal("Файл успешно загружен", message);
        Assert.NotNull(result);
        
        Directory.Delete(uploadsRoot, recursive: true);
    }
}