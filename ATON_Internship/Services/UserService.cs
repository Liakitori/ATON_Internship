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

        public async Task<List<User>> GetAllActiveUsersAsync()
        {
            var currentUserLogin = GetCurrentUserLogin();
            if (!await IsAdminAsync(currentUserLogin))
            {
                throw new UnauthorizedAccessException("Только администратор может посмотреть список активных пользователей.");
            }

            var users = await _usersRepository.GetAllUsersAsync();
            return users.Where(u => IsActive(u)).OrderBy(u => u.CreatedOn).ToList();
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            var currentUserLogin = GetCurrentUserLogin();
            if (!await IsAdminAsync(currentUserLogin))
            {
                throw new UnauthorizedAccessException("Только администратор может просматривать данные пользователя");
            }

            var user = await _usersRepository.GetUserByLoginAsync(login);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            return user;
        }

        public async Task<User> GetUserByLoginAndPasswordAsync(string login, string password)
        {
            var currentUserLogin = GetCurrentUserLogin();
            if (currentUserLogin != login)
            {
                throw new UnauthorizedAccessException("Только сам пользователь может просмотреть эти данные");
            }

            var user = await _usersRepository.GetUserByLoginAndPasswordAsync(login, password);
            if (user == null || !IsActive(user))
            {
                throw new UnauthorizedAccessException("Неверный логин или пароль, либо пользователь неактивен.");
            }

            return user;
        }

        public async Task<List<User>> GetUsersOlderThenAsync(int age)
        {
            var currentUserLogin = GetCurrentUserLogin();
            if (!await IsAdminAsync(currentUserLogin))
            {
                throw new UnauthorizedAccessException("Только администратор может просмотреть данную информацию.");
            }

            var users = await _usersRepository.GetUsersOlderThanAsync(age);
            if (users == null)
            {
                throw new KeyNotFoundException("Пользователи старше указанного возраста не найдены.");
            }

            return users;
        }

        public async Task<User> DeleteUserAsync(string login, bool isSoftDelete)
        {
            var currentUserLogin = GetCurrentUserLogin();
            if (!await IsAdminAsync(currentUserLogin))
            {
                throw new UnauthorizedAccessException("Только администраторы могут удалять пользователей.");
            }

            var user = await _usersRepository.DeleteUserAsync(login, isSoftDelete);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            if (isSoftDelete)
            {
                user.RevokedBy = currentUserLogin;
                await _usersRepository.UpdateUserAsync(user);
            }

            return user;
        }

        public async Task<User> RestoreUserAsync(string login)
        {
            var currentUserLogin = GetCurrentUserLogin();
            if(!await IsAdminAsync(currentUserLogin))
            {
                throw new UnauthorizedAccessException("Только администраторы могут восстанавливать пользователей.");
            }

            var user = await _usersRepository.GetUserByLoginAsync(login);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            user.RevokedOn = null;
            user.RevokedBy = null;
            user.ModifiedOn = DateTime.Now; ;
            user.ModifiedBy = currentUserLogin;

            return await _usersRepository.UpdateUserAsync(user);
        }
    }
}
