namespace FileSharing.WebApi.Infrastructure.Storage;

/// <summary>
/// Содержит метод для инициализации хранилища файлов.
/// </summary>
/// <param name="env">директория веб-проекта.</param>
public class StorageInitializer(IWebHostEnvironment env)
{
    private readonly string _uploadsFolder = Path.Combine(env.WebRootPath, "Uploads");

    /// <summary>
    /// Создает директорию хранения файлов пользователей при запуске программы, если такая отсутствует
    /// </summary>
    public void EnsureUploadDirectoryExists()
    {
        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }
    }
}