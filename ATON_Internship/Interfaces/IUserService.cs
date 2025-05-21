using ATON_Internship.Models;

namespace ATON_Internship.Interfaces
{
    public interface IUserService
    {
        //Create
        Task<User> CreateUserAsync(
            string login,
            string password,
            string name,
            int gender,
            DateTime birthday,
            bool isAdmin);

        //Update-1
        Task<User> UpdateUserDetailsAsync(
            string login,
            string newName,
            int newGender,
            DateTime newBirthday);

        Task<User> UpdateUserPasswordAsync(
            string login,
            string newPassword);

        Task<User> UpdateUserLoginAsync(
            string login,
            string newLogin);

        //Read
        Task<List<User>> GetAllActiveUsersAsync();
        Task<User> GetUserByLoginAsync(string login);
        Task<User> GetUserByLoginAndPasswordAsync(
            string login,
            string password);

        Task<List<User>> GetUsersOlderThenAsync(int age);

        //Delete
        Task<User> DeleteUserAsync(
            string login,
            bool isSoftDelete);

        //Update-2
        Task<User> RestoreUserAsync(string login);

        Task<List<string>> GetAllLoginsAsync();
    }
}
