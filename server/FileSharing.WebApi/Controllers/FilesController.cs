using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileSharing.WebApi.Controllers
{
    /// <summary>
    /// Контроллер для работы с файлами.
    /// </summary>
    /// <param name="fileService">Сервис, в котором осуществлена логика работы с файлами.</param>
    /// <param name="currentUserService">Сервис для получения данных об авторизованном пользователе.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController(IFileService fileService, ICurrentUserService currentUserService) : ControllerBase
    {
        /// <summary>
        /// Получение всех файлов директории (для администратора).
        /// </summary>
        /// <returns>Список файлов.</returns>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            var files = await fileService.GetAllFiles();
            return Ok(files);
        }
        
        /// <summary>
        /// Получение всех файлов текущего пользователя.
        /// </summary>
        /// <returns>Список файлов текущего пользователя.</returns>
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

        /// <summary>
        /// Загрузка файла.
        /// </summary>
        /// <param name="file">Загружаемый файл.</param>
        /// <returns>Результат в виде имени и размера файла, либо сообщение об ошибке.</returns>
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

        /// <summary>
        /// Скачивание файла.
        /// </summary>
        /// <param name="id">Идентификатор файла, который необходимо скачать.</param>
        /// <returns>Файл, либо сообщение об ошибке.</returns>
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

        /// <summary>
        /// Удаление файла.
        /// </summary>
        /// <param name="id">Идентификатор удаляемого файла.</param>
        /// <returns>Сообщение о выполнении операции.</returns>
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

        /// <summary>
        /// Сделать файл публичным.
        /// </summary>
        /// <param name="fileId">Идентификатор публикуемого файла.</param>
        /// <param name="sharedFileDto">Данные о файле (кому предоставлен доступ, является ли файл публичным?).</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Скачивание публичного файла.
        /// </summary>
        /// <param name="token">Уникальный токен публичного файла, необходимо вставить в конец ссылки для скачивания.</param>
        /// <returns>Файл для скачивания.</returns>
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