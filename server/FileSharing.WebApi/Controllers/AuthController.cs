using FileSharing.WebApi.Application.Interfaces;
using FileSharing.WebApi.Domain.Entities;
using FileSharing.WebApi.DTO;
using Microsoft.AspNetCore.Mvc;

namespace FileSharing.WebApi.Controllers
{
    /// <summary>
    /// Контроллер для аутентификации пользователей.
    /// </summary>
    /// <param name="authService">Сервис логики аутентификации пользователя.</param>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        /// <summary>
        /// Регистрация пользователя.
        /// </summary>
        /// <param name="request">Логин и пароль пользователя.</param>
        /// <returns>Нового зарегистрированного пользваотеля</returns>
        [HttpPost("register")]
        public async Task<ActionResult<User>> Register(UserDto request)
        {
            var user = await authService.RegisterAsync(request);
            if (user is null)
                return BadRequest("Такой пользователь уже существует.");
            
            return Ok(user);
        }

        /// <summary>
        /// Вход в систему.
        /// </summary>
        /// <param name="request">Логин и пароль пользователя.</param>
        /// <returns>JWT токен для аутентификации.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            var token = await authService.LoginAsync(request);
            if (token is null)
                return BadRequest("Неправильный логин или пароль.");

            return Ok(token);
        }
    }
}
