using ATON_Internship.Models;

namespace ATON_Internship.Interfaces
{
    public interface IUsersRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<User> DeleteUserAsync(string login, bool isSoftDelete);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByLoginAndPasswordAsync(string login, string password);
        Task<List<User>> GetUsersOlderThanAsync(int age);
    }
}
