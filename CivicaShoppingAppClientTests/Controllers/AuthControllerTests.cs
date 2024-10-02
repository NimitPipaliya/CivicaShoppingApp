using CivicaShoppingAppClient.Controllers;
using CivicaShoppingAppClient.Infrastructure;
using CivicaShoppingAppClient.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace CivicaShoppingAppClientTests.Controllers
{
    public class AuthControllerTests
    {
        //--------------------------Index---------------------------------------------
        [Fact]
        public void Index_ReturnsViewWithUsers_WhenResponseIsSuccessAndSearchIsNull()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sortOrder = "asc";

            var expectedUsers = new List<UserViewModel>()
            {
                new UserViewModel{UserId = 1, Name = "Test1"},
                new UserViewModel{UserId = 2, Name = "Test2"},
            };

            var count = 2;

            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Data = expectedUsers,
                Message = "",
                Success = true
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = count,
                Success = true
            };

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            //Act
            var actual = target.Index(searchString, page, pageSize, sortOrder) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(count, expectedCount.Data);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        public void Index_ReturnsRedirectToAction_WhenResponseIsNull()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sortOrder = "asc";

            var expectedUsers = new List<UserViewModel>()
            {
                new UserViewModel{UserId = 1, Name = "Test1"},
                new UserViewModel{UserId = 2, Name = "Test2"},
            };

            var count = 2;

            var expectedCount = new ServiceResponse<int>()
            {
                Data = count,
                Success = true
            };

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            //Act
            var actual = target.Index(searchString, page, pageSize, sortOrder) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Home", actual.ControllerName);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void Index_ReturnsViewWithUsers_WhenResponseIsSuccessAndSearchIsNotNull()
        {
            //Arrange
            string searchString = "Test1";
            int page = 1;
            int pageSize = 6;
            string sortOrder = "asc";

            var expectedUsers = new List<UserViewModel>()
            {
                new UserViewModel{UserId = 1, Name = "Test1"},
                new UserViewModel{UserId = 2, Name = "Test2"},
            };

            var count = 1;

            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Data = expectedUsers,
                Message = "",
                Success = true
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = count,
                Success = true
            };

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            //Act
            var actual = target.Index(searchString, page, pageSize, sortOrder) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(count, expectedCount.Data);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void Index_RedirectstoPageOne_WhenResponseIsSuccessAndPageIsGreaterThanPageSize()
        {
            //Arrange
            string searchString = null;
            int page = 3;
            int pageSize = 2;
            string sortOrder = "asc";

            var expectedUsers = new List<UserViewModel>()
            {
                new UserViewModel{UserId = 1, Name = "Test1"},
                new UserViewModel{UserId = 2, Name = "Test2"},
            };

            var count = 2;

            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Data = expectedUsers,
                Message = "",
                Success = true
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = count,
                Success = true
            };

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            //Act
            var actual = target.Index(searchString, page, pageSize, sortOrder) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(count, expectedCount.Data);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void Index_ReturnsEmptyView_WhenResponseIsSuccessAndSearchIsNull()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sortOrder = "asc";

            var expectedUsers = new List<UserViewModel>()
            {
            };

            var count = 0;

            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Data = expectedUsers,
                Message = "",
                Success = true
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = count,
                Success = true
            };

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            //Act
            var actual = target.Index(searchString, page, pageSize, sortOrder) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(count, expectedCount.Data);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void Index_ReturnsEmptyView_WhenUsersRetrivalFails()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sortOrder = "asc";

            var expectedUsers = new List<UserViewModel>()
            {
                new UserViewModel{UserId = 1, Name = "Test1"},
                new UserViewModel{UserId = 2, Name = "Test2"},
            };

            var count = 2;

            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserViewModel>>
            {
                Data = null,
                Message = "",
                Success = false
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = count,
                Success = true
            };

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            //Act
            var actual = target.Index(searchString, page, pageSize, sortOrder) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(count, expectedCount.Data);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }


        //-------------------------------------------Delete----------------------------------------
        [Fact]
        public void Delete_ReturnsRedirectToAction_WhenDeletedSuccessfully()
        {
            // Arrange
            var id = 1;

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = "Success",
                Success = true
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedServiceResponse);

            // Act
            var actual = target.Delete(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(expectedServiceResponse.Message, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void Delete_ReturnsRedirectToAction_WhenDeletionFailed()
        {
            var id = 1;

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = "Error",
                Success = false
            };
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedServiceResponse);

            // Act
            var actual = target.Delete(id) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(expectedServiceResponse.Message, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }

        //-----------------------------------------------------RegisterUser----------------------------------------
        [Fact]
        public void RegisterUser_ReturnsView()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockHttpContext = new Mock<HttpContext>();

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };

            var expectedResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse);

            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object,mockJwtTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            //Act
            var result = target.RegisterUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void RegisterUser_ModelIsInvalid()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel { Name = "firstname" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };

            var expectedResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedResponse);

            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            target.ModelState.AddModelError("LoginId", "Login id is required");
            //Act
            var actual = target.RegisterUser(registerViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(registerViewModel, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            Assert.False(target.ModelState.IsValid);
            //mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }

        [Fact]
        public void RegisterUser_RedirectToRegisterSuccess_WhenUserSavedSuccessfully()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            { Name = "firstname", Password = "Password@123", Email = "email@gmail.com", ConfirmPassword = "Password@123", LoginId = "loginid" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "User saved successfully";
            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = successMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var expectedQueResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.RegisterUser(registerViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("RegisterSuccess", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }

        

        [Fact]
        public void RegisterUser_ReturnView_WhenBadRequest()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            { Name = "firstname", Password = "Password@123", Email = "email@gmail.com", ConfirmPassword = "Password@123", LoginId = "loginid" };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "Error Occurs";

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };

            var expectedResponse1 = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse1);

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object,mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.RegisterUser(registerViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
            //mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }

        [Fact]
        public void RegisterUser_ReturnView_WhenBadRequest_WhenSomethingWentWrong()
        {
            // Arrange
            var registerViewModel = new RegisterViewModel
            { Name = "firstname", Password = "Password@123", Email = "email@gmail.com", ConfirmPassword = "Password@123", LoginId = "loginid" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };

            var expectedResponse1 = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse1);

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            //expectedResponse = null;
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.RegisterUser(registerViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong, please try after sometime", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), registerViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
            //mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        //------------------------------------------------RegisterSuccess---------------------------------------
        [Fact]
        public void RegisterSuccess_ReturnsView()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            //Act
            var result = target.RegisterSuccess() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        //------------------------------------LogoutUser-----------------------------
        //Logout
        [Fact]
        public void Logout_RedirectsToAction_WhenLogoutSuccessful()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new DefaultHttpContext();
            var mockTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");

            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext,
                }
            };

            // Act
            var actual = target.LogoutUser() as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Home", actual.ControllerName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
        }

        // LoginUser (Get)
        [Fact]
        public void LoginUser_ReturnsViews()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockHttpContext = new Mock<HttpContext>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();

            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            //Act
            var result = target.LoginUser() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }


        //-----------------------------------------------Login-------------------------------------------
        [Fact]
        public void Login_ModelIsInvalid()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            { Password = "Password@123" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["Endpoint:CivicaContactApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var mockTokenHandler = new Mock<IJwtTokenHandler>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            target.ModelState.AddModelError("UserName", "Username is required");
            //Act
            var actual = target.LoginUser(loginViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(loginViewModel, actual.Model);
            mockConfiguration.Verify(c => c["Endpoint:CivicaContactApi"], Times.Never);
            Assert.False(target.ModelState.IsValid);
        }


        [Fact]
        public void Login_ReturnView_WhenBadRequest()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            { Password = "Password@123", Username = "loginid" };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["Endpoint:CivicaContactApi"]).Returns("fakeEndPoint");
            var errorMessage = "Error Occurs";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.LoginUser(loginViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }

        [Fact]
        public void Login_Success_RedirectToAction()
        {
            // Arrange
            var loginViewModel = new LoginViewModel { Username = "loginid", Password = "Password" };
            var mockToken = "mockToken";
            var mockUserId = "1";
            var mockIsAdmin = "1";

            var mockResponseCookie = new Mock<IResponseCookies>();
            mockResponseCookie.Setup(c => c.Append("jwtToken", mockToken, It.IsAny<CookieOptions>()));
            mockResponseCookie.Setup(c => c.Append("userId", mockUserId, It.IsAny<CookieOptions>()));
            mockResponseCookie.Setup(c => c.Append("adminId", mockIsAdmin, It.IsAny<CookieOptions>()));

            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpResponse = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
            mockHttpResponse.SetupGet(c => c.Cookies).Returns(mockResponseCookie.Object);

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["Endpoint:CivicaContactApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Data = mockToken,
            };

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };

            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()))
                                 .Returns(expectedResponse);


            var claims = new[]
            {
                new Claim("UserId", mockUserId),
                new Claim("Admin", mockIsAdmin)
            };
            var jwtToken = new JwtSecurityToken(claims: claims);
            mockTokenHandler.Setup(t => t.ReadJwtToken(mockToken)).Returns(jwtToken);


            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var result = target.LoginUser(loginViewModel) as RedirectToActionResult;

            // Assert
            Assert.NotNull(result);
            mockResponseCookie.Verify(c => c.Append("jwtToken", mockToken, It.IsAny<CookieOptions>()), Times.Once);
            mockResponseCookie.Verify(c => c.Append("userId", mockUserId, It.IsAny<CookieOptions>()), Times.Once);
            Assert.True(target.ModelState.IsValid);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()), Times.Once);
        }


        [Fact]
        public void Login_RedirectToAction_WhenBadRequest_WhenResponseIsNull()
        {
            // Arrange
            var loginViewModel = new LoginViewModel
            { Password = "Password@123", Username = "loginid" }; var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockToken = new Mock<IJwtTokenHandler>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockToken.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            //Act
            var actual = target.LoginUser(loginViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong, please try after sometime", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), loginViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }

        //---------------------------------------------ForgetPassword--------------------------------------
        [Fact]
        public void ForgetPassword_ReturnsView()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockHttpContext = new Mock<HttpContext>();

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };

            var expectedResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse);

            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            //Act
            var result = target.ForgetPassword() as ViewResult;

            // Assert
            Assert.NotNull(result);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void ForgetPassword_ModelIsInvalid()
        {
            // Arrange
            var forgetPassword = new ForgetPasswordViewModel { Answer = "firstname" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };

            var expectedResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedResponse);

            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            target.ModelState.AddModelError("LoginId", "Login id is required");
            //Act
            var actual = target.ForgetPassword(forgetPassword) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(forgetPassword, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            Assert.False(target.ModelState.IsValid);
            //mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }

        [Fact]
        public void ForgetPassword_RedirectToHome_WhenPasswordSavedSuccessfully()
        {
            // Arrange
            var forgetPassword = new ForgetPasswordViewModel
            { Answer = "firstname", NewPassword = "Password@123", ConfirmNewPassword = "Password@123", LoginId = "loginid" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "Password reset successful";
            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,
                Message = successMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var expectedQueResponse = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), forgetPassword, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
               .Returns(expectedQueResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.ForgetPassword(forgetPassword) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), forgetPassword, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }



        [Fact]
        public void ForgetPassword_ReturnView_WhenBadRequest()
        {
            // Arrange
            var forgetPassword = new ForgetPasswordViewModel
            { Answer = "firstname", NewPassword = "Password@123", ConfirmNewPassword = "Password@123", LoginId = "loginid" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "Error Occurs";

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };

            var expectedResponse1 = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse1);

            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), forgetPassword, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.ForgetPassword(forgetPassword) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), forgetPassword, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
            //mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }

        [Fact]
        public void ForgetPassword_ReturnView_WhenBadRequest_WhenSomethingWentWrong()
        {
            // Arrange
            var forgetPassword = new ForgetPasswordViewModel
            { Answer = "firstname", NewPassword = "Password@123", ConfirmNewPassword = "Password@123", LoginId = "loginid" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var questions = new List<SecurityQuestionViewModel>()
            {
                new SecurityQuestionViewModel{SecurityQuestionId =1, Question = "TestQue1"},
                new SecurityQuestionViewModel{SecurityQuestionId =2, Question = "TestQue2"},
            };

            var expectedResponse1 = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>
            {
                Success = true,
                Data = questions
            };

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60))
                .Returns(expectedResponse1);

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            //expectedResponse = null;
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), forgetPassword, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.ForgetPassword(forgetPassword) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong, please try after sometime", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), forgetPassword, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
            //mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        // ChnagePassword
        [Fact]
        public void ChnagePassword_ReturnsViews()
        {
            // Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            var mockHttpContext = new Mock<HttpContext>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            //Act
            var result = target.ChangePassword() as ViewResult;

            // Assert
            Assert.NotNull(result);
        }

        [Fact]
        public void ChangePassword_ModelIsInvalid()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            { NewPassword = "Password@123" };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            target.ModelState.AddModelError("OldPassword", "Old Password is required");

            //Act
            var actual = target.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(changePasswordViewModel, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            Assert.False(target.ModelState.IsValid);
        }

        [Fact]
        public void ChnagePAssword_RedirectToAction_WhenBadRequest()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = "Oldpassword@123",
                NewPassword = "Password@123",
                NewConfirmPassword = "Password@123"
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "Error Occurs";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.NotFound)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);

        }

        [Fact]
        public void ChnagePassword_Success_RedirectToAction()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = "Oldpassword@123",
                NewPassword = "Password@123",
                NewConfirmPassword = "Password@123"
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Success = true,

            };
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()))
             .Returns(expectedResponse);

            var mockResponseCookie = new Mock<IResponseCookies>();
            mockResponseCookie.Setup(c => c.Delete("jwtToken"));
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpResponse = new Mock<HttpResponse>();
            mockHttpContext.SetupGet(c => c.Response).Returns(mockHttpResponse.Object);
            mockHttpResponse.SetupGet(c => c.Cookies).Returns(mockResponseCookie.Object);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.ChangePassword(changePasswordViewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("LoginUser", actual.ActionName);
            Assert.Equal("Auth", actual.ControllerName);
            Assert.True(target.ModelState.IsValid);
            mockResponseCookie.Verify(c => c.Delete("jwtToken"), Times.Once);
        }

        [Fact]
        public void ChnagePassword_RedirectToAction_WhenBadRequest_WhenResponseIsNull()
        {
            // Arrange
            var changePasswordViewModel = new ChangePasswordViewModel
            {
                OldPassword = "Oldpassword@123",
                NewPassword = "Password@123",
                NewConfirmPassword = "Password@123"
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var mockJwtTokenHandler = new Mock<IJwtTokenHandler>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedResponse = new HttpResponseMessage(HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()))
               .Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new AuthController(mockHttpClientService.Object, mockConfiguration.Object, mockJwtTokenHandler.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            //Act
            var actual = target.ChangePassword(changePasswordViewModel) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong, please try after sometime", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), changePasswordViewModel, It.IsAny<HttpRequest>()), Times.Once);
            Assert.True(target.ModelState.IsValid);
        }
    }
}
