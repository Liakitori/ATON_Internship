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
        /*
        //Create
        Task<User> CreateUserAsync(
            string login,
            string password,
            string name,
            int gender,
            DateTime birthday,
            bool isAdmin
            );

        //Update-1
        Task<User> UpdateUserDetailsAsync(
            string login,
            string newName,
            int newGender,
            DateTime newBirthday
            );
        Task<User> UpdateUserPasswordAsync(
            string login,
            string newPassword
            );
        Task<User> UpdateUserLoginAsync(
            string login,
            string newLogin
            );

        //Read
        Task<List<User>> GetAllActiveUsersAsync();
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByLoginAndPasswordAsync(
            string login,
            string password
            );
        Task<User> GetUserOlderThenAsync(int age);

        //Delete
        Task<User> DeleteUserAsync(
            string login,
            bool isSoftDelete
            );

        //Update-2
        Task<User> RestoreUserAsync(string login);
        */
    }
}
