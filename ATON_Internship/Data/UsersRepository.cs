using ATON_Internship.Interfaces;
using ATON_Internship.Models;

namespace ATON_Internship.Data
{
    public class UsersRepository : IUsersRepository
    {
        private readonly List<User> _users;
        public UsersRepository()
        {
            _users = new List<User>
            {
                new User
                {
                    Guid = Guid.NewGuid(),
                    Login = "Admin",
                    Password = "Admin123",
                    Name = "Administrator",
                    Gender = 2, // Неизвестно
                    Birthday = null,
                    Admin = true,
                    CreatedOn = DateTime.Now,
                    CreatedBy = "System",
                    ModifiedOn = DateTime.Now,
                    ModifiedBy = "System",
                    RevokedOn = null,
                    RevokedBy = null
                }
            };
        }

        public async Task<User> AddUserAsync (User user)
        {
            _users.Add(user);
            return user;
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var existingUser = _users.FirstOrDefault(u => u.Guid == user.Guid);
            if (existingUser == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            existingUser.Login = user.Login;
            existingUser.Password = user.Password;
            existingUser.Name = user.Name;
            existingUser.Gender = user.Gender;
            existingUser.Birthday = user.Birthday;
            existingUser.Admin = user.Admin;
            existingUser.CreatedOn = user.CreatedOn;
            existingUser.CreatedBy = user.CreatedBy;
            existingUser.ModifiedOn = user.ModifiedOn;
            existingUser.ModifiedBy = user.ModifiedBy;
            existingUser.RevokedOn = user.RevokedOn;
            existingUser.RevokedBy = user.RevokedBy;

            return existingUser;
        }

        public async Task<User> DeleteUserAsync(string login, bool isSoftDelete)
        {
            var user = _users.FirstOrDefault(u => u.Login == login);
            if (user == null)
            {
                throw new KeyNotFoundException("Пользователь не найден.");
            }

            if (isSoftDelete)
            {
                user.RevokedOn = DateTime.Now;
                user.RevokedBy = "Admin";
                return user;
            }
            else
            {
                _users.Remove(user);
                return user;
            }
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return _users.ToList();
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return _users.FirstOrDefault(u => u.Login == login);
        }

        public async Task<User> GetUserByLoginAndPasswordAsync(string login, string password)
        {
            return _users.FirstOrDefault(u => u.Login == login && u.Password == password);
        }

        public async Task<List<User>> GetUsersOlderThanAsync(int age)
        {
            var thresholdDate = DateTime.Now.AddYears(-age);
            return _users.Where(u => u.Birthday.HasValue && u.Birthday.Value <= thresholdDate).ToList();
        }
    }
}
