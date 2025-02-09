using System.Net;

namespace FileSharing.FileClient
{
    class FileClient
    {
        private static string serverUrl = "http://localhost:5000/";

        static async Task Main(string[] args)
        {
            Console.WriteLine("Выберите действие: 1 - Загрузить файл, 2 - Скачать файл");
            string choice = Console.ReadLine();

            if (choice == "1")
            {
                Console.WriteLine("Введите путь к файлу:");
                string filePath = Console.ReadLine();
                await UploadFile(filePath);
            }

            else if (choice == "2")
            {
                Console.WriteLine("Введите имя файла для скачивания:");
                string fileName = Console.ReadLine();
                await DownloadFile(fileName);
            }

            else
                Console.WriteLine("Такого варианта нет.");

        }

        private static async Task DownloadFile(string fileName)
        {
            using (var client = new HttpClient())
            {
                var downloadUrl = $"{serverUrl}?file={fileName}";
                HttpResponseMessage response = await client.GetAsync(downloadUrl);

                if (response.IsSuccessStatusCode)
                {
                    var savePath = Path.Combine("D:/Downloads", fileName);
                    using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        await response.Content.CopyToAsync(fs);
                    }

                    Console.WriteLine($"Файл {fileName} успешно скачан.");
                }

                else
                {
                    Console.WriteLine("Ошибка при скачивании файла.");
                }
            }
        }

        private static async Task UploadFile(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Файл не найден");
            }

            using var client = new HttpClient();
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var fileName = Path.GetFileName(filePath);
                HttpContent fileContent = new StreamContent(fs);
                fileContent.Headers.Add("FileName", fileName);

                HttpResponseMessage response = await client.PostAsync(serverUrl, fileContent);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Файл успешно загружен");
                }
                else
                {
                    Console.WriteLine("Ошибка при загрузке файла");
                }
            }
        }
    }
}