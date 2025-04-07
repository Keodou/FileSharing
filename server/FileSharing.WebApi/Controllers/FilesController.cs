using System.Security.Claims;
using FileSharing.WebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileSharing.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly string _uploadsFolder;
        private readonly FileSharingDbContext _dbContext;

        public FilesController(IWebHostEnvironment emv, FileSharingDbContext dbContext)
        {
            _dbContext = dbContext;
            _uploadsFolder = Path.Combine(emv.WebRootPath, "Uploads");
            if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);
        }

        [Authorize]
        [HttpGet("list")]
        public IActionResult GetFiles()
        {
            var files = Directory.GetFiles(_uploadsFolder);
            if (files == null)
            {
                return NotFound("Нет загруженных файлов");
            }

            return Ok(files
                .Select(filePath => new
                {
                    Name = Path.GetFileName(filePath),
                    Created = new FileInfo(filePath).CreationTime,
                    Size = new FileInfo(filePath).Length
                })
                .ToList());
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileTask(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не найден");
            }
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var ownerId))
            {
                return Unauthorized("Пользователь не авторизован или идентификатор недействителен");
            }
            
            var filePath = Path.Combine(_uploadsFolder, Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
            await file.CopyToAsync(new FileStream(filePath, FileMode.Create));

            var fileModel = new FileModel
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                UploadDate = DateTime.UtcNow,
                OwnerId = ownerId,
                Path = filePath
            };

            _dbContext.Files.Add(fileModel);
            await _dbContext.SaveChangesAsync();

            return Ok(new { Name = file.FileName, Size = file.Length });
        }

        [Authorize]
        [HttpGet("{fileName}")]
        public IActionResult DownloadFile(string fileName)
        {
            var filePath = Path.Combine(_uploadsFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл не найден.");
            }

            return PhysicalFile(filePath, "application/octet-stream", fileName);
        }

        [Authorize]
        [HttpDelete("{fileName}")]
        public IActionResult DeleteFile(string fileName)
        {
            var filePath = Path.Combine(_uploadsFolder, fileName);
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл не найден.");
            }

            System.IO.File.Delete(filePath);
            return Ok($"{fileName} был успешно удален.");
        }
    }
}
