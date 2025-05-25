using Microsoft.AspNetCore.Http;

namespace FileSharing.Tests.Utils;

public static class FileTestUtils
{
    /// <summary>
    /// Создает мок файла.
    /// </summary>
    /// <param name="content">текст, который содержится в моке файла.</param>
    /// <param name="fileName">имя файла.</param>
    /// <returns></returns>
    public static IFormFile CreateMockFile(string content, string fileName)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(content);
        writer.Flush();
        stream.Position = 0;

        return new FormFile(stream, 0, stream.Length, "IdFromForm", fileName)
        {
            Headers = new HeaderDictionary(),
            ContentType = "text/plain"
        };
    }
}