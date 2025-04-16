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
    public class FilesController(IFileService fileService, ICurrentUserService currentUserService) : ControllerBase
    {
        [Authorize]
        [HttpGet("list")]
        public async Task<IActionResult> GetUserFiles()
        {
            var userId = currentUserService.UserId;
            var files = await fileService.GetUserFilesAsync(userId);
            return Ok(files);
        }

        [Authorize]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFileTask(IFormFile file)
        {
            var userId = currentUserService.UserId;
            var (success, message, result) = await fileService.UploadFileAsync(userId, file);
            return success ? Ok(result) : BadRequest(message);
        }

        [Authorize]
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

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            var userId = currentUserService.UserId;
            var (success, message) = await fileService.DeleteFileAsync(userId, id);
            return success ? Ok(message) : StatusCode(403, message);
        }
    }
}