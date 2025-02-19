using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileSharing.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private string uploadsFolder = "D:/FileStorage";

        [HttpGet("list")]
        public IActionResult GetFiles()
        {
            var files = Directory.GetFiles(uploadsFolder);
            if (files == null)
            {
                return NotFound("Нет загруженных файлов");
            }

            return Ok(files
                .Select(Path.GetFileName)
                .ToList());
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileTask(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не найден");
            }

            var filePath = Path.Combine(uploadsFolder, file.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Ok(new { Name = file.FileName, Size = file.Length });
        }

        [HttpGet("{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var filePath = Path.Combine(uploadsFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл не найден.");
            }

            return PhysicalFile(filePath, "application/octet-stream", fileName);
        }
    }
}
