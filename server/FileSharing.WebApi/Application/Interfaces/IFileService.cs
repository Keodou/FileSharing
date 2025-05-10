using FileSharing.WebApi.Application.Enums;
using FileSharing.WebApi.Domain.Entities;

namespace FileSharing.WebApi.Application.Interfaces;

public interface IFileService
{
    Task<IEnumerable<FileModel>> GetAllFiles();
    Task<IEnumerable<FileModel>> GetUserFilesAsync(Guid userId);
    Task<(bool Success, string Message, object? Result)> UploadFileAsync(Guid userId, IFormFile file);
    Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadFileAsync(Guid userId, Guid fileId);
    Task<(bool Success, string Message)> DeleteFileAsync(Guid userId, UserRole userRole, Guid fileId);
    Task<(bool Success, string Message)> ShareFileAsync(Guid userId, Guid fileId, bool isPublic,
        Guid? sharedWithUserId);
    Task<(FileModel?, string)> GetSharedFileAsync(string token);
}