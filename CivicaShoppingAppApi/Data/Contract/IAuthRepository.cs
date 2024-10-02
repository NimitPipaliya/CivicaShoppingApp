using CivicaShoppingAppApi.Models;

namespace CivicaShoppingAppApi.Data.Contract
{
    public interface IAuthRepository
    {
        User ValidateUser(string username);
        IEnumerable<User> GetAllUsers(int page, int pageSize, string? search, string sortOrder);

        int TotalUsers(string? search);

        User? GetUser(int id);

        bool DeleteUser(int id);

        bool RegisterUser(User user);

        bool UserExists(string loginId, string email);

        IEnumerable<SecurityQuestion> GetAllSecurityQuestions();
        bool UpdateUser(User user);
    }
}
