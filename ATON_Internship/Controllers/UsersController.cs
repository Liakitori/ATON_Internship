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

        public UsersController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        // Аутентификация и выдача токена
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Создание пользователя (Доступно админам)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
        {
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Изменение имени, пола или даты рождения (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/details")]
        public async Task<IActionResult> UpdateUserDetails(string login, [FromBody] UpdateUserDetailsRequest request)
        {
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Изменение пароля (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/password")]
        public async Task<IActionResult> UpdateUserPassword(string login, [FromBody] string newPassword)
        {
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Изменение логина (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/login")]
        public async Task<IActionResult> UpdateUserLogin(string login, [FromBody] string newLogin)
        {
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
                Console.WriteLine($"[UsersController] Unauthorized: {ex.Message}");
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[UsersController] Error: {ex.Message}");
                return BadRequest(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // Получение пользователей старше возраста (Доступно админам)
        [Authorize]
        [HttpGet("older-than")]
        public async Task<IActionResult> GetUsersOlderThan([FromQuery] int age)
        {
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
