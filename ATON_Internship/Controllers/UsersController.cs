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

        public UsersController(
            IUserService userService,
            ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        // Аутентификация и выдача токена
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userService.GetUserByLoginAndPasswordAsync(request.Login, request.Password);
            var token = _tokenService.GenerateToken(user);
            return Ok(new { Token = token });
        }
        

        // Создание пользователя (Доступно админам)
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
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

        // Изменение имени, пола или даты рождения (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/details")]
        public async Task<IActionResult> UpdateUserDetails(string login, [FromBody] UpdateUserDetailsRequest request)
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

        // Изменение пароля (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/password")]
        public async Task<IActionResult> UpdateUserPassword(string login, [FromBody] string newPassword)
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

        // Изменение логина (Админ или сам пользователь, если активен)
        [Authorize]
        [HttpPut("{login}/login")]
        public async Task<IActionResult> UpdateUserLogin(string login, [FromBody] string newLogin)
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

        // Список активных пользователей (Доступно админам)
        [Authorize]
        [HttpGet("active")]
        public async Task<IActionResult> GetAllActiveUsers()
        {
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

        // Получение пользователя по логину (Доступно админам)
        [Authorize]
        [HttpGet("{login}")]
        public async Task<IActionResult> GetUserByLogin(string login)
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

        // Получение пользователей старше возраста (Доступно админам)
        [Authorize]
        [HttpGet("older-than")]
        public async Task<IActionResult> GetUsersOlderThan([FromQuery] int age)
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

        // Удаление пользователя (Доступно админам)
        [Authorize]
        [HttpDelete("{login}")]
        public async Task<IActionResult> DeleteUser(string login, [FromQuery] bool isSoftDelete = true)
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

        // Восстановление пользователя (Доступно админам)
        [Authorize]
        [HttpPut("{login}/restore")]
        public async Task<IActionResult> RestoreUser(string login)
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

        [Authorize]
        [HttpGet("logins")]
        public async Task<IActionResult> GetAllLogins()
        {
            var logins = await _userService.GetAllLoginsAsync();
            return Ok(logins);
        }

    }
}
