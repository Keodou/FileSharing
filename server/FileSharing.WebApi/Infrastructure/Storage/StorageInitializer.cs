namespace FileSharing.WebApi.Infrastructure.Storage;

public class StorageInitializer(IWebHostEnvironment env)
{
    private readonly string _uploadsFolder = Path.Combine(env.WebRootPath, "Uploads");

    public void EnsureUploadDirectoryExists()
    {
        if (!Directory.Exists(_uploadsFolder))
        {
            Directory.CreateDirectory(_uploadsFolder);
        }
    }
}