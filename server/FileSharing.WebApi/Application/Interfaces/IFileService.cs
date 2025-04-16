using FileSharing.WebApi.Entities;

namespace FileSharing.WebApi.Application.Interfaces;

public interface IFileService
{
    Task<IEnumerable<FileModel>> GetUserFilesAsync(Guid userId);
    Task<(bool Success, string Message, object? Result)> UploadFileAsync(Guid userId, IFormFile file);
    Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadFileAsync(Guid userId, Guid fileId);
    Task<(bool Success, string Message)> DeleteFileAsync(Guid userId, Guid fileId);
}