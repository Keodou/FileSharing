using FileSharing.Core;

namespace FileSharing.Infrastructure
{
    public class FileService : IFileService
    {
        private readonly string _storagePath;

        public FileService(string storagePath)
        {
            _storagePath = storagePath;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task UploadFileAsync(string fileName, Stream fileStream)
        {
            string filePath = Path.Combine(_storagePath, fileName);
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.CopyToAsync(fs);
            }
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            string filePath = Path.Combine(_storagePath, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл не найден.");
            }

            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }
    }
}
