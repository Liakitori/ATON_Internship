using ATON_Internship.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static ATON_Internship.Models.DTOs;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ATON_Internship.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(
            IUserService userService,
            ITokenService tokenService,
            ILogger<UsersController> logger
            )
        {
            _userService = userService;
            _tokenService = tokenService;
            _logger = logger;
        }

        // Аутентификация и выдача токена
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.GetUserByLoginAndPasswordAsync(request.Login, request.Password);
                if (user == null)
                {
                    return Unauthorized("Неверный логин или пароль.");
                }

                var token = _tokenService.GenerateToken(user);
                return Ok(new { Token = token });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при логине: {Message}", ex.Message);
                return Unauthorized (ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при логине: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Создание пользователя (Доступно админам)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.CreateUserAsync(
                    request.Login,
                    request.Password,
                    request.Name,
                    request.Gender,
                    request.Birthday ?? DateTime.MinValue,
                    request.IsAdmin
                );
                return Ok(new UserDto
                {
                    Login = user.Login,
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    IsActive = user.RevokedOn == null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при создании пользователя: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Конфликт при создании пользователя: {Message}", ex.Message);
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при создании пользователя: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Изменение имени, пола или даты рождения (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/details")]
        public async Task<IActionResult> UpdateUserDetails(string login, [FromBody] UpdateUserDetailsRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var user = await _userService.UpdateUserDetailsAsync(
                    login,
                    request.NewName,
                    request.NewGender,
                    request.NewBirthday ?? DateTime.MinValue
                );
                return Ok(new UserDto
                {
                    Login = user.Login,
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    IsActive = user.RevokedOn == null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при обновлении данных пользователя: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Пользователь не найден: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении данных пользователя: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Изменение пароля (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/password")]
        public async Task<IActionResult> UpdateUserPassword(string login, [FromBody] string newPassword)
        {
            if (string.IsNullOrEmpty(newPassword))
            {
                return BadRequest("Новый пароль не может быть пустым");
            }

            try
            {
                var user = await _userService.UpdateUserPasswordAsync(login, newPassword);
                return Ok(new UserDto
                {
                    Login = user.Login,
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    IsActive = user.RevokedOn == null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при обновлении пароля: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Пользователь не найден: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении пароля: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Изменение логина (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/login")]
        public async Task<IActionResult> UpdateUserLogin(string login, [FromBody] string newLogin)
        {
            if (string.IsNullOrEmpty(newLogin))
            {
                return BadRequest("Новый логин не может быть пустым.");
            }

            try
            {
                var user = await _userService.UpdateUserLoginAsync(login, newLogin);
                return Ok(new UserDto
                {
                    Login = user.Login,
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    IsActive = user.RevokedOn == null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при обновлении логина: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Пользователь не найден: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning("Конфликт при обновлении логина: {Message}", ex.Message);
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении логина: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Список активных пользователей (Доступно админам)
        [Authorize]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].ToString();
                var users = await _userService.GetAllActiveUsersAsync();
                var userDtos = users.Select(u => new UserDto
                {
                    Login = u.Login,
                    Name = u.Name,
                    Gender = u.Gender,
                    Birthday = u.Birthday,
                    IsActive = u.RevokedOn == null
                }).ToList();
                return Ok(userDtos);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при получении активных пользователей: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении активных пользователей: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Получение пользователя по логину (Доступно админам)
        [Authorize]
        [HttpGet("{login}")]
        public async Task<IActionResult> GetUserByLogin(string login)
        {
            try
            {
                var user = await _userService.GetUserByLoginAsync(login);
                return Ok(new UserDetailsDto
                {
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    IsActive = user.RevokedOn == null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при получении пользователя: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Пользователь не найден: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователя: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Получение пользователей старше возраста (Доступно админам)
        [Authorize]
        [HttpGet("older-than")]
        public async Task<IActionResult> GetUsersOlderThan([FromQuery] int age)
        {
            if (age < 0)
            {
                return BadRequest("Возраст не может быть отрицательным.");
            }

            try
            {
                var users = await _userService.GetUsersOlderThenAsync(age);
                var userDtos = users.Select(u => new UserDto
                {
                    Login = u.Login,
                    Name = u.Name,
                    Gender = u.Gender,
                    Birthday = u.Birthday,
                    IsActive = u.RevokedOn == null
                }).ToList();
                return Ok(userDtos);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при получении пользователей старше возраста: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при получении пользователей старше возраста: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Удаление пользователя (Доступно админам)
        [Authorize]
        [HttpDelete("{login}")]
        public async Task<IActionResult> DeleteUser(string login, [FromQuery] bool isSoftDelete = true)
        {
            try
            {
                var user = await _userService.DeleteUserAsync(login, isSoftDelete);
                return Ok(new UserDto
                {
                    Login = user.Login,
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    IsActive = user.RevokedOn == null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при удалении пользователя: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Пользователь не найден: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при удалении пользователя: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

        // Восстановление пользователя (Доступно админам)
        [Authorize]
        [HttpPut("{login}/restore")]
        public async Task<IActionResult> RestoreUser(string login)
        {
            try
            {
                var user = await _userService.RestoreUserAsync(login);
                return Ok(new UserDto
                {
                    Login = user.Login,
                    Name = user.Name,
                    Gender = user.Gender,
                    Birthday = user.Birthday,
                    IsActive = user.RevokedOn == null
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Неавторизованный доступ при восстановлении пользователя: {Message}", ex.Message);
                return Unauthorized(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning("Пользователь не найден: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при восстановлении пользователя: {Message}", ex.Message);
                return StatusCode(500, "Внутренняя ошибка сервера.");
            }
        }

    }
}
