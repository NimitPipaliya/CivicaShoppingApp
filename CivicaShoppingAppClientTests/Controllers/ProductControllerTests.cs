using CivicaShoppingAppClient.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Moq;
using CivicaShoppingAppClient.Controllers;
using CivicaShoppingAppClient.ViewModels;
using AutoFixture;

namespace CivicaShoppingAppClientTests.Controllers
{
    public class ProductControllerTests
    {
        //-------------------------------------Index----------------
        [Fact]
        public void Index_ReturnsViewWithProducts_WhenResponseIsSuccessAndSearched()
        {
            //Arrange
            string searchString = "te";
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<ProductListViewModel>>();
            
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductListViewModel>>
            {
                Data = expectedProducts,
                Message = "",
                Success = true
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

          
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Index(searchString, page, pageSize, sort_dir) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(expectedProducts, actual.Model);
            
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void Index_ReturnsViewWithProduct_WhenResponseIsSuccess_SearchBarIsNull()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<ProductListViewModel>>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductListViewModel>>
            {
                Data = expectedProducts,
                Message = "",
                Success = true
            };

         
            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Index(searchString, page, pageSize, sort_dir) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(expectedProducts, actual.Model);

            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]
        public void Index_ReturnsView_EmptyProduct_WhenResponseIsSuccess()
        {
            //Arrange
            string searchString = null;
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<ProductListViewModel>>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductListViewModel>>
            {
                Data = null,
                Message = "",
                Success = false
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Index(searchString, page, pageSize, sort_dir) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductListViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }


        //-------------------------------------Create----------------
        [Fact]
        public void Create_ReturnsView()
        {
            //Arrange
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var mockHttpContext = new Mock<HttpContext>();
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                },
            };
            //Act
            var actual = target.Create() as ViewResult;
            //Assert
            Assert.NotNull(actual);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once());
        }

        [Fact]
        public void Create_ProductSavedSuccessfully_ReturnsViews()
        {
            //Arrange
            var viewModel = new AddProductViewModel()
            {
                ProductName = "test",
                ProductDescription = "test",
                ProductPrice = 123,
                Quantity = 1
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            string successMessage = null;
            var expectedServiceResponse = new ServiceResponse<AddProductViewModel>
            {
                Success = true,
                Data = viewModel,
            };

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.Create(viewModel) as ViewResult;
            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]
        public void Create_ReturnsSameViewPage_WhenServiceResponseIsNull_RedirectToAction()
        {
            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Create<AddProductViewModel>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var successMessage = "Product saved successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage
            };

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Create(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Create_ReturnsSameViewPage_WhenServiceResponseIsFalse_RedirectToAction()
        {
            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Create<AddProductViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();

            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act

            var actual = target.Create(viewModel) as RedirectToActionResult;

            //Assert

            Assert.NotNull(actual);

            Assert.True(target.ModelState.IsValid);

            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Create_ProductFailedToSave_ReturnRedirectToActionResult()

        {

            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Create<AddProductViewModel>();

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

            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };
            //Act
            var actual = target.Create(viewModel) as ViewResult;

            //Assert

            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(errorMessage, actual.TempData["ErrorMessage"]);

            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }
        [Fact]

        public void Create_ProductFailedToSave_ReturnSomethingwentWrong()
        {
            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Create<AddProductViewModel>();

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
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockDataProvider = new Mock<ITempDataProvider>();

            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };
            //Act
            var actual = target.Create(viewModel) as ViewResult;

            //Assert

            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Something went wrong, please try after sometime.", actual.TempData["ErrorMessage"]);

            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);

            mockHttpClientService.Verify(c => c.PostHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        //-------------------------------------Details----------------
        [Fact]
        public void Details_ReturnsDetailsView()
        {
            //Arrange
            var fixture = new Fixture();
            var Product = fixture.Create<ProductListViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var successMessage = "Details Shown";

            var expectedServiceReponse = new ServiceResponse<ProductListViewModel>()
            {
                Message = successMessage,

                Success = true,
                Data = Product
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceReponse)),
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {

                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(Product.ToString(), actual.Model.ToString());
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Details_RedirectToIndex_WhenResponseSuccessIsFalse()
        {
            //Arrange
            var fixture = new Fixture();
            var Product = fixture.Create<ProductListViewModel>();


            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var errorMessage = "something wrong";

            var expectedServiceReponse = new ServiceResponse<ProductListViewModel>()
            {
                Message = errorMessage,
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceReponse)),
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Details_RedirectToIndex_WhenResponseDataIsNull()
        {
            //Arrange
            var fixture = new Fixture();
            var Product = fixture.Create<ProductListViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var errorMessage = "something wrong";

            var expectedServiceReponse = new ServiceResponse<ProductListViewModel>()
            {
                Message = errorMessage,
                Data = null,
                Success = true
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceReponse)),
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Details_RedirectToIndex_WhenErrorResponseIsNotNull()
        {
            //Arrange
            var fixture = new Fixture();
            var Product = fixture.Create<ProductListViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var errorMessage = "something wrong,error response is null";

            var expectedServiceReponse = new ServiceResponse<ProductListViewModel>()
            {
                Message = errorMessage,
                Data = Product,
                Success = true
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceReponse)),
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Details_RedirectToIndex_WhenErrorResponseIsNull()
        {
            //Arrange
            var fixture = new Fixture();
            var Product = fixture.Create<ProductListViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var errorMessage = "Something went wrong please try after some time.";

            var expectedServiceReponse = new ServiceResponse<ProductListViewModel>()
            {
                Message = errorMessage,
                Data = Product,
                Success = true
            };


            var expectedReponse = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError)
            {
                Content = null
            };

            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockHttpContext = new Mock<HttpContext>();
            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object,
                }

            };

            //Act
            var actual = target.Details(1) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);

            Assert.Equal(errorMessage, target.TempData["errorMessage"]);
            Assert.Equal("Index", actual.ActionName);


            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }
        [Fact]
        public void Details_RedirectToAction_WhenProductNotExists()
        {
            // Arrange
            int categoryId = 1;
            var expectedSuccessResponseContent = new ServiceResponse<ProductListViewModel>
            {
                Success = false,
                Message = string.Empty,
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = null
            };
            var mockHttpContext = new Mock<HttpContext>();
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedSuccessResponse);
            var mockTepDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTepDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext { HttpContext = mockHttpContext.Object },
                TempData = tempData,
            };

            // Act
            var actual = target.Details(categoryId) as RedirectToActionResult;
            // Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<ProductListViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        //-------------------------------------Edit----------------
        [Fact]
        public void Edit_ReturnsView_WhenStatusCodeIsSuccess()
        {
            var id = 1;
            var fixture = new Fixture();
            var viewModel = fixture.Create<UpdateProductViewModel>();
           
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
        
            var mockHttpContext = new Mock<HttpContext>();

            var expectedServiceResponse = new ServiceResponse<UpdateProductViewModel>
            {
                Data = viewModel,
                Success = true
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);


            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Edit_RedirectToIndex_WhenStatusCodeIsSuccess_serviceResponseIsFalse()
        {
            var id = 1;
            var fixture = new Fixture();
            var Product = fixture.Create<UpdateProductViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
         

            var mockHttpContext = new Mock<HttpContext>();

            var expectedServiceResponse = new ServiceResponse<UpdateProductViewModel>
            {
                Success = false,
                Data = Product,
                Message  = "error"
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index",actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Edit_RedirectToIndex_WhenStatusCodeIsSuccess_serviceResponseIsNull()
        {
            var id = 1;
            var fixture = new Fixture();
            var Product = fixture.Create<UpdateProductViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");


            var mockHttpContext = new Mock<HttpContext>();

            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);
        }

        [Fact]
        public void Edit_RedirectToIndex_WhenStatusCodeIsSuccess_serviceResponseDataIsNull()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<UpdateProductViewModel>
            {
                Message = "",
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Edit_WhenErrorMessageIsNull_WhenStatusCodeIsNotSuccess()
        {
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedServiceResponse = new ServiceResponse<UpdateProductViewModel>
            {
                Message = null,
                Data = new UpdateProductViewModel
                {
                    ProductName = "test",
                    ProductDescription = "test",
                    ProductPrice = 123,
                    Quantity = 1
                },
                Success = false
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Edit_WhenErrorMessageNotNull_WhenStatusCodeIsNotSuccess()
        {
            //Arrange
            var id = 1;
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Something went wrong please try after some time.", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.GetHttpResponseMessage<UpdateProductViewModel>(It.IsAny<string>(), It.IsAny<HttpRequest>()), Times.Once);

        }

        //-------------------------------------Edit Post----------------
        [Fact]
        public void Edit_ProductSavedSuccessfully_RedirectToAction()
        {
            //Arrange
            var id = 1;
            var fixture = new Fixture();
            var viewModel = fixture.Create<UpdateProductViewModel>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var successMessage = "Product saved successfully";
            var expectedServiceResponse = new ServiceResponse<string>
            {
                Message = successMessage
            };
            var expectedReponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedServiceResponse))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
            //Act
            var actual = target.Edit(viewModel) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(successMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);

        }

        [Fact]
        public void Edit_ProductFailedToSave_WhenErrorResponseIsNotNull_ReturnRedirectToActionResult()
        {
            //Arrange
            var fixture = new Fixture();
            var viewModel = fixture.Create<UpdateProductViewModel>();

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
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
        

            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);


        }
        [Fact]
        public void Edit_ProductFailedToSave_ReturnSomethingWentWrong()
        {
            //Arrange
            var viewModel = new UpdateProductViewModel {ProductName = "test", ProductDescription = "test", ProductPrice = 123, Quantity = 1 };
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
                Content = new StringContent(JsonConvert.SerializeObject(null))
            };
            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>())).Returns(expectedReponse);
            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },

            };
       

            //Act
            var actual = target.Edit(viewModel) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.True(target.ModelState.IsValid);
            Assert.Equal("Something went wrong, please try after sometime.", target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Exactly(2));
            mockHttpClientService.Verify(c => c.PutHttpResponseMessage(It.IsAny<string>(), viewModel, It.IsAny<HttpRequest>()), Times.Once);


        }
        //-------------------------------------Delete----------------
        [Fact]
        public void Delete_ReturnsRedirectToAction_WhenProductDeletedSuccessfully()
        {
            var id = 1;
            var Product = new ProductListViewModel()
            {
                ProductName = "test",
                ProductDescription = "test",
                ProductPrice = 123,
                Quantity = 1,
                GstPercentage = 1,
                FinalPrice = 1,
                IsAddedToCart = true,
                ProductId = 1
            };
            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");
            var errorMessage = "Product Deleted Successfully";
            var expectedSuccessData = new ServiceResponse<string>
            {
                Success = true,
                Message = errorMessage
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedSuccessData))
            };

            var httpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedSuccessData);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext.Object,
                }
            };

            //Act
            var actual = target.Delete(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["SuccessMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        [Fact]

        public void Delete_ReturnsRedirectToAction_WhenProductisNotDeleted()
        {
            var id = 1;
            var Product = new ProductListViewModel()
            {
                ProductName = "test",
                ProductDescription = "test",
                ProductPrice = 123,
                Quantity = 1,
                GstPercentage = 1,
                FinalPrice = 1,
                IsAddedToCart = true,
                ProductId = 1
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("endPoint");
            var errorMessage = "Something went wrong, please try after sometime.";
            var expectedSuccessData = new ServiceResponse<string>
            {
                Success = false,
                Message = errorMessage
            };

            var expectedSuccessResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(JsonConvert.SerializeObject(expectedSuccessData))
            };

            var httpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedSuccessData);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);
            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext.Object,
                }
            };

            //Act

            var actual = target.Delete(id) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal(errorMessage, target.TempData["ErrorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, It.IsAny<HttpRequest>(), null, 60), Times.Once);

        }
        //-------------------------------------QuantityOfProducts----------------
        [Fact]
        public void QuantityOfProducts_ReturnsViewWithData_WhenResponseIsSuccess()
        {
            //Arrange
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<QuantityViewModel>>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<QuantityViewModel>>
            {
                Data = expectedProducts,
                Message = "",
                Success = true
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<QuantityViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);


            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.QuantityOfProducts(page, pageSize, sort_dir) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(expectedProducts, actual.Model);

            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<QuantityViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        
        [Fact]
        public void QuantityOfProducts_ReturnsRedirectToActionHomeIndex_WhenResponseIsNull()
        {
            //Arrange
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<QuantityViewModel>>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");
            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();

  
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);


            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.QuantityOfProducts(page, pageSize, sort_dir) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Home", actual.ControllerName);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        public void QuantityOfProducts_ReturnsView_EmptyProduct_WhenResponseIsNotSuccess()
        {
            //Arrange
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<QuantityViewModel>>();


            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<QuantityViewModel>>
            {
                Data = null,
                Message = "",
                Success = false
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<QuantityViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.QuantityOfProducts(page, pageSize, sort_dir) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<QuantityViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }

        //-------------------------------------ProductsSold----------------
        [Fact]
        public void ProductsSold_ReturnsViewWithData_WhenResponseIsSuccess()
        {
            //Arrange
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<ProductsSoldViewModel>>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductsSoldViewModel>>
            {
                Data = expectedProducts,
                Message = "",
                Success = true
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductsSoldViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);


            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.ProductsSold(page, pageSize, sort_dir) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(expectedProducts, actual.Model);

            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductsSoldViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        
        [Fact]
        public void ProductsSold_ReturnsRedirectToactionHomeIndex_WhenResponseIsNull()
        {
            //Arrange
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<ProductsSoldViewModel>>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);


            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.ProductsSold(page, pageSize, sort_dir) as RedirectToActionResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Index", actual.ActionName);
            Assert.Equal("Home", actual.ControllerName);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
        [Fact]
        public void ProductsSold_ReturnsView_EmptyProduct_WhenResponseIsNotSuccess()
        {
            //Arrange
            int page = 1;
            int pageSize = 6;
            string sort_dir = "asc";

            var fixture = new Fixture();
            var expectedProducts = fixture.Create<List<ProductsSoldViewModel>>();

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndPoint");

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductsSoldViewModel>>
            {
                Data = null,
                Message = "",
                Success = false
            };

            var expectedCount = new ServiceResponse<int>()
            {
                Data = expectedProducts.Count(),
                Success = true
            };

            var mockHttpContext = new Mock<HttpContext>();
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductsSoldViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), It.IsAny<Object>(), 60)).Returns(expectedServiceResponse);

            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60)).Returns(expectedCount);

            var mockDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockDataProvider.Object);

            var target = new ProductController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext
                {
                    HttpContext = mockHttpContext.Object
                },
            };

            //Act
            var actual = target.ProductsSold(page, pageSize, sort_dir) as ViewResult;

            //Assert
            Assert.NotNull(actual);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductsSoldViewModel>>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, It.IsAny<HttpRequest>(), null, 60), Times.Once);
        }
    }
}
