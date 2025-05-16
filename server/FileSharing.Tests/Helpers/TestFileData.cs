using FileSharing.WebApi.Domain.Entities;

namespace FileSharing.Tests.Helpers;

public static class TestFileData
{
    public static List<FileModel> GetTestFiles(Guid userId)
    {
        return
        [
            new FileModel
            {
                Id = Guid.NewGuid(),
                FileName = "file1.txt",
                OwnerId = userId,
                UploadDate = DateTime.UtcNow,
                Size = 1024,
                Path = "path/1",
                ContentType = "text/plain"
            },

            new FileModel
            {
                Id = Guid.NewGuid(),
                FileName = "file2.txt",
                OwnerId = userId,
                UploadDate = DateTime.UtcNow,
                Size = 2048,
                Path = "path/2",
                ContentType = "text/plain"
            },

            new FileModel
            {
                Id = Guid.NewGuid(),
                FileName = "userfile.txt",
                OwnerId = Guid.NewGuid(),
                UploadDate = DateTime.UtcNow,
                Size = 512,
                Path = "other/path",
                ContentType = "text/plain"
            }
        ];
    }
}