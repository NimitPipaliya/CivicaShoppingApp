using CivicaShoppingAppApi.Controllers;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppApiTests.Controller
{
    public class ApiAuthControllerTests
    {
        [Fact]
        public void ChangePassword_ReturnsOkResponse_WhenChangePasswordSuccessfully()
        {
            var changePasswordDto = new ChangePasswordDto
            {
                LoginId = "loginId",
                OldPassword = "TestOldPassword@123",
                NewPassword = "TestNewPassword@123",
                NewConfirmPassword = "TestNewPassword@123"
            };
            var response = new ServiceResponse<string>
            {
                Success = true,
                Message = "Password changed successfully."
            };
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.ChangePassword(It.IsAny<ChangePasswordDto>())).Returns(response);

            //Act
            var actual = target.ChangePassword(changePasswordDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.ChangePassword(It.IsAny<ChangePasswordDto>()), Times.Once);
        }

        [Fact]
        public void ChangePassword_ReturnsBadRequest_WhenPasswordIsNotChanged()
        {
            // Arrange
            var changePasswordDto = new ChangePasswordDto
            {
                LoginId = "loginId",
                OldPassword = "TestOldPassword@123",
                NewPassword = "TestNewPassword@123",
                NewConfirmPassword = "TestNewPassword@123"
            };
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.ChangePassword(It.IsAny<ChangePasswordDto>())).Returns(response);

            // Act
            var actual = target.ChangePassword(changePasswordDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.ChangePassword(It.IsAny<ChangePasswordDto>()), Times.Once);
        }

        [Fact]
        public void ForgetPassword_ReturnsOkResponse_WhenResetPasswordSuccessfully()
        {
            var resetPasswordDto = new ForgetPasswordDto
            {
                LoginId = "loginId",
                SecurityQuestionId = 1,
                Answer = "Blue",
                NewPassword = "TestNewPassword@123",
                ConfirmNewPassword = "TestNewPassword@123"
            };
            var response = new ServiceResponse<string>
            {
                Success = true,
                Message = "Successfully reset password!"
            };

            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.ForgetPassword(It.IsAny<ForgetPasswordDto>())).Returns(response);

            //Act
            var actual = target.ForgetPassword(resetPasswordDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.ForgetPassword(It.IsAny<ForgetPasswordDto>()), Times.Once);
        }


        [Fact]
        public void ForgetPassword_ReturnsBadRequest_WhenPasswordIsNotReset()
        {
            // Arrange
            var resetPasswordDto = new ForgetPasswordDto
            {
                LoginId = "loginId",
                NewPassword = "TestNewPassword@123",
                ConfirmNewPassword = "TestNewPassword@123"
            };
            var response = new ServiceResponse<string>
            {
                Success = false,
                Message = "Something went wrong, please try after sometime"
            };
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.ForgetPassword(It.IsAny<ForgetPasswordDto>())).Returns(response);

            // Act
            var actual = target.ForgetPassword(resetPasswordDto) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.ForgetPassword(It.IsAny<ForgetPasswordDto>()), Times.Once);
        }
        [Theory]
        [InlineData("User already exists.")]
        [InlineData("Something went wrong, please try after sometime.")]
        [InlineData("Mininum password length should be 8")]
        [InlineData("Password should be apphanumeric")]
        [InlineData("Password should contain special characters")]
        public void Register_ReturnsBadRequest_WhenRegistrationFails(string message)
        {
            // Arrange
            var registerDto = new RegisterUserDto();
            var mockAuthService = new Mock<IAuthService>();
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = message

            };
            mockAuthService.Setup(service => service.RegisterUserService(registerDto))
                           .Returns(expectedServiceResponse);

            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.AddRegister(registerDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(message, ((ServiceResponse<string>)actual.Value).Message);
            Assert.False(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
            mockAuthService.Verify(service => service.RegisterUserService(registerDto), Times.Once);
        }
        [Fact]
        public void Register_ReturnsOk_WhenRegistrationSuccess()
        {
            // Arrange
            var registerDto = new RegisterUserDto();
            var mockAuthService = new Mock<IAuthService>();
            var message = "User Added Successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = message

            };
            mockAuthService.Setup(service => service.RegisterUserService(registerDto))
                           .Returns(expectedServiceResponse);

            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.AddRegister(registerDto) as OkObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(message, ((ServiceResponse<string>)actual.Value).Message);
            Assert.True(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.OK, actual.StatusCode);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
            mockAuthService.Verify(service => service.RegisterUserService(registerDto), Times.Once);
        }

        [Fact]
        public void GetAllSecurityQuestions_ReturnsOk_WhenQuestionsExists()
        {
            //Arrange

            var questions = new List<SecurityQuestion>
             {
            new SecurityQuestion {SecurityQuestionId=1,Question="Question 1"},
            new SecurityQuestion{ SecurityQuestionId = 2, Question = "Question 2"},
            };
            var response = new ServiceResponse<IEnumerable<SecurityQuestion>>
            {
                Success = true,
            };

            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.GetAllQuestions()).Returns(response);

            //Act
            var actual = target.GetAllSecurityQuestions() as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.GetAllQuestions(), Times.Once);
        }


        [Fact]
        public void GetAllSecurityQuestions_ReturnsNotFound_WhenQuestionsDoesnotExist()
        {
            //Arrange
            var response = new ServiceResponse<IEnumerable<SecurityQuestion>>
            {
                Success = false,
                Data = Enumerable.Empty<SecurityQuestion>()
            };
            var mockAuthService = new Mock<IAuthService>();
            var target = new AuthController(mockAuthService.Object);
            mockAuthService.Setup(c => c.GetAllQuestions()).Returns(response);

            //Act
            var actual = target.GetAllSecurityQuestions() as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockAuthService.Verify(c => c.GetAllQuestions(), Times.Once);
        }

        [Fact]
        public void GetAllUsers_ReturnsOkWithUsers_SearchIsNull()
        {
            //Arrange
            var users = new List<User>
            {
               new User{UserId=1,Name="User 1", Phone = "1234567890"},
                 new User{UserId=2,Name="User 2", Phone = "1234567899"},
             };

            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            var response = new ServiceResponse<IEnumerable<UserDto>>
            {
                Success = true,
                Data = users.Select(c => new UserDto { UserId = c.UserId, Name = c.Name, Phone = c.Phone }) // Convert to UserDto
            };

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.GetAllUsers(page, pageSize, null, sortOrder)).Returns(response);

            //Act
            var actual = target.GetAllUsers(null, page, pageSize,sortOrder) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockUserService.Verify(c => c.GetAllUsers(page, pageSize, null, sortOrder), Times.Once);
        }    
        
        [Fact]
        public void GetAllUsers_ReturnsOkWithUsers_SearchIsNotNull()
        {
            //Arrange
            var users = new List<User>
            {
               new User{UserId=1,Name="User 1", Phone = "1234567890"},
                 new User{UserId=2,Name="User 2", Phone = "1234567899"},
             };

            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string search = "C";
            var response = new ServiceResponse<IEnumerable<UserDto>>
            {
                Success = true,
                Data = users.Select(c => new UserDto { UserId = c.UserId, Name = c.Name, Phone = c.Phone }) // Convert to UserDto
            };

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.GetAllUsers(page, pageSize, search, sortOrder)).Returns(response);

            //Act
            var actual = target.GetAllUsers(search, page, pageSize,sortOrder) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockUserService.Verify(c => c.GetAllUsers(page, pageSize, search, sortOrder), Times.Once);
        }

        [Fact]
        public void GetAllUsers_ReturnsNotFound_WhenSearchIsNull()
        {
            //Arrange
            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            var response = new ServiceResponse<IEnumerable<UserDto>>
            {
                Success = false,
                Data = null
            };

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.GetAllUsers(page, pageSize, null, sortOrder)).Returns(response);

            //Act
            var actual = target.GetAllUsers(null,page, pageSize, sortOrder) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockUserService.Verify(c => c.GetAllUsers(page, pageSize, null, sortOrder), Times.Once);
        }
        
        [Fact]
        public void GetAllUsers_ReturnsNotFound_WhenSearchIsNotNull()
        {
            //Arrange
            int page = 1;
            int pageSize = 2;
            string sortOrder = "asc";
            string search = "C";

            var response = new ServiceResponse<IEnumerable<UserDto>>
            {
                Success = false,
                Data = null
            };

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.GetAllUsers(page, pageSize, search, sortOrder)).Returns(response);

            //Act
            var actual = target.GetAllUsers(search,page, pageSize, sortOrder) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockUserService.Verify(c => c.GetAllUsers(page, pageSize, search, sortOrder), Times.Once);
        }

        [Fact]

        public void GetUserById_ReturnsOk()
        {

            var userId = 1;
            var user = new User
            {

                UserId = userId,
                Name = "User 1"
            };

            var response = new ServiceResponse<UserDto>
            {
                Success = true,
                Data = new UserDto
                {
                    UserId = userId,
                    Name = user.Name
                }
            };

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.GetUser(userId)).Returns(response);

            //Act
            var actual = target.GetUserById(userId) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockUserService.Verify(c => c.GetUser(userId), Times.Once);
        }

        [Fact]

        public void GetUserById_ReturnsNotFound()
        {

            var userId = 1;
  
            var response = new ServiceResponse<UserDto>
            {
                Success = false,
                Data = null
            };

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.GetUser(userId)).Returns(response);

            //Act
            var actual = target.GetUserById(userId) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockUserService.Verify(c => c.GetUser(userId), Times.Once);
        }

        [Fact]
        public void RemoveUser_ReturnsOkResponse_WhenUserDeletedSuccessfully()
        {

            var userId = 1;
            var response = new ServiceResponse<string>
            {
                Success = true,
            };
            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.RemoveUser(userId)).Returns(response);

            //Act

            var actual = target.RemoveUser(userId) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockUserService.Verify(c => c.RemoveUser(userId), Times.Once);
        }

        [Fact]
        public void RemoveUser_ReturnsBadRequest_WhenUserNotDeleted()
        {

            var userId = 1;
            var response = new ServiceResponse<string>
            {
                Success = false,
            };
            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.RemoveUser(userId)).Returns(response);

            //Act

            var actual = target.RemoveUser(userId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockUserService.Verify(c => c.RemoveUser(userId), Times.Once);
        }

        [Fact]
        public void RemoveUser_ReturnsBadRequest_WhenUserIsLessThanZero()
        {

            var userId = 0;

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);

            //Act

            var actual = target.RemoveUser(userId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal("Enter correct data please", actual.Value);
        }

        [Fact]
        public void GetTotalCountOfUsers_ReturnsOkWithUsers_WhenSearchIsNull()
        {
            //Arrange
            var users = new List<User>
             {
            new User{UserId=1,Name="User 1", Phone = "1234567890"},
            new User{UserId=2,Name="User 2", Phone = "1234567899"},
            };


            var response = new ServiceResponse<int>
            {
                Success = true,
                Data = users.Count
            };

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.TotalUsers(null)).Returns(response);

            //Act
            var actual = target.GetTotalCountOfUsers(null) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal(2, response.Data);
            mockUserService.Verify(c => c.TotalUsers(null), Times.Once);
        }
        
        [Fact]
        public void GetTotalCountOfUsers_ReturnsOkWithUsers_WhenSearchIsNotNull()
        {
            //Arrange
            var users = new List<User>
             {
            new User{UserId=1,Name="User 1", Phone = "1234567890"},
            new User{UserId=2,Name="User 2", Phone = "1234567899"},
            };


            var response = new ServiceResponse<int>
            {
                Success = true,
                Data = users.Count
            };

            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.TotalUsers("n")).Returns(response);

            //Act
            var actual = target.GetTotalCountOfUsers("n") as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal(2, response.Data);
            mockUserService.Verify(c => c.TotalUsers("n"), Times.Once);
        }

        [Fact]
        public void GetTotalCountOfUsers_ReturnsNotFound_SearchIsNotNull()
        {
            var response = new ServiceResponse<int>
            {
                Success = false,
                Data = 0
            };

            var search = "d";
            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.TotalUsers(search)).Returns(response);

            //Act
            var actual = target.GetTotalCountOfUsers(search) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal(0, response.Data);
            mockUserService.Verify(c => c.TotalUsers(search), Times.Once);
        }
         
        [Fact]
        public void GetTotalCountOfUsers_ReturnsNotFound_WhenSearchIsNull()
        {
            var response = new ServiceResponse<int>
            {
                Success = false,
                Data = 0
            };

          
            var mockUserService = new Mock<IAuthService>();
            var target = new AuthController(mockUserService.Object);
            mockUserService.Setup(c => c.TotalUsers(null)).Returns(response);

            //Act
            var actual = target.GetTotalCountOfUsers(null) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            Assert.Equal(0, response.Data);
            mockUserService.Verify(c => c.TotalUsers(null), Times.Once);
        }
        [Theory]
        [InlineData("Invalid username or password!")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void Login_ReturnsBadRequest_WhenLoginFails(string message)
        {
            // Arrange
            var loginDto = new LoginDto();
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = message

            };
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(service => service.LoginUserService(loginDto))
                           .Returns(expectedServiceResponse);

            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.Login(loginDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(message, ((ServiceResponse<string>)actual.Value).Message);
            Assert.False(((ServiceResponse<string>)actual.Value).Success);
            Assert.Equal((int)HttpStatusCode.BadRequest, actual.StatusCode);
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(badRequestResult.Value);
            Assert.False(((ServiceResponse<string>)badRequestResult.Value).Success);
            mockAuthService.Verify(service => service.LoginUserService(loginDto), Times.Once);
        }
        [Fact]
        public void Login_ReturnsOk_WhenLoginSucceeds()
        {
            // Arrange
            var loginDto = new LoginDto { Username = "username", Password = "password" };
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = string.Empty

            };
            var mockAuthService = new Mock<IAuthService>();
            mockAuthService.Setup(service => service.LoginUserService(loginDto))
                           .Returns(expectedServiceResponse);

            var target = new AuthController(mockAuthService.Object);

            // Act
            var actual = target.Login(loginDto) as ObjectResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull((ServiceResponse<string>)actual.Value);
            Assert.Equal(string.Empty, ((ServiceResponse<string>)actual.Value).Message);
            Assert.True(((ServiceResponse<string>)actual.Value).Success);
            var okResult = Assert.IsType<OkObjectResult>(actual);
            Assert.IsType<ServiceResponse<string>>(okResult.Value);
            Assert.True(((ServiceResponse<string>)okResult.Value).Success);
            mockAuthService.Verify(service => service.LoginUserService(loginDto), Times.Once);
        }



    }
}

