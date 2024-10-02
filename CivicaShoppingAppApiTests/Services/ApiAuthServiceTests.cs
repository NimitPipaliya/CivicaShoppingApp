using AutoFixture;
using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Contract;
using CivicaShoppingAppApi.Services.Implementation;
using Fare;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppApiTests.Services
{
    public class ApiAuthServiceTests
    {
        // ChangePassword
        [Fact]
        public void ChangePassword_ReturnsErrorMessage_WhenExistingUerIsNull()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                NewConfirmPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Invalid username or password"
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockTokenService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns<User>(null);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
        }

        [Fact]
        public void ChangePassword_ReturnsErrorMessage_WhenNewAndOldPasswordIsSame()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "NewTest@123",
                NewPassword = "NewTest@123",
                NewConfirmPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Old password and new password can not be same."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                LoginId = changePasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            mockTokenService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            var target = new AuthService(mockAuthRepository.Object, mockTokenService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void ChangePassword_ReturnsErrorMessage_WhenVerifyPasswordHashFails()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                NewConfirmPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Old password is incorrect."
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                LoginId = changePasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockTokenService.Object);
            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(false);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
        }

        [Fact]
        public void ChangePassword_ReturnsErrorMessage_WhenUpdationFails()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                NewConfirmPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Something went wrong, please try after sometime"
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                LoginId = changePasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockTokenService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockAuthRepository.Setup(p => p.UpdateUser(It.IsAny<User>())).Returns(false);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockAuthRepository.Verify(p => p.UpdateUser(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public void ChangePassword_ReturnsSuccessMessage_WhenUpdatedSuccessfully()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "Test@123",
                NewPassword = "NewTest@123",
                NewConfirmPassword = "NewTest@123"
            };

            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = true,
                Message = "Successfully changed password, Please signin!"
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                LoginId = changePasswordDto.LoginId,
            };

            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockTokenService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockAuthRepository.Setup(p => p.UpdateUser(It.IsAny<User>())).Returns(true);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockAuthRepository.Verify(p => p.UpdateUser(It.IsAny<User>()), Times.Once);
        }
        [Fact]
        public void ChangePassword_ReturnsErrorMessage_WhenPasswordStrengthIsNotProper()
        {
            var changePasswordDto = new ChangePasswordDto()
            {
                LoginId = "Test",
                OldPassword = "NewTest@123",
                NewPassword = "test",
                NewConfirmPassword = "test"
            };
            string message = "Mininum password length should be 8\r\nPassword should be alphanumeric\r\nPassword should contain special characters\r\n";
            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = message
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                LoginId = changePasswordDto.LoginId,
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            mockTokenService.Setup(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>())).Returns(true);
            mockTokenService.Setup(x => x.CheckPasswordStrength(changePasswordDto.NewPassword)).Returns(message);
            var target = new AuthService(mockAuthRepository.Object, mockTokenService.Object);

            mockAuthRepository.Setup(c => c.ValidateUser(changePasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ChangePassword(changePasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal(response.Message, actual.Message);
            mockAuthRepository.Verify(c => c.ValidateUser(changePasswordDto.LoginId), Times.Once);
            mockTokenService.Verify(x => x.VerifyPasswordHash(changePasswordDto.OldPassword, It.IsAny<byte[]>(), It.IsAny<byte[]>()), Times.Once);
            mockTokenService.Verify(x => x.CheckPasswordStrength(changePasswordDto.NewPassword), Times.Once);
        }

        [Fact]
        public void ChangePassword_ReturnsErrorMessage_WhenDtoIsNull()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto() { };
            changePasswordDto = null;
            var response = new ServiceResponse<ChangePasswordDto>()
            {
                Data = changePasswordDto,
                Success = false,
                Message = "Something went wrong, please try after sometime"
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockAuthRepository.Object, mockTokenService.Object);

            // Act
            var actual = target.ChangePassword(changePasswordDto);

            // Assert
            Assert.NotNull(actual);
            Assert.False(actual.Success);
            Assert.Equal(response.Message, actual.Message);
        }

        // Forget Password
        // Reset password

        [Fact]
        public void ForgetPassword_ReturnsSuccessMessage_WhenDtoIsNull()
        {
            // Arrange
            var forgetPasswordDto = new ForgetPasswordDto() { };
            forgetPasswordDto = null;
            var response = new ServiceResponse<ForgetPasswordDto>()
            {
                Data = forgetPasswordDto,
                Success = false,
                Message = "Something went wrong, please try after sometime"
            };

            var mockUserRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockUserRepository.Object, mockTokenService.Object);

            // Act
            var actual = target.ForgetPassword(forgetPasswordDto);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
        }

        [Fact]
        public void ForgetPassword_ReturnsErrorMessage_WhenExistingUerIsNull()
        {
            // Arrange
            var forgetPasswordDto = new ForgetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ForgetPasswordDto>()
            {
                Data = forgetPasswordDto,
                Success = false,
                Message = "Invalid username!"
            };

            var mockUserRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockUserRepository.Object, mockTokenService.Object);
            mockUserRepository.Setup(c => c.ValidateUser(forgetPasswordDto.LoginId)).Returns<User>(null);

            // Act
            var actual = target.ForgetPassword(forgetPasswordDto);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(forgetPasswordDto.LoginId), Times.Once);
        }

        [Fact]
        public void ForgetPassword_ReturnsErrorMessage_WhenQuestionSelectedIsNotSame()
        {
            var forgetPasswordDto = new ForgetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 2,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ForgetPasswordDto>()
            {
                Data = forgetPasswordDto,
                Success = false,
                Message = "User verification failed!"
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                SecurityQuestionId = 1,
                LoginId = forgetPasswordDto.LoginId,
            };
            var mockUserRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(forgetPasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ForgetPassword(forgetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(forgetPasswordDto.LoginId), Times.Once);
        }

        [Fact]
        public void ForgetPassword_ReturnsErrorMessage_WhenAnswerVerificationFails()
        {
            var forgetPasswordDto = new ForgetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ForgetPasswordDto>()
            {
                Data = forgetPasswordDto,
                Success = false,
                Message = "User verification failed!"
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                SecurityQuestionId = 1,
                LoginId = forgetPasswordDto.LoginId,
            };
            var mockUserRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(forgetPasswordDto.LoginId)).Returns(user);

            //Act
            var actual = target.ForgetPassword(forgetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(forgetPasswordDto.LoginId), Times.Once);
        }

        [Fact]
        public void ForgetPassword_ReturnsErrorMessage_WhenPasswordStrengthIsWeak()
        {
            //Arrange
            var forgetPasswordDto = new ForgetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "test",
                ConfirmNewPassword = "test"
            };
            string message = "Mininum password length should be 8\r\nPassword should be alphanumeric\r\nPassword should contain special characters\r\n";
            var response = new ServiceResponse<ForgetPasswordDto>()
            {
                Data = forgetPasswordDto,
                Success = false,
                Message = message
            };
            var user = new User()
            {
                UserId = 1,
                Name = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                LoginId = forgetPasswordDto.LoginId,
            };
            var mockUserRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(forgetPasswordDto.LoginId)).Returns(user);
            mockTokenService.Setup(x => x.CheckPasswordStrength(forgetPasswordDto.NewPassword)).Returns(message);

            //Act
            var actual = target.ForgetPassword(forgetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(forgetPasswordDto.LoginId), Times.Once);
            mockTokenService.Setup(x => x.CheckPasswordStrength(forgetPasswordDto.NewPassword)).Returns(message);
        }

        [Fact]
        public void ForgetPassword_ReturnsErrorMessage_WhenPasswordUpdateFails()
        {
            //Arrange
            var forgetPasswordDto = new ForgetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ForgetPasswordDto>()
            {
                Data = forgetPasswordDto,
                Success = false,
                Message = "Something went wrong, please try after sometime"
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                LoginId = forgetPasswordDto.LoginId,
            };

            var mockUserRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockUserRepository.Object, mockTokenService.Object);

            mockUserRepository.Setup(c => c.ValidateUser(forgetPasswordDto.LoginId)).Returns(user);
            mockUserRepository.Setup(x => x.UpdateUser(user)).Returns(false);

            //Act
            var actual = target.ForgetPassword(forgetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(forgetPasswordDto.LoginId), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUser(user), Times.Once);
        }

        [Fact]
        public void ForgetPassword_ReturnsSuccess_WhenPasswordResetSuccessfully()
        {
            //Arrange
            var forgetPasswordDto = new ForgetPasswordDto()
            {
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                NewPassword = "Test@1234",
                ConfirmNewPassword = "Test@1234"
            };

            var response = new ServiceResponse<ForgetPasswordDto>()
            {
                Data = forgetPasswordDto,
                Success = true,
                Message = "Successfully reset password!"
            };

            var user = new User()
            {
                UserId = 1,
                Name = "test",
                SecurityQuestionId = 1,
                Answer = "testAnswer",
                LoginId = forgetPasswordDto.LoginId,
            };

            var mockUserRepository = new Mock<IAuthRepository>();
            var mockTokenService = new Mock<IPasswordService>();
            var target = new AuthService(mockUserRepository.Object, mockTokenService.Object);
            mockUserRepository.Setup(c => c.ValidateUser(forgetPasswordDto.LoginId)).Returns(user);
            mockUserRepository.Setup(x => x.UpdateUser(user)).Returns(true);

            //Act
            var actual = target.ForgetPassword(forgetPasswordDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockUserRepository.Verify(c => c.ValidateUser(forgetPasswordDto.LoginId), Times.Once);
            mockUserRepository.Verify(x => x.UpdateUser(user), Times.Once);
        }

        [Fact]
        public void RegisterUserService_ReturnsPasswordWeak_WhenPasswordIsWeak()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Password = "pass",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns("Password too weak");

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Password too weak", result.Message);
        }

        [Fact]
        public void RegisterUserService_ReturnsUserAlreadyExists_WhenUserExists()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                LoginId = "existingUserLoginId",
                Email = "existingUser@example.com",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(true);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("User already exist", result.Message);
        }

        [Fact]
        public void RegisterUserService_ReturnsInvalidAge_WhenAgeOutOfRange()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                BirthDate = DateTime.Now.AddYears(-121),
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Age should not be less than 18 and greater than 120", result.Message);
        }

        [Fact]
        public void RegisterUserService_ReturnsInvalidGender_WhenGenderIsInvalid()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = "unknown",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Gender can be either M or F", result.Message);
        }

        [Fact]
        public void RegisterUserService_ReturnsInvalidPhoneNumber_WhenPhoneNumberIsInvalid_DigitLessThan10()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = "M",
                Phone = "123",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Enter valid phone number", result.Message);
        }

        [Fact]
        public void RegisterUserService_ReturnsInvalidPhoneNumber_WhenPhoneNumberIsInvalid_DigitMoreThan12()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = "M",
                Phone = "12322233344445",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Enter valid phone number", result.Message);
        }
        [Fact]
        public void RegisterUserService_ReturnsSuccess_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Password = "ValidPassword@123",
                LoginId = "newLoginId",
                Email = "newUser@example.com",
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = "M",
                Phone = "1234567890",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);
            mockAuthRepository.Setup(repo => repo.RegisterUser(It.IsAny<User>())).Returns(true);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(string.Empty, result.Message);
        }

        [Fact]
        public void RegisterUserService_ReturnsSomethingWentWrong_WhenRegistrationFails()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();

            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var registerDto = new RegisterUserDto
            {
                Password = "ValidPassword@123",
                LoginId = "newLoginId",
                Email = "newUser@example.com",
                BirthDate = DateTime.Now.AddYears(-25),
                Gender = "M",
                Phone = "1234567890",
            };

            mockPasswordService.Setup(service => service.CheckPasswordStrength(registerDto.Password)).Returns(string.Empty);
            mockAuthRepository.Setup(repo => repo.UserExists(registerDto.LoginId, registerDto.Email)).Returns(false);
            mockAuthRepository.Setup(repo => repo.RegisterUser(It.IsAny<User>())).Returns(false);

            // Act
            var result = authService.RegisterUserService(registerDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, please try after sometime", result.Message);
        }

        [Fact]
        public void GetAllQuestions_ReturnsQuestions_WhenDataExists()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            var mockQuestions = new List<SecurityQuestion>
            {
                new SecurityQuestion { SecurityQuestionId = 1, Question = "Question 1" },
                new SecurityQuestion { SecurityQuestionId = 2, Question = "Question 2" }
            };

            mockAuthRepository.Setup(repo => repo.GetAllSecurityQuestions()).Returns(mockQuestions);

            // Act
            var result = authService.GetAllQuestions();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(mockQuestions, result.Data);
        }

        [Fact]
        public void GetAllQuestions_ReturnsDataDoesNotExist_WhenNoQuestions()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(repo => repo.GetAllSecurityQuestions()).Returns(new List<SecurityQuestion>());

            // Act
            var result = authService.GetAllQuestions();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Data does not exists.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]
        public void GetAllQuestions_ReturnsDataDoesNotExist_WhenQuestionsIsNull()
        {
            // Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockPasswordService = new Mock<IPasswordService>();
            var authService = new AuthService(mockAuthRepository.Object, mockPasswordService.Object);

            mockAuthRepository.Setup(repo => repo.GetAllSecurityQuestions()).Returns((IEnumerable<SecurityQuestion>)null);

            // Act
            var result = authService.GetAllQuestions();

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Data does not exists.", result.Message);
            Assert.Null(result.Data);
        }

        [Fact]

        public void GetAllUsers_ReturnsUsers_WhenUsersExist_SearchIsNull()
        {

            // Arrange
            string sortOrder = "asc";
            var users = new List<User>
        {
            new User
            {
                UserId = 1,
                Name = "John",
                Email = "john@example.com",
                Phone = "1234567890",
                Gender = "Male",
                BirthDate = new DateTime(),
                Age = 2,
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "black"
            },

                new User
            {
                UserId = 2,
                Salutation = "Mr.",
                Name = "Jane",
                Email = "jane@example.com",
                Phone = "9876543210",
                Gender = "Female",
                BirthDate = new DateTime(),
                Age = 2,
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "black"

            }
        };
            int page = 1;
            int pageSize = 2;

            var mockRepository = new Mock<IAuthRepository>();
            var passwordService = new Mock<IPasswordService>();
            mockRepository.Setup(r => r.GetAllUsers(page, pageSize, null, sortOrder)).Returns(users);

            var userService = new AuthService(mockRepository.Object, passwordService.Object);

            // Act
            var actual = userService.GetAllUsers(page, pageSize, null, sortOrder);

            // Assert
            Assert.True(actual.Success);
            Assert.NotNull(actual.Data);
            Assert.Equal(users.Count, actual.Data.Count());
            mockRepository.Verify(r => r.GetAllUsers(page, pageSize, null, sortOrder), Times.Once);
        }

        [Fact]

        public void GetAllUsers_ReturnsNoUsers_WhenNoUsersExist_SearchIsNull()
        {

            // Arrange
            string sortOrder = "asc";
            int page = 1;
            int pageSize = 2;

            var mockRepository = new Mock<IAuthRepository>();
            var passwordService = new Mock<IPasswordService>();
            mockRepository.Setup(r => r.GetAllUsers(page, pageSize, null, sortOrder)).Returns<IEnumerable<User>>(null);

            var userService = new AuthService(mockRepository.Object, passwordService.Object);

            // Act
            var actual = userService.GetAllUsers(page, pageSize, null, sortOrder);

            // Assert
            Assert.False(actual.Success);
            Assert.Null(actual.Data);
            mockRepository.Verify(r => r.GetAllUsers(page, pageSize, null, sortOrder), Times.Once);
        }

        [Fact]
        public void TotalUsers_ReturnsUsers_WhenSearchIsNotNull()
        {
            string search = "abc";
            var users = new List<User>
        {
            new User
            {
                UserId = 1,
                Name = "John"

            },
            new User
            {
                UserId = 2,
                Name = "Jane"

            }
        };

            var mockRepository = new Mock<IAuthRepository>();
            mockRepository.Setup(r => r.TotalUsers(search)).Returns(users.Count);
            var passwordService = new Mock<IPasswordService>();

            var userService = new AuthService(mockRepository.Object, passwordService.Object);

            // Act
            var actual = userService.TotalUsers(search);

            // Assert
            Assert.True(actual.Success);
            Assert.Equal(users.Count, actual.Data);
            mockRepository.Verify(r => r.TotalUsers(search), Times.Once);
        }

        [Fact]
        public void GetUser_ReturnsUser_WhenUserExist()
        {
            // Arrange
            var userId = 1;
            var user = new User
            {
                UserId = 2,
                Salutation = "Mr.",
                Name = "Jane",
                Email = "jane@example.com",
                Phone = "9876543210",
                Gender = "Female",
                BirthDate = new DateTime(),
                Age = 2,
                LoginId = "test",
                SecurityQuestionId = 1,
                Answer = "black"

            };

            var mockRepository = new Mock<IAuthRepository>();
            mockRepository.Setup(r => r.GetUser(userId)).Returns(user);
            var passwordService = new Mock<IPasswordService>();

            var userService = new AuthService(mockRepository.Object, passwordService.Object);

            // Act
            var actual = userService.GetUser(userId);

            // Assert
            Assert.True(actual.Success);
            Assert.NotNull(actual.Data);
            mockRepository.Verify(r => r.GetUser(userId), Times.Once);
        }

        [Fact]
        public void GetUser_ReturnsNoRecord_WhenNoUsersExist()
        {
            // Arrange
            var userId = 1;


            var mockRepository = new Mock<IAuthRepository>();
            mockRepository.Setup(r => r.GetUser(userId)).Returns<IEnumerable<User>>(null);
            var passwordService = new Mock<IPasswordService>();
            var userService = new AuthService(mockRepository.Object, passwordService.Object);

            // Act
            var actual = userService.GetUser(userId);

            // Assert
            Assert.False(actual.Success);
            Assert.Null(actual.Data);
            Assert.Equal("Something went wrong,try after sometime", actual.Message);
            mockRepository.Verify(r => r.GetUser(userId), Times.Once);
        }

        [Fact]
        public void RemoveUser_ReturnsDeletedSuccessfully_WhenDeletedSuccessfully()
        {
            var userId = 1;


            var mockRepository = new Mock<IAuthRepository>();
            mockRepository.Setup(r => r.DeleteUser(userId)).Returns(true);
            var passwordService = new Mock<IPasswordService>();

            var userService = new AuthService(mockRepository.Object, passwordService.Object);

            // Act
            var actual = userService.RemoveUser(userId);

            // Assert
            Assert.True(actual.Success);
            Assert.NotNull(actual);
            Assert.Equal("User deleted successfully", actual.Message);
            mockRepository.Verify(r => r.DeleteUser(userId), Times.Once);
        }

        [Fact]
        public void RemoveUser_SomethingWentWrong_WhenDeletionFailed()
        {
            var userId = 1;


            var mockRepository = new Mock<IAuthRepository>();
            mockRepository.Setup(r => r.DeleteUser(userId)).Returns(false);
            var passwordService = new Mock<IPasswordService>();

            var userService = new AuthService(mockRepository.Object, passwordService.Object);

            // Act
            var actual = userService.RemoveUser(userId);

            // Assert
            Assert.False(actual.Success);
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong", actual.Message);
            mockRepository.Verify(r => r.DeleteUser(userId), Times.Once);
        }

        [Fact]
        public void LoginUserService_ReturnsSomethingWentWrong_WhenLoginDtoIsNull()
        {
            //Arrange
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IPasswordService>();

            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(null);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, please try after some time", result.Message);

        }
        [Fact]
        public void LoginUserService_ReturnsInvalidUsernameOrPassword_WhenUserIsNull()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username"
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IPasswordService>();
            mockAuthRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns<User>(null);

            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid user login id or password", result.Message);
            mockAuthRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);


        }
        [Fact]
        public void LoginUserService_ReturnsInvalidUsernameOrPassword_WhenPasswordIsWrong()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username",
                Password = "password"
            };
            var user = new User
            {
                UserId = 1,
                LoginId = "username",
                Email = "abc@gmail.com"
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IPasswordService>();
            mockAuthRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns(user);

            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Invalid user login id or password", result.Message);
            mockAuthRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);


        }

        [Fact]
        public void LoginUserService_ReturnsResponse_WhenLoginIsSuccessful()
        {
            //Arrange
            var loginDto = new LoginDto
            {
                Username = "username",
                Password = "password"
            };
            var user = new User
            {
                UserId = 1,
                LoginId = "username",
                Email = "abc@gmail.com"
            };
            var mockAuthRepository = new Mock<IAuthRepository>();
            var mockConfiguration = new Mock<IPasswordService>();
            mockAuthRepository.Setup(repo => repo.ValidateUser(loginDto.Username)).Returns(user);
            mockConfiguration.Setup(repo => repo.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt)).Returns(true);
            mockConfiguration.Setup(repo => repo.CreateToken(user)).Returns("");

            var target = new AuthService(mockAuthRepository.Object, mockConfiguration.Object);


            // Act
            var result = target.LoginUserService(loginDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            mockAuthRepository.Verify(repo => repo.ValidateUser(loginDto.Username), Times.Once);
            mockConfiguration.Verify(repo => repo.VerifyPasswordHash(loginDto.Password, user.PasswordHash, user.PasswordSalt), Times.Once);
            mockConfiguration.Verify(repo => repo.CreateToken(user), Times.Once);


        }


    }
}
