using ATON_Internship.Interfaces;
using ATON_Internship.Models;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;

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
                    Gender = 2,
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

        public async Task<User> AddUserAsync(User user)
        {
            _users.Add(user);
            return await Task.FromResult(user);
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            var index = _users.FindIndex(u => u.Guid == user.Guid);
            _users[index] = user;
            return await Task.FromResult(user);
        }

        public async Task<User> DeleteUserAsync(string login, bool isSoftDelete, string revokedBy)
        {
            var user = _users.FirstOrDefault(u => u.Login.Equals(login, StringComparison.OrdinalIgnoreCase));
            if (isSoftDelete)
            {
                user.RevokedOn = DateTime.Now;
                user.RevokedBy = revokedBy;
            }
            else
            {
                _users.Remove(user);
            }
            return await Task.FromResult(user);
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await Task.FromResult(_users.ToList());
        }

        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Login == login));
        }

        public async Task<User> GetUserByLoginAndPasswordAsync(string login, string password)
        {
            return await Task.FromResult(_users.FirstOrDefault(u => u.Login == login && u.Password == password));
        }

        public async Task<List<User>> GetUsersOlderThanAsync(int age)
        {
            var thresholdDate = DateTime.Now.AddYears(-age);
            var users = _users.Where(u => u.Birthday.HasValue && u.Birthday.Value <= thresholdDate).ToList();
            return await Task.FromResult(users);
        }

        public async Task<List<string>> GetAllLoginsAsync()
        {
            var logins = _users.Select(u => u.Login).ToList();
            return await Task.FromResult(logins);
        }

    }
}
