namespace FileSharing.Core
{
    public interface IFileService
    {
        Task UploadFileAsync(string fileName, Stream fileStream);
        Task<Stream> DownloadFileAsync(string fileName);
    }
}
