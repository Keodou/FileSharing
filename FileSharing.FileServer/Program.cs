using System.Diagnostics;
using System.Net;

// vsdfgsgf
namespace FileSharing.FileServer
{
    class FileServer
    {
        private static string storagePath = "FileStorage";

        static void Main(string[] args)
        {
            // Create directory for files, if it not created
            if (!Directory.Exists(storagePath))
            {
                Directory.CreateDirectory(storagePath);
            }

            // Initializing HTTP-server
            var listener = new HttpListener();
            listener.Prefixes.Add("http://localhost:5000/");
            listener.Start();
            Console.WriteLine("Сервер запущен. Ожидание запросов...");

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                ProcessRequest(context);
            }
        }

        private static void ProcessRequest(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            if (request.HttpMethod == "POST")
            {
                string fileName = request.Headers["FileName"];
                string filePath = Path.Combine(storagePath, fileName);

                using (var fs = new FileStream(filePath, FileMode.Create))
                {
                    request.InputStream.CopyTo(fs);
                }

                response.StatusCode = (int)HttpStatusCode.OK;
                Console.WriteLine($"Файл {fileName} загружен");
            }

            else if (request.HttpMethod == "GET")
            {
                string fileName = request.QueryString["file"];
                string filePath = Path.Combine(storagePath, fileName);

                if (File.Exists(filePath))
                {
                    response.ContentType = "application/octet-stream";
                    response.AddHeader("Content-Disposition", $"attachment; filename={fileName}");

                    using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                    {
                        fs.CopyTo(response.OutputStream);
                    }

                    response.StatusCode = (int)HttpStatusCode.OK;
                    Console.WriteLine($"Файл {fileName} отправлен");
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.NotFound;
                    Console.WriteLine($"Файл {fileName} не найден");
                }

                response.Close();
            }
        }
    }
}