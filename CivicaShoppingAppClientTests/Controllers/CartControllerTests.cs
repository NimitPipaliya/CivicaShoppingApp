using CivicaShoppingAppClient.Controllers;
using CivicaShoppingAppClient.Infrastructure;
using CivicaShoppingAppClient.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;

namespace CivicaShoppingAppClientTests.Controllers
{
    public class CartControllerTests
    {
        // GetCartItemsByUserId()
        [Fact]
        public void GetCartItemsByUserId_ReturnsView_WhenCartItemsAreFound()
        {
            // Arrange
            int userId = 2;
            var cartItems = new List<UserCartViewModel>()
            {
                new UserCartViewModel
                {
                    CartId = 1,
                    UserId = userId,
                    ProductId = 1,
                    ProductQuantity = 5,
                    Product = new ProductListViewModel()
                    {
                        ProductId = 1,
                        ProductName = "Product 1",
                    },
                },
                new UserCartViewModel
                {
                    CartId = 2,
                    UserId = userId,
                    ProductId = 2,
                    ProductQuantity = 5,
                    Product = new ProductListViewModel()
                    {
                        ProductId = 2,
                        ProductName = "Product 2",
                    },
                },
            };

            var response = new ServiceResponse<IEnumerable<UserCartViewModel>>()
            {
                Data = cartItems,
                Success = true,
                Message = ""
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserCartViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.GetCartItemsByUserId(userId) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Model);
            Assert.Equal(cartItems, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserCartViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void GetCartItemsByUserId_ReturnsView_WhenCartItemsAreNotFound()
        {
            // Arrange
            int userId = 2;
            IEnumerable<UserCartViewModel> cartItems = new List<UserCartViewModel>();

            var response = new ServiceResponse<IEnumerable<UserCartViewModel>>()
            {
                Success = false,
                Message = ""
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserCartViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.GetCartItemsByUserId(userId) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual.Model);
            Assert.Equal(cartItems, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<IEnumerable<UserCartViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        // DeleteFromCart()
        [Fact]
        public void DeleteFromCart_SetsSuccessMessage_WhenResponseIsTrue()
        {
            // Arrange
            int userId = 1;
            int productId = 1;

            var response = new ServiceResponse<string>()
            {
                Success = true,
                Message = "Success",
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.DeleteFromCart(userId, productId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("GetCartItemsByUserId", actual.ActionName);
            Assert.Equal(userId, actual.RouteValues["userId"]);
            Assert.Equal(response.Message, target.TempData["successMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void DeleteFromCart_SetsErrorMessage_WhenResponseIsFalse()
        {
            // Arrange
            int userId = 1;
            int productId = 1;

            var response = new ServiceResponse<string>()
            {
                Success = false,
                Message = "Error occurred",
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);

            var mockTempDataProvider = new Mock<ITempDataProvider>();
            var tempData = new TempDataDictionary(new DefaultHttpContext(), mockTempDataProvider.Object);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                TempData = tempData,
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.DeleteFromCart(userId, productId) as RedirectToActionResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal("GetCartItemsByUserId", actual.ActionName);
            Assert.Equal(userId, actual.RouteValues["userId"]);
            Assert.Equal(response.Message, target.TempData["errorMessage"]);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(o => o.ExecuteApiRequest<ServiceResponse<string>>(It.IsAny<string>(), HttpMethod.Delete, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        // BuyProducts()
        [Fact]
        public void BuyProducts_ReturnsView_WhenProductsAreFound()
        {
            // Arrange
            int page = 1;
            int pageSize = 9;
            string sortDir = "asc";
            int userId = 1;

            var buyProducts = new List<BuyProductViewModel>()
            {
                new BuyProductViewModel()
                {
                    ProductId = 1,
                    ProductName = "Product 1",
                    ProductDescription = "Description 1",
                    isAddedToCart = false,
                },
                new BuyProductViewModel()
                {
                    ProductId = 2,
                    ProductName = "Product 2",
                    ProductDescription = "Description 2",
                    isAddedToCart = false,
                },
            };

            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel()
                {
                    CartId = 1,
                    ProductId = 1,
                    UserId = 1,
                    ProductQuantity = 1,
                },
                new CartItemViewModel()
                {
                    CartId = 2,
                    ProductId = 2,
                    UserId = 1,
                    ProductQuantity = 1,
                },
            };

            var countResponse = new ServiceResponse<int>()
            {
                Data = 10,
                Success = true,
            };

            var cartItemsResponse = new ServiceResponse<IEnumerable<CartItemViewModel>>()
            {
                Data = cartItems,
                Success = true,
            };

            var response = new ServiceResponse<IEnumerable<BuyProductViewModel>>()
            {
                Data = buyProducts,
                Success = true,
            };

            var claims = new List<Claim>
            {
                new Claim("UserId", "1")
            };
            var identity = new GenericIdentity("username");
            identity.AddClaims(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(claimsPrincipal),
            };

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CartItemViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(cartItemsResponse);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(countResponse);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.BuyProducts(page, pageSize, sortDir) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(buyProducts, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CartItemViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void BuyProducts_ReturnsView_WhenNoCartItemsAreFound()
        {
            // Arrange
            int page = 1;
            int pageSize = 9;
            string sortDir = "asc";
            int userId = 1;

            var buyProducts = new List<BuyProductViewModel>()
            {
                new BuyProductViewModel()
                {
                    ProductId = 1,
                    ProductName = "Product 1",
                    ProductDescription = "Description 1",
                    isAddedToCart = false,
                },
                new BuyProductViewModel()
                {
                    ProductId = 2,
                    ProductName = "Product 2",
                    ProductDescription = "Description 2",
                    isAddedToCart = false,
                },
            };

            var countResponse = new ServiceResponse<int>()
            {
                Data = 10,
                Success = true,
            };

            var cartItemsResponse = new ServiceResponse<IEnumerable<CartItemViewModel>>()
            {
                Success = false,
            };

            var response = new ServiceResponse<IEnumerable<BuyProductViewModel>>()
            {
                Data = buyProducts,
                Success = true,
            };

            var claims = new List<Claim>
            {
                new Claim("UserId", "1")
            };
            var identity = new GenericIdentity("username");
            identity.AddClaims(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(claimsPrincipal),
            };

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CartItemViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(cartItemsResponse);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(countResponse);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.BuyProducts(page, pageSize, sortDir) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(buyProducts, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CartItemViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void BuyProducts_ReturnsView_WhenUserIsNotLoggedIn()
        {
            // Arrange
            int page = 1;
            int pageSize = 9;
            string sortDir = "asc";
            int userId = 1;

            var buyProducts = new List<BuyProductViewModel>()
            {
                new BuyProductViewModel()
                {
                    ProductId = 1,
                    ProductName = "Product 1",
                    ProductDescription = "Description 1",
                    isAddedToCart = false,
                },
                new BuyProductViewModel()
                {
                    ProductId = 2,
                    ProductName = "Product 2",
                    ProductDescription = "Description 2",
                    isAddedToCart = false,
                },
            };

            var countResponse = new ServiceResponse<int>()
            {
                Data = 10,
                Success = true,
            };

            var response = new ServiceResponse<IEnumerable<BuyProductViewModel>>()
            {
                Data = buyProducts,
                Success = true,
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(countResponse);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.BuyProducts(page, pageSize, sortDir) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(buyProducts, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void BuyProducts_ReturnsViewWithEmptyList_WhenNoProductsAreFound()
        {
            // Arrange
            int page = 1;
            int pageSize = 9;
            string sortDir = "asc";
            int userId = 1;

            var buyProducts = new List<BuyProductViewModel>();
            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel()
                {
                    CartId = 1,
                    ProductId = 1,
                    UserId = 1,
                    ProductQuantity = 1,
                },
                new CartItemViewModel()
                {
                    CartId = 2,
                    ProductId = 2,
                    UserId = 1,
                    ProductQuantity = 1,
                },
            };

            var countResponse = new ServiceResponse<int>()
            {
                Data = 10,
                Success = true,
            };

            var cartItemsResponse = new ServiceResponse<IEnumerable<CartItemViewModel>>()
            {
                Data = cartItems,
                Success = true,
            };

            var response = new ServiceResponse<IEnumerable<BuyProductViewModel>>()
            {
                Success = false,
            };

            var claims = new List<Claim>
            {
                new Claim("UserId", "1")
            };
            var identity = new GenericIdentity("username");
            identity.AddClaims(claims);
            var claimsPrincipal = new ClaimsPrincipal(identity);

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext()
            {
                User = new ClaimsPrincipal(claimsPrincipal),
            };

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CartItemViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(cartItemsResponse);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(countResponse);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.BuyProducts(page, pageSize, sortDir) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(buyProducts, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<CartItemViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void BuyProducts_ReturnsViewWithEmptyList_WhenNoProductsWereFound_AndUserIsNotLoggedIn()
        {
            // Arrange
            int page = 1;
            int pageSize = 9;
            string sortDir = "asc";
            int userId = 1;

            var buyProducts = new List<BuyProductViewModel>();
            var cartItems = new List<CartItemViewModel>()
            {
                new CartItemViewModel()
                {
                    CartId = 1,
                    ProductId = 1,
                    UserId = 1,
                    ProductQuantity = 1,
                },
                new CartItemViewModel()
                {
                    CartId = 2,
                    ProductId = 2,
                    UserId = 1,
                    ProductQuantity = 1,
                },
            };

            var countResponse = new ServiceResponse<int>()
            {
                Data = 10,
                Success = true,
            };

            var cartItemsResponse = new ServiceResponse<IEnumerable<CartItemViewModel>>()
            {
                Data = cartItems,
                Success = true,
            };

            var response = new ServiceResponse<IEnumerable<BuyProductViewModel>>()
            {
                Success = false,
            };

            var mockHttpClientService = new Mock<IHttpClientService>();
            var mockConfiguration = new Mock<IConfiguration>();
            var httpContext = new DefaultHttpContext();

            mockConfiguration.Setup(c => c["EndPoint:CivicaApi"]).Returns("fakeEndpoint");
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(response);
            mockHttpClientService.Setup(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>())).Returns(countResponse);

            var target = new CartController(mockHttpClientService.Object, mockConfiguration.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = httpContext,
                }
            };

            // Act
            var actual = target.BuyProducts(page, pageSize, sortDir) as ViewResult;

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(buyProducts, actual.Model);
            mockConfiguration.Verify(c => c["EndPoint:CivicaApi"], Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
            mockHttpClientService.Verify(c => c.ExecuteApiRequest<ServiceResponse<int>>(It.IsAny<string>(), HttpMethod.Get, httpContext.Request, It.IsAny<object>(), It.IsAny<int>()), Times.Once);
        }
    }
}
