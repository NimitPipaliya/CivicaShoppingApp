using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Contract;
using System.Diagnostics.Metrics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Win32;
using System.Diagnostics.CodeAnalysis;

namespace CivicaShoppingAppApi.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IPasswordService _passwordService;

        public AuthService(IAuthRepository authRepository, IPasswordService passwordService)
        {
            _authRepository = authRepository;
            _passwordService = passwordService;
        }

        public ServiceResponse<string> LoginUserService(LoginDto login)
        {
            var response = new ServiceResponse<string>();
            if (login != null)
            {
                var user = _authRepository.ValidateUser(login.Username);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Invalid user login id or password";
                    return response;
                }
                else if (!_passwordService.VerifyPasswordHash(login.Password, user.PasswordHash, user.PasswordSalt))
                {
                    response.Success = false;
                    response.Message = "Invalid user login id or password";
                    return response;
                }

                string token = _passwordService.CreateToken(user);
                response.Success = true;
                response.Data = token;
                response.Message = "Success";
                return response;
            }

            response.Success = false;
            response.Message = "Something went wrong, please try after some time";
            return response;
        }

    public ServiceResponse<IEnumerable<UserDto>> GetAllUsers(int page, int pageSize, string? search, string sortOrder)
    {
        var response = new ServiceResponse<IEnumerable<UserDto>>();
        var users = _authRepository.GetAllUsers(page, pageSize, search, sortOrder);

        if (users != null && users.Any())
        {
            List<UserDto> userDtos = new List<UserDto>();
            foreach (var contact in users.ToList())
            {
                userDtos.Add(new UserDto()
                {
                    UserId = contact.UserId,
                    Name = contact.Name,
                    Salutation = contact.Salutation,
                    Phone = contact.Phone,
                    BirthDate = contact.BirthDate,
                    Email = contact.Email,
                    Gender = contact.Gender,
                    LoginId = contact.LoginId,
                    Age = contact.Age,


                });
            }
            response.Data = userDtos;
            response.Success = true;
            response.Message = "Registered successfully!";
        }
        else
        {
            response.Success = false;
            response.Message = "No record found";
        }

        return response;
    }

    public ServiceResponse<int> TotalUsers(string? search)
    {
        var response = new ServiceResponse<int>();
        int totalUsers = _authRepository.TotalUsers(search);

        response.Data = totalUsers;
        return response;
    }

    public ServiceResponse<UserDto> GetUser(int userId)
    {
        var response = new ServiceResponse<UserDto>();
        var existingUser = _authRepository.GetUser(userId);
        if (existingUser != null)
        {
            var user = new UserDto()
            {
                UserId = existingUser.UserId,
                Salutation = existingUser.Salutation,
                BirthDate = existingUser.BirthDate,
                Age = existingUser.Age,
                Name = existingUser.Name,
                LoginId = existingUser.LoginId,
                Phone = existingUser.Phone,
                Email = existingUser.Email,
                Gender = existingUser.Gender


            };
            response.Data = user;
        }

        else
        {
            response.Success = false;
            response.Message = "Something went wrong,try after sometime";
        }
            
        return response;
    }


    public ServiceResponse<string> RemoveUser(int id)
    {
        var response = new ServiceResponse<string>();
        var result = _authRepository.DeleteUser(id);

        if (result)
        {
            response.Message = "User deleted successfully";
        }
        else
        {
            response.Success = false;
            response.Message = "Something went wrong";
        }

        return response;
    }

        public ServiceResponse<string> RegisterUserService(RegisterUserDto register)
        {
            var response = new ServiceResponse<string>();
            var message = string.Empty;
            if (register != null)
            {
                message = _passwordService.CheckPasswordStrength(register.Password);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    response.Success = false;
                    response.Message = message;
                    return response;
                }
                else if (_authRepository.UserExists(register.LoginId, register.Email))
                {
                    response.Success = false;
                    response.Message = "User already exist";
                    return response;
                }
                else if (CalculateAge(register.BirthDate) > 120 || CalculateAge(register.BirthDate) < 18)
                {
                    response.Success = false;
                    response.Message = "Age should not be less than 18 and greater than 120";
                    return response;
                }
                else if (register.Gender.ToLower() != "m" && register.Gender.ToLower() != "f")
                {
                    response.Success = false;
                    response.Message = "Gender can be either M or F";
                    return response;
                }
                else if (register.Phone.Length > 12 || register.Phone.Length < 10)
                {
                    response.Success = false;
                    response.Message = "Enter valid phone number";
                    return response;
                }
                else
                {
                    User user = new User()
                    {
                        Salutation = register.Salutation,
                        Name = register.Name,
                        LoginId = register.LoginId,
                        Email = register.Email,
                        Phone = register.Phone,
                        BirthDate = register.BirthDate,
                        Gender = register.Gender,
                        SecurityQuestionId = register.SecurityQuestionId,
                        Answer = register.Answer,
                        IsAdmin = false,
                        Age = CalculateAge(register.BirthDate)
                    };

                    _passwordService.CreatePasswordHash(register.Password, out byte[] passwordHash, out byte[] passwordSalt);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    var result = _authRepository.RegisterUser(user);
                    response.Success = result;
                    response.Message = result ? string.Empty : "Something went wrong, please try after sometime";
                }
            }
            return response;

        }
        public ServiceResponse<IEnumerable<SecurityQuestion>> GetAllQuestions()
        {
            var response = new ServiceResponse<IEnumerable<SecurityQuestion>>();
            var questions = _authRepository.GetAllSecurityQuestions();
            if (questions != null && questions.Any())
            {

                response.Data = questions;
                response.Success = true;
            }
            else
            {
                response.Success = false;
                response.Message = "Data does not exists.";
            }

            return response;
        }

        [ExcludeFromCodeCoverage]
        private static int CalculateAge(DateTime birthdate)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - birthdate.Year;
            // Check if the birthday for this year has occurred or not
            if (birthdate.Date > today.AddYears(-age))
            {
                age--;
            }
            return age;
        }
        public ServiceResponse<string> ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var response = new ServiceResponse<string>();
            var message = string.Empty;
            if (changePasswordDto != null)
            {
                var user = _authRepository.ValidateUser(changePasswordDto.LoginId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Invalid username or password";
                    return response;
                }
                if (!_passwordService.VerifyPasswordHash(changePasswordDto.OldPassword, user.PasswordHash, user.PasswordSalt))
                {
                    response.Success = false;
                    response.Message = "Old password is incorrect.";
                    return response;
                }
                if (changePasswordDto.OldPassword == changePasswordDto.NewPassword)
                {
                    response.Success = false;
                    response.Message = "Old password and new password can not be same.";
                    return response;
                }
                message = _passwordService.CheckPasswordStrength(changePasswordDto.NewPassword);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    response.Success = false;
                    response.Message = message;
                    return response;
                }
                _passwordService.CreatePasswordHash(changePasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                var result = _authRepository.UpdateUser(user);
                response.Success = result;
                if (result)
                {
                    response.Message = "Successfully changed password, Please signin!";
                }
                else
                {
                    response.Message = "Something went wrong, please try after sometime";
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime";
            }
            return response;
        }
        public ServiceResponse<string> ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
            var response = new ServiceResponse<string>();
            if (forgetPasswordDto != null)
            {
                var user = _authRepository.ValidateUser(forgetPasswordDto.LoginId);
                if (user == null)
                {
                    response.Success = false;
                    response.Message = "Invalid username!";
                    return response;
                }
                if (forgetPasswordDto.SecurityQuestionId != user.SecurityQuestionId)
                {
                    response.Success = false;
                    response.Message = "User verification failed!";
                    return response;
                }
                forgetPasswordDto.Answer = forgetPasswordDto.Answer.Trim();
                if (forgetPasswordDto.Answer != user.Answer)
                {
                    response.Success = false;
                    response.Message = "User verification failed!";
                    return response;
                }
                var message = _passwordService.CheckPasswordStrength(forgetPasswordDto.NewPassword);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    response.Success = false;
                    response.Message = message;
                    return response;
                }
                _passwordService.CreatePasswordHash(forgetPasswordDto.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
                var result = _authRepository.UpdateUser(user);
                response.Success = result;
                if (result)
                {
                    response.Message = "Successfully reset password!";
                }
                else
                {
                    response.Message = "Something went wrong, please try after sometime";
                }
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime";
            }
            return response;
        }
    }
}
