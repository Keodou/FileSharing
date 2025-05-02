using FileSharing.WebApi.Application.Enums;
using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.WebApi.Application.Services;

public class FileService(FileSharingDbContext dbContext, IWebHostEnvironment emv) : IFileService
{
    private readonly string _uploadsFolder = Path.Combine(emv.WebRootPath, "Uploads");

    public async Task<IEnumerable<FileModel>> GetAllFiles()
    {
        return await dbContext.Files.ToListAsync();
    }
    
    public async Task<IEnumerable<FileModel>> GetUserFilesAsync(Guid userId)
    {
        return await dbContext.Files.Where(f => f.OwnerId == userId).ToListAsync();
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

        dbContext.Files.Add(fileModel);
        await dbContext.SaveChangesAsync();
        return (true, "Файл успешно загружен", new { file.FileName, file.Length });
    }

    public async Task<(byte[], string, string)> DownloadFileAsync(Guid userId, Guid fileId)
    {
        var file = await dbContext.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null || file.OwnerId != userId || !File.Exists(file.Path))
            throw new FileNotFoundException("Файл не найден или доступ запрещен.");

        var bytes = await File.ReadAllBytesAsync(file.Path);
        return (bytes, file.ContentType, file.FileName);
    }

    public async Task<(bool, string)> DeleteFileAsync(Guid userId, UserRole userRole, Guid fileId)
    {
        var file = await dbContext.Files.FirstOrDefaultAsync(f => f.Id == fileId);

        if (file == null)
            return (false, "Файл не найден.");
        
        if (file.OwnerId != userId && userRole != UserRole.Admin)
            return (false, "Файл не найден или доступ запрещен.");
        
        if (File.Exists(file.Path))
            File.Delete(file.Path);

        dbContext.Files.Remove(file);
        await dbContext.SaveChangesAsync();
        return (true, $"{file.FileName} был удален.");
    }
}