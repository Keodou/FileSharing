using FileSharing.WebApi.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileSharing.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IFileService fileService, ICurrentUserService currentUserService) : ControllerBase
    {
        [Authorize(Roles = "User")]
        [HttpGet("list")]
        public async Task<IActionResult> GetUserFiles()
        {
            var userId = currentUserService.UserId;
            var files = await fileService.GetUserFilesAsync(userId);
            return Ok(files);
        }

        [Authorize(Roles = "User")]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileTask(IFormFile file)
        {
            var userId = currentUserService.UserId;
            var (success, message, result) = await fileService.UploadFileAsync(userId, file);
            return success ? Ok(result) : BadRequest(message);
        }

        [Authorize(Roles = "User")]
        [HttpGet("download/{id}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            try
            {
                var userId = currentUserService.UserId;
                var (fileBytes, contentType, fileName) = await fileService.DownloadFileAsync(userId, id);
                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Roles = "User")]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var userId = currentUserService.UserId;
            var (success, message) = await fileService.DeleteFileAsync(userId, id);
            return success ? Ok(message) : StatusCode(403, message);
        }
    }
}