using ATON_Internship.Interfaces;
using ATON_Internship.Models;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace ATON_Internship.Services
{
    public class UserService : IUserService
    {
        private readonly IUsersRepository _usersRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUsersRepository usersRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _usersRepository = usersRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUserLogin()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<User> CreateUserAsync(
            string login,
            string password,
            string name,
            int gender,
            DateTime birthday,
            bool isAdmin)
        {
            var currentUserLogin = GetCurrentUserLogin();
            var user = new User
            {
                Guid = Guid.NewGuid(),
                Login = login,
                Password = password,
                Name = name,
                Gender = gender,
                Birthday = birthday == DateTime.MinValue ? null : birthday,
                Admin = isAdmin,
                CreatedOn = DateTime.Now,
                CreatedBy = currentUserLogin,
                ModifiedOn = DateTime.Now,
                ModifiedBy = currentUserLogin,
            };
            return await _usersRepository.AddUserAsync(user);
        }

        public async Task<User> UpdateUserDetailsAsync(
            string login,
            string newName,
            int newGender,
            DateTime newBirthday)
        {
            var currentUserLogin = GetCurrentUserLogin();
            var user = await _usersRepository.GetUserByLoginAsync(login);
            user.Name = newName;
            user.Gender = newGender;
            user.Birthday = newBirthday == DateTime.MinValue ? null : newBirthday;
            user.ModifiedOn = DateTime.Now;
            user.ModifiedBy = currentUserLogin;
            return await _usersRepository.UpdateUserAsync(user);
        }

        public async Task<User> UpdateUserPasswordAsync(string login, string newPassword)
        {
            var currentUserLogin = GetCurrentUserLogin();
            var user = await _usersRepository.GetUserByLoginAsync(login);
            user.Password = newPassword;
            user.ModifiedOn = DateTime.Now;
            user.ModifiedBy = currentUserLogin;
            return await _usersRepository.UpdateUserAsync(user);
        }

        public async Task<User> UpdateUserLoginAsync(string login, string newLogin)
        {
            var currentUserLogin = GetCurrentUserLogin();
            var user = await _usersRepository.GetUserByLoginAsync(login);
            user.Login = newLogin;
            user.ModifiedOn = DateTime.Now;
            user.ModifiedBy = currentUserLogin;
            return await _usersRepository.UpdateUserAsync(user);
        }

        public async Task<List<User>> GetAllActiveUsersAsync()
        {
            var users = await _usersRepository.GetAllUsersAsync();
            return users.Where(u => u.RevokedOn == null).OrderBy(u => u.CreatedOn).ToList();
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await _usersRepository.GetUserByLoginAsync(login);
        }

        public async Task<User> GetUserByLoginAndPasswordAsync(string login, string password)
        {
            return await _usersRepository.GetUserByLoginAndPasswordAsync(login, password);
        }

        public async Task<List<User>> GetUsersOlderThenAsync(int age)
        {
            return await _usersRepository.GetUsersOlderThanAsync(age);
        }

        public async Task<User> DeleteUserAsync(string login, bool isSoftDelete)
        {
            var currentUserLogin = GetCurrentUserLogin();
            return await _usersRepository.DeleteUserAsync(login, isSoftDelete, currentUserLogin);
        }

        public async Task<User> RestoreUserAsync(string login)
        {
            var currentUserLogin = GetCurrentUserLogin();
            var user = await _usersRepository.GetUserByLoginAsync(login);
            user.RevokedOn = null;
            user.RevokedBy = null;
            user.ModifiedOn = DateTime.Now;
            user.ModifiedBy = currentUserLogin;
            return await _usersRepository.UpdateUserAsync(user);
        }

        public async Task<List<string>> GetAllLoginsAsync()
        {
            return await _usersRepository.GetAllLoginsAsync();
        }

    }
}
