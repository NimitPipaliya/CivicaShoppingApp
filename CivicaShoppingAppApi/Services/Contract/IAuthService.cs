using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;

namespace CivicaShoppingAppApi.Services.Contract
{
    public interface IAuthService
    {
        ServiceResponse<string> LoginUserService(LoginDto login);
        ServiceResponse<IEnumerable<UserDto>> GetAllUsers(int page, int pageSize, string? search, string sortOrder);

        ServiceResponse<int> TotalUsers(string? search);

        ServiceResponse<UserDto> GetUser(int userId);

        ServiceResponse<string> RemoveUser(int id);

        ServiceResponse<string> RegisterUserService(RegisterUserDto register);

        ServiceResponse<IEnumerable<SecurityQuestion>> GetAllQuestions();
        ServiceResponse<string> ChangePassword(ChangePasswordDto changePasswordDto);
        ServiceResponse<string> ForgetPassword(ForgetPasswordDto forgetPasswordDto);
    }
}
