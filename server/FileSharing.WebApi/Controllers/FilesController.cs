using System.Security.Claims;
using FileSharing.WebApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<IActionResult> GetUserFiles()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var ownerId))
            {
                return Unauthorized("Пользователь не авторизован или идентификатор недействителен");
            }

            var userFiles = await _dbContext.Files
                .Where(f => f.OwnerId == ownerId)
                .ToListAsync();

            return Ok(userFiles);
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
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == id);
            if (file == null)
            {
                return NotFound("Файл не найден.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (file.OwnerId.ToString() != userId)
            {
                return StatusCode(403, "У вас нет прав на скачивание этого файла.");
            }
            
            var filePath = file.Path;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Файл был удален с сервера");
            }
            
            return PhysicalFile(filePath, file.ContentType, file.FileName);
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == id);
            
            if (file == null)
            {
                return NotFound("Файл не найден.");
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (file.OwnerId.ToString() != userId)
            {
                return StatusCode(403, "У вас нет прав на удаление этого файла.");
            }
            
            var filePath = file.Path;
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Не существует файла по данной директории");
            }

            System.IO.File.Delete(filePath);
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync();
            return Ok($"{file.FileName} был успешно удален.");
        }
    }
}