using ATON_Internship.Models;

namespace ATON_Internship.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
