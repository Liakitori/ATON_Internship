using ATON_Internship.Interfaces;
using ATON_Internship.Models;

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
            var login = _httpContextAccessor.HttpContext?.
                                                   Request.
                                                   Headers["X-Curren-User"].
                                                   FirstOrDefault();
            if (string.IsNullOrEmpty( login ))
            {
                throw new UnauthorizedAccessException("Текущий пользователь не указан в заголовке X-Current-User.");
            }
            return login;
        }

        public async Task<bool> IsAdminAsync(string login)
        {
            var user = await _usersRepository.GetUserByLoginAsync(login);
            return user != null && user.Admin && user.RevokedOn == null;
        }

        private bool IsActive(User user)
        {
            return user.RevokedOn == null;
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
            if (!await IsAdminAsync(currentUserLogin))
            {
                throw new UnauthorizedAccessException("Только администраторы могут создавать пользователей.");
            }

            if (await _usersRepository.GetUserByLoginAsync(login) != null)
            {
                throw new InvalidOperationException("Пользователь с таким логином уже существует.");
            }

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
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            bool isAdmin = await IsAdminAsync(currentUserLogin);
            bool isSelf = currentUserLogin == login;

            if (!isAdmin && !(isSelf && IsActive(user)))
            {
                throw new UnauthorizedAccessException("Недостаточно прав для изменения данных пользователя.");
            }

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
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            bool isAdmin = await IsAdminAsync(currentUserLogin);
            bool isSelf = currentUserLogin == login;

            if (!isAdmin && !(isSelf && IsActive(user)))
            {
                throw new UnauthorizedAccessException("Недостаточно прав для изменения пароля.");
            }

            user.Password = newPassword;
            user.ModifiedOn = DateTime.Now;
            user.ModifiedBy = currentUserLogin;

            return await _usersRepository.UpdateUserAsync(user);
        }

        public async Task<User> UpdateUserLoginAsync(string login, string newLogin)
        {
            var currentUserLogin = GetCurrentUserLogin();
            var user = await _usersRepository.GetUserByLoginAsync(login);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            bool isAdmin = await IsAdminAsync(currentUserLogin);
            bool isSelf = currentUserLogin == login;

            if (!isAdmin && !(isSelf && IsActive(user)))
            {
                throw new UnauthorizedAccessException("Недостаточно прав для изменения логина.");
            }

            if (await _usersRepository.GetUserByLoginAsync(newLogin) != null)
            {
                throw new InvalidOperationException("Такой логин уже занят");
            }

            user.Login = newLogin;
            user.ModifiedOn = DateTime.Now;
            user.ModifiedBy = currentUserLogin;

            return await _usersRepository.UpdateUserAsync(user);
        }

        public Task<List<User>> GetAllActiveUsersAsync()
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByLoginAsync(string login)
        {
            throw new NotImplementedException();
        }

        public Task<User> GetUserByLoginAndPasswordAsync(string login, string password)
        {
            throw new NotImplementedException();
        }

        public Task<List<User>> GetUsersOlderThenAsync(int age)
        {
            throw new NotImplementedException();
        }

        public Task<User> DeleteUserAsync(string login, bool isSoftDelete)
        {
            throw new NotImplementedException();
        }

        public Task<User> RestoreUserAsync(string login)
        {
            throw new NotImplementedException();
        }
    }
}
