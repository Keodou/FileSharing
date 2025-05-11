using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileSharing.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IFileService fileService, ICurrentUserService currentUserService) : ControllerBase
    {
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            var files = await fileService.GetAllFiles();
            return Ok(files);
        }
        
        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetUserFiles()
        {
            var currentUser = currentUserService.GetCurrentUser();
            if (currentUser == null)
                return Unauthorized("Пользователь не авторизован.");
            
            var files = await fileService.GetUserFilesAsync(currentUser.Id);
            return Ok(files);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileTask(IFormFile file)
        {
            var currentUser = currentUserService.GetCurrentUser();
            if (currentUser == null)
                return Unauthorized("Пользователь не авторизован.");
            
            var (success, message, result) = await fileService.UploadFileAsync(currentUser.Id, file);
            return success ? Ok(result) : BadRequest(message);
        }

        [Authorize]
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            try
            {
                var currentUser = currentUserService.GetCurrentUser();
                if (currentUser == null)
                    return Unauthorized("Пользователь не авторизован.");
                
                var (fileBytes, contentType, fileName) = await fileService.DownloadFileAsync(currentUser.Id, id);
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var currentUser = currentUserService.GetCurrentUser();
            if (currentUser == null)
                return Unauthorized("Пользователь не авторизован.");
            
            var (success, message) = await fileService.DeleteFileAsync(currentUser.Id, currentUser.Role, id);
            return success ? Ok(message) : StatusCode(403, message);
        }

        [Authorize]
        [HttpPost("share/{fileId}")]
        public async Task<IActionResult> ShareFile(Guid fileId, [FromBody] SharedFileDto sharedFileDto)
        {
            var currentUser = currentUserService.GetCurrentUser();
            if (currentUser == null)
                return Unauthorized("Пользователь не авторизован.");
            
            var (success, message) = await fileService.ShareFileAsync(currentUser.Id, fileId, sharedFileDto.IsPublic,
                sharedFileDto.SharedWithUserId);
            return success ? Ok(message) : BadRequest(message);
        }
        
        [HttpGet("share/{token}")]
        public async Task<IActionResult> GetSharedFile(string token)
        {
            var (file, message) = await fileService.GetSharedFileAsync(token);
            if (file == null)
                return NotFound(message);

            return PhysicalFile(file.Path, file.ContentType, file.FileName);
        }
    }
}