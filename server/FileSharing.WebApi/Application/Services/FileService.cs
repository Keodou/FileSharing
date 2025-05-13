using FileSharing.WebApi.Application.Enums;
using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.Domain.Entities;
using FileSharing.WebApi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FileSharing.WebApi.Application.Services;

/// <summary>
/// Осуществляет логику работы с файлами.
/// </summary>
/// <param name="dbContext">контекст базы данных.</param>
/// <param name="emv">окружение веб-хоста (для получения директории загружаемых файлов.</param>
public class FileService(FileSharingDbContext dbContext, IWebHostEnvironment emv) : IFileService
{
    private readonly string _uploadsFolder = Path.Combine(emv.WebRootPath, "Uploads");
    
    /// <inheritdoc />
    public async Task<IEnumerable<FileModel>> GetAllFiles()
    {
        return await dbContext.Files.ToListAsync();
    }
    
    /// <inheritdoc />
    public async Task<IEnumerable<FileModel>> GetUserFilesAsync(Guid userId)
    {
        return await dbContext.Files.Where(f => f.OwnerId == userId).ToListAsync();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<(byte[], string, string)> DownloadFileAsync(Guid userId, Guid fileId)
    {
        var file = await dbContext.Files.FirstOrDefaultAsync(f => f.Id == fileId);
        if (file == null || file.OwnerId != userId || !File.Exists(file.Path))
            throw new FileNotFoundException("Файл не найден или доступ запрещен.");

        var bytes = await File.ReadAllBytesAsync(file.Path);
        return (bytes, file.ContentType, file.FileName);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public async Task<(bool, string)> ShareFileAsync(Guid userId, Guid fileId, bool isPublic, Guid? sharedWithUserId = null)
    {
        var file = await dbContext.Files.FindAsync(fileId);
        if (file == null)
            return (false, "Файл не найден.");

        if (file.OwnerId != userId)
            return (false, "Вы не владелец этого файла.");

        var sharedFile = new SharedFile()
        {
            Id = Guid.NewGuid(),
            FileId = fileId,
            SharedWithUserId = isPublic ? null : sharedWithUserId,
            Token = isPublic ? Guid.NewGuid().ToString() : null,
            SharedAt = DateTime.UtcNow
        };

        dbContext.SharedFiles.Add(sharedFile);
        await dbContext.SaveChangesAsync();

        return (true, isPublic
            ? $"Файл доступен по ссылке: /api/files/share/{sharedFile.Token}"
            : "Файл успешно предоставлен пользователю.");
    }

    /// <inheritdoc />
    public async Task<(FileModel?, string)> GetSharedFileAsync(string token)
    {
        var sharedFile = await dbContext.SharedFiles
            .Include(fs => fs.File)
            .FirstOrDefaultAsync(fs => fs.Token == token);

        if (sharedFile == null)
            return (null, "Файл не найден или закрыт доступ.");

        if (sharedFile.ExpirationDate != null && sharedFile.ExpirationDate < DateTime.UtcNow)
            return (null, "Срок действия ссылки истек.");

        return (sharedFile.File, "Файл найден.");
    }
}