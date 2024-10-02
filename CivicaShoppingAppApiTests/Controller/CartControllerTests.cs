using CivicaShoppingAppApi.Controllers;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Services.Contract;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppApiTests.Controller
{
    public class CartControllerTests
    {
        [Fact]
        public void GetCartItemsByUserId_returnsOk_WhenOrderExist()
        {
            //Arrange
            var expectedProductList = new List<UserCartDto>()
            {
               new UserCartDto  {
                  ProductId=1,
                ProductQuantity = 5
               },
              new UserCartDto  {
                 ProductId = 2,
                 ProductQuantity = 5    
              },

            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserCartDto>>()
            {
                Success = true,
                Data = expectedProductList
            };
            var mockCartService = new Mock<ICartService>();
            mockCartService.Setup(service => service.GetCartItemsByUserId(1)).Returns(expectedServiceResponse);

            var target = new CartController(mockCartService.Object);

            //Act
            var actual = target.GetCartItemsByUserId(1) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockCartService.Verify(c => c.GetCartItemsByUserId(1), Times.Once);
        }

        [Theory]
        [InlineData("No record found")]
        public void GetCartItemsByUserId_returnsbadRequest_WhenOrderNotExist(string errorMessage)
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<IEnumerable<UserCartDto>>()
            {
                Success = false,
                Message = errorMessage

            };
            var mockCartService = new Mock<ICartService>();
            mockCartService.Setup(service => service.GetCartItemsByUserId(1)).Returns(expectedServiceResponse);

            var target = new CartController(mockCartService.Object);

            //Act
            var actual = target.GetCartItemsByUserId(1) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockCartService.Verify(c => c.GetCartItemsByUserId(1), Times.Once);
        }

        [Fact]
        public void AddToCart_ReturnsOk_WhenProductAddedSuccessfully()
        {

            //Arrange
            var AddToCartDto = new AddToCartDto()
            {
                ProductId =1,
                ProductQuantity =1
            };

            var responseString = new ServiceResponse<string>
            {
                Success = true,
                Message = "Product added to cart."
            };

            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);
            mockCartService.Setup(c => c.AddToCart(It.IsAny<AddToCartDto>())).Returns(responseString);
            //Act
            var actual = target.AddToCart(AddToCartDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockCartService.Verify(c => c.AddToCart(It.IsAny<AddToCartDto>()), Times.Once);
        }

        [Theory]
        [InlineData("Cannot add more than 5 products of the same type")]
        [InlineData("Cannot add more than 10 items to cart")]
        public void AddProduct_ReturnsNotFound_WhenProductnotAdded(string errorMessage)
        {

            //Arrange
            var addToCart = new AddToCartDto()
            {
               ProductId    =1,
               ProductQuantity =1
            };


            var responseString = new ServiceResponse<string>
            {
                Success = false,
                /* Data = AddProductDto */// Convert to ProductDto
                Message = errorMessage
            };

            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);
            mockCartService.Setup(c => c.AddToCart(It.IsAny<AddToCartDto>())).Returns(responseString);
            //Act
            var actual = target.AddToCart(addToCart) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockCartService.Verify(c => c.AddToCart(It.IsAny<AddToCartDto>()), Times.Once);
        }

        [Fact]
        public void UpdateCart_ReturnsOk_WhenCartUpdatedSuccessfully()
        {

            //Arrange
            var updateCartDto = new UpdateCartDto()
            {
                ProductId = 1,
                ProductQuantity=1,
            };
            var responseString = new ServiceResponse<string>
            {
                Success = true,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = "Cart updated successfully."
            };

            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);
            mockCartService.Setup(c => c.UpdateCart(It.IsAny<UpdateCartDto>())).Returns(responseString);
            //Act
            var actual = target.UpdateCart(updateCartDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockCartService.Verify(c => c.UpdateCart(It.IsAny<UpdateCartDto>()), Times.Once);
        }



        [Theory]
        [InlineData("Cannot add more than 5 products of the same type")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void UpdateCart_ReturnsNotFound_WhenCartNotUpdatedSuccessfully(string errorMessage)
        {

            //Arrange
            var updateCartDto = new UpdateCartDto()
            {
                ProductId = 1,
                ProductQuantity = 1,
            };
            var responseString = new ServiceResponse<string>
            {
                Success = false,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = errorMessage
            };

            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);

            mockCartService.Setup(c => c.UpdateCart(It.IsAny<UpdateCartDto>())).Returns(responseString);
            //Act
            var actual = target.UpdateCart(updateCartDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockCartService.Verify(c => c.UpdateCart(It.IsAny<UpdateCartDto>()), Times.Once);
        }

        [Fact]
        public void RemoveAllItemsForUser_ReturnBadRequest_WhenItemsNotDeleted()
        {

            var responseString = new ServiceResponse<string>
            {
                Success = false,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = "Something went wrong, please try after sometime."
            };

            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);

            mockCartService.Setup(c => c.RemoveAllItemsForUser(1)).Returns(responseString);

            //Act
            var actual = target.RemoveAllItemsForUser(1) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockCartService.Verify(c => c.RemoveAllItemsForUser(1), Times.Once);

        }

        [Fact]
        public void RemoveAllItemsForUser_ReturnBadRequest_WhenUserIdISNotValid()
        {
            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);

            //Act
            var actual = target.RemoveAllItemsForUser(0) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);

        }

        [Fact]
        public void RemoveAllItemsForUser_Returnok_WhenItemsDeleted()
        {
            var responseString = new ServiceResponse<string>
            {
                Success = true,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = "Items deleted successfully."
            };

            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);

            mockCartService.Setup(c => c.RemoveAllItemsForUser(1)).Returns(responseString);

            //Act
            var actual = target.RemoveAllItemsForUser(1) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockCartService.Verify(c => c.RemoveAllItemsForUser(1), Times.Once);

        }

        [Fact]
        public void RemoveParticularProductFromCart_ReturnBadRequest_WhenItemsNotDeleted()
        {

            var responseString = new ServiceResponse<string>
            {
                Success = false,
                Message = "Something went wrong, please try after sometime."
            };

            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);

            mockCartService.Setup(c => c.RemoveParticularItemFromCart(1,1)).Returns(responseString);

            //Act
            var actual = target.RemoveParticularProductFromCart(1,1) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockCartService.Verify(c => c.RemoveParticularItemFromCart(1,1), Times.Once);

        }

        [Fact]
        public void RemoveParticularProductFromCart_ReturnBadRequest_WhenUserIdISNotValid()
        {
            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);

            //Act
            var actual = target.RemoveParticularProductFromCart(0,0) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);

        }

        [Fact]
        public void RemoveParticularProductFromCart_Returnok_WhenItemsDeleted()
        {
            var responseString = new ServiceResponse<string>
            {
                Success = true,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = "Items deleted successfully."
            };

            var mockCartService = new Mock<ICartService>();

            var target = new CartController(mockCartService.Object);

            mockCartService.Setup(c => c.RemoveParticularItemFromCart(1, 1)).Returns(responseString);

            //Act
            var actual = target.RemoveParticularProductFromCart(1, 1) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockCartService.Verify(c => c.RemoveParticularItemFromCart(1, 1), Times.Once);

        }


    }
}
