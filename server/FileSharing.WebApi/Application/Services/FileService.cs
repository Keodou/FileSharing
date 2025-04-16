using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.WebApi.Application.Services;

public class FileService : IFileService
{
    private readonly FileSharingDbContext _dbContext;
    private readonly string _uploadsFolder;

    public FileService(FileSharingDbContext dbContext, IWebHostEnvironment emv)
    {
        _dbContext = dbContext;
        _uploadsFolder = Path.Combine(emv.WebRootPath, "Uploads");
        if (!Directory.Exists(_uploadsFolder))
            Directory.CreateDirectory(_uploadsFolder);
    }

    public async Task<IEnumerable<FileModel>> GetUserFilesAsync(Guid userId)
    {
        return await _dbContext.Files.Where(f => f.OwnerId == userId).ToListAsync();
    }

    public async Task<(bool, string, object?)> UploadFileAsync(Guid userId, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return (false, "Файл не найден", null);
        
        var filePath = Path.Combine(_uploadsFolder, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var fileModel = new FileModel
        {
            Id = Guid.NewGuid(),
            FileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length,
            UploadDate = DateTime.UtcNow,
            OwnerId = userId,
            Path = filePath
        };

        _dbContext.Files.Add(fileModel);
        await _dbContext.SaveChangesAsync();
        return (true, "Файл успешно загружен", new { file.FileName, file.Length });
    }

    public Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadFileAsync(Guid userId, Guid fileId)
    {
        throw new NotImplementedException();
    }

    public Task<(bool Success, string Message)> DeleteFileAsync(Guid userId, Guid fileId)
    {
        throw new NotImplementedException();
    }
}