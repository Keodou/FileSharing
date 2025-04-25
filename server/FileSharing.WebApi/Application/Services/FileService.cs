using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.Domain.Entities;
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

    public async Task<(byte[], string, string)> DownloadFileAsync(Guid userId, Guid fileId)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null || file.OwnerId != userId || !File.Exists(file.Path))
            throw new FileNotFoundException("Файл не найден или доступ запрещен.");

        var bytes = await File.ReadAllBytesAsync(file.Path);
        return (bytes, file.ContentType, file.FileName);
    }

    public async Task<(bool, string)> DeleteFileAsync(Guid userId, Guid fileId)
    {
        var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null || file.OwnerId != userId)
            return (false, "Файл не найден или доступ запрещен.");
        
        if (File.Exists(file.Path))
            File.Delete(file.Path);

        _dbContext.Files.Remove(file);
        await _dbContext.SaveChangesAsync();
        return (true, $"{file.FileName} был удален.");
    }
}