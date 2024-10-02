using CivicaShoppingAppClient.Controllers;
using CivicaShoppingAppClient.Infrastructure;
using CivicaShoppingAppClient.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing.Printing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppClientTests.Controllers
{
    public class OrderControllerTests
    {
        [Fact]
        public void Index_ReturnsView()
        {
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var mockHttpContext = new Mock<HttpContext>();
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            var actual = target.Index() as ViewResult;

            Assert.NotNull(actual);
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsCorrectView()
        {
            // Arrange
            int userId = 2;
            int page = 1;
            int pageSize = 2;
            string sort_direction = "asc";


            var allOrders = new List<OrderListViewModel> {
              new OrderListViewModel { OrderNumber =1,OrderDate = new DateTime()},
              new OrderListViewModel { OrderNumber =2,OrderDate = new DateTime() },
            };
            var expectedResponse = new ServiceResponse<IEnumerable<OrderListViewModel>>
            {
                Success = true,
                Data = allOrders
            };


            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService
              .Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                  It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60))
              .Returns(new ServiceResponse<int> { Success = true, Data = 2 }); // Mocking totalCount as 3


            mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<OrderListViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);
 

            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.GetAllOrdersByUserId(userId, page, pageSize, sort_direction) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<List<OrderListViewModel>>(actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService
                .Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60), Times.Once);
            mockHttpClientService
              .Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<OrderListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60), Times.Once);
        }
        
        [Fact]
        public void GetAllOrdersByUserId_ReturnsEmptyList_WhenCountIsZero()
        {
            // Arrange
            int userId = 2;
            int page = 1;
            int pageSize = 2;
            string sort_direction = "asc";
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService
              .Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                  It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60))
              .Returns(new ServiceResponse<int> { Success = true, Data = 0 }); // Mocking totalCount as 3

            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.GetAllOrdersByUserId(userId, page, pageSize, sort_direction) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<List<OrderListViewModel>>(actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService
                .Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60), Times.Once);
        }
        
        [Fact]
        public void GetAllOrdersByUserId_ReturnsRedirectToAction_WhenPageIsGreaterThanTotalPages()
        {
            // Arrange
            int userId = 2;
            int page = 4;
            int pageSize = 2;
            string sort_direction = "asc";
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService
              .Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                  It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60))
              .Returns(new ServiceResponse<int> { Success = true, Data = 6 }); // Mocking totalCount as 3

            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.GetAllOrdersByUserId(userId, page, pageSize, sort_direction) as RedirectToActionResult;
            var redirectToActionResult = Assert.IsType<RedirectToActionResult>(actual);
            Assert.Equal("GetAllOrdersByUserId", redirectToActionResult.ActionName);
            Assert.Equal(1, redirectToActionResult.RouteValues["page"]);
            Assert.Equal(pageSize, redirectToActionResult.RouteValues["pageSize"]);
            // Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService
                .Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60), Times.Once);
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsEmptyList_WhenSuccessIsFalse()
        {
            // Arrange
            int userId = 2;
            int page = 1;
            int pageSize = 2;
            string sort_direction = "asc";


            var allOrders = new List<OrderListViewModel> {

            };
            var expectedResponse = new ServiceResponse<IEnumerable<OrderListViewModel>>
            {
                Success = false,
                Data = allOrders
            };


            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService
              .Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                  It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60))
              .Returns(new ServiceResponse<int> { Success = true, Data = 2 }); // Mocking totalCount as 3


            mockHttpClientService
                .Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<OrderListViewModel>>>(
                    It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60))
                .Returns(expectedResponse);


            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new Microsoft.AspNetCore.Mvc.ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };

            // Act
            var actual = target.GetAllOrdersByUserId(userId, page, pageSize, sort_direction) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.IsType<List<OrderListViewModel>>(actual.Model);
            Assert.Equal(0, expectedResponse.Data.Count());
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService
                .Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(
                    It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60), Times.Once);
            mockHttpClientService
              .Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<OrderListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, mockHttpContext.Object.Request, null, 60), Times.Once);
        }

        [Fact]
        public void OrderSummary_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var viewModel = new List<OrderSummaryViewModel>() {
             new OrderSummaryViewModel{
                OrderNumber = 1,
                OrderId = 1,
                UserId = id,
                ProductId = 1,
                OrderDate = new DateTime(),
            }
             }; 
            
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedServiceResponse = new ServiceResponse<IEnumerable<OrderSummaryViewModel>>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderSummary(id) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }
        
        [Fact]
        public void OrderSummary_WhenStatusCodeIsSuccess_ReturnsRedirectToAction_ServiceResponseIsNull()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
                TempData = tempData,


            };
        
            //Act
            var actual = target.OrderSummary(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void OrderSummary_WhenStatusCodeIsSuccess_ReturnsRedirectToAction_DataIsNull()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<OrderSummaryViewModel>
            {
                Message = "",
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderSummary(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

         [Fact]
        public void OrderSummary_WhenStatusCodeIsSuccess_ReturnsRedirectToAction_SuccesIsFalse()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<OrderSummaryViewModel>
            {
                Message = "",
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderSummary(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void OrderSummary_ReturnsRedirectToAction_WhenFails()
        {
            //Arrange
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = errorMessage

            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderSummary(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void OrderSummary_ReturnsRedirectToAction_SomethingWentWrong()
        {
            //Arrange
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "Something went wrong. Please try after sometime.";
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderSummary(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }


        [Fact]
        public void OrderPlaced_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var viewModel = new List<OrderSummaryViewModel>() {
             new OrderSummaryViewModel{
                OrderNumber = 1,
                OrderId = 1,
                UserId = id,
                ProductId = 1,
                OrderDate = new DateTime(),
            }
             };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedServiceResponse = new ServiceResponse<IEnumerable<OrderSummaryViewModel>>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderPlaced(id) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void OrderPlaced_WhenStatusCodeIsSuccess_ReturnsRedirectToAction_ServiceResponseIsNull()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var expectedResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedResponse);
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
                TempData = tempData,


            };

            //Act
            var actual = target.OrderPlaced(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void OrderPlaced_WhenStatusCodeIsSuccess_ReturnsRedirectToAction_DataIsNull()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<OrderSummaryViewModel>
            {
                Message = "",
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderPlaced(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void OrderPlaced_WhenStatusCodeIsSuccess_ReturnsRedirectToAction_SuccesIsFalse()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<OrderSummaryViewModel>
            {
                Message = "",
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderPlaced(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void OrderPlaced_ReturnsRedirectToAction_WhenFails()
        {
            //Arrange
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = errorMessage

            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderPlaced(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void OrderPlaced_ReturnsRedirectToAction_SomethingWentWrong()
        {
            //Arrange
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var errorMessage = "Something went wrong. Please try after sometime";
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new OrderController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.OrderPlaced(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<OrderSummaryViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }






    }
}
