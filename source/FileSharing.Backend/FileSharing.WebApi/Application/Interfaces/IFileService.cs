using FileSharing.WebApi.Application.Enums;
using FileSharing.WebApi.Domain.Entities;

namespace FileSharing.WebApi.Application.Interfaces;

/// <summary>
/// Определяет методы сервиса работы с файлами.
/// </summary>
public interface IFileService
{
    Task<IEnumerable<FileModel>> GetAllFiles();
    
    /// <summary>
    /// Выводит список файлов пользователя.
    /// </summary>
    /// <param name="userId">идентификатор пользователя для вывода личных файлов.</param>
    /// <returns>список файлов по идентифкатору пользователя.</returns>
    Task<IEnumerable<FileModel>> GetUserFilesAsync(Guid userId);
    
    /// <summary>
    /// Реализует логику загрузки файла в директорию, делает запись в базу данных.
    /// </summary>
    /// <param name="userId">идентификатор владельца файла.</param>
    /// <param name="file">загружаемый файл.</param>
    /// <returns>Сообщение о загрузке, а также название и длину файла.</returns>
    Task<(bool Success, string Message, object? Result)> UploadFileAsync(Guid userId, IFormFile file);
    
    /// <summary>
    /// Реализует логику скачивания файла с сервера.
    /// </summary>
    /// <param name="userId">идентификатор пользователя для проверки, является ли тот владельцем.</param>
    /// <param name="fileId">идентификатор файла для скачивания.</param>
    /// <returns>Файл для скачивания.</returns>
    Task<(byte[] FileBytes, string ContentType, string FileName)> DownloadFileAsync(Guid userId, Guid fileId);
    
    /// <summary>
    /// Реализует логику удаления файлов из директории и соответствующей записи из БД.
    /// </summary>
    /// <param name="userId">идентификатор пользователя для проверки, является ли тот владельцем.</param>
    /// <param name="userRole">роль пользователя для проверки доступа.</param>
    /// <param name="fileId">идентификатор файла для удаления.</param>
    /// <returns></returns>
    Task<(bool Success, string Message)> DeleteFileAsync(Guid userId, UserRole userRole, Guid fileId);
    
    /// <summary>
    /// Реализует логику создания обычного файла из публичного.
    /// </summary>
    /// <param name="userId">идентификатор пользователя для проверки, является ли тот владельцем.</param>
    /// <param name="fileId">идентификатор файла.</param>
    /// <param name="isPublic">определяет, является ли файл публчиным.</param>
    /// <param name="sharedWithUserId">идентификатор пользователя, которому открыт доступ к файлу.</param>
    /// <returns></returns>
    Task<(bool Success, string Message)> ShareFileAsync(Guid userId, Guid fileId, bool isPublic,
        Guid? sharedWithUserId);
    
    /// <summary>
    /// Реализует логику скачивания публичного файла.
    /// </summary>
    /// <param name="token">уникальный токен для публичного файла, по которому осуществляется скачивание.</param>
    /// <returns>Файл, если такой существует, или null и сообщение об отсутствии такого файла.</returns>
    Task<(FileModel?, string)> GetSharedFileAsync(string token);
}