using System.Security.Claims;
using FileSharing.WebApi.Application.Interfaces;
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
        private readonly IFileService _fileService;

        public FilesController(IWebHostEnvironment emv, FileSharingDbContext dbContext, IFileService service)
        {
            _dbContext = dbContext;
            _uploadsFolder = Path.Combine(emv.WebRootPath, "Uploads");
            if (!Directory.Exists(_uploadsFolder))
                Directory.CreateDirectory(_uploadsFolder);
            _fileService = service;
        }

        private Guid GetUserId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userId, out var guid) ? guid : throw new UnauthorizedAccessException();
        }

        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetUserFiles()
        {
            var userId = GetUserId();
            var files = await _fileService.GetUserFilesAsync(userId);
            return Ok(files);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileTask(IFormFile file)
        {
            var userId = GetUserId();
            var (success, message, result) = await _fileService.UploadFileAsync(userId, file);
            return success ? Ok(result) : BadRequest(message);
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