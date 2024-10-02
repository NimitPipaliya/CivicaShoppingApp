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
    public class OrderControllerTests
    {
        [Fact]
        public void GetOrderByOrderNumber_returnsOk_WhenOrderExist()
        {
            //Arrange
            var expectedProductList = new List<OrderDto>()
            {
               new OrderDto  {
                  ProductId=1,
               OrderAmount = 500

               },
              new OrderDto  {
                  ProductId=2,
                OrderAmount = 50
              },

            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<OrderDto>>()
            {
                Success = true,
                Data = expectedProductList
            };
            var mockProductService = new Mock<IOrderService>();
            mockProductService.Setup(service => service.GetOrderByOrderNumber(1)).Returns(expectedServiceResponse);

            var target = new OrderController(mockProductService.Object);

            //Act
            var actual = target.GetOrderByOrderNumber(1) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetOrderByOrderNumber(1), Times.Once);
        }

        [Theory]
        [InlineData("No record found")]
        public void GetOrderByOrderNumber_returnsbadRequest_WhenOrderNotExist(string errorMessage)
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<IEnumerable<OrderDto>>()
            {
                Success = false,
                Message = errorMessage

            };
            var mockProductService = new Mock<IOrderService>();
            mockProductService.Setup(service => service.GetOrderByOrderNumber(1)).Returns(expectedServiceResponse);

            var target = new OrderController(mockProductService.Object);

            //Act
            var actual = target.GetOrderByOrderNumber(1) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetOrderByOrderNumber(1), Times.Once);
        }


        [Fact]
        public void GetAllOrdersByUserId_returnsOk_WhenOrderExist()
        {
            //Arrange
            var expectedProductList = new List<OrderListDto>()
            {
               new OrderListDto  {
                 OrderNumber = 1,

               },
              new OrderListDto  {
                 OrderNumber = 2
              },

            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<OrderListDto>>()
            {
                Success = true,
                Data = expectedProductList
            };
            var mockProductService = new Mock<IOrderService>();
            mockProductService.Setup(service => service.GetAllOrdersByUserId(1,1,1,"asc")).Returns(expectedServiceResponse);

            var target = new OrderController(mockProductService.Object);

            //Act
            var actual = target.GetAllOrdersByUserId(1,1,1,"asc") as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetAllOrdersByUserId(1,1,1,"asc"), Times.Once);
        }

        [Theory]
        [InlineData("No record found")]
        public void GetAllOrdersByUserId_returnsbadRequest_WhenOrderNotExist(string errorMessage)
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<IEnumerable<OrderListDto>>()
            {
                Success = false,
                Message = errorMessage

            };
            var mockProductService = new Mock<IOrderService>();
            mockProductService.Setup(service => service.GetAllOrdersByUserId(1, 1, 1, "asc")).Returns(expectedServiceResponse);

            var target = new OrderController(mockProductService.Object);

            //Act
            var actual = target.GetAllOrdersByUserId(1, 1, 1, "asc") as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetAllOrdersByUserId(1, 1, 1, "asc"), Times.Once);
        }

        [Fact]
        public void TotalOrderByUserID_returnsOk_WhenOrderExist()
        {
            //Arrange
            var expectedServiceResponse = new ServiceResponse<int>()
            {
                Success = true,
                Data = 1
            };
            var mockProductService = new Mock<IOrderService>();
            mockProductService.Setup(service => service.TotalOrderByUser(1)).Returns(expectedServiceResponse);

            var target = new OrderController(mockProductService.Object);

            //Act
            var actual = target.TotalOrderByUserId(1) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.TotalOrderByUser(1), Times.Once);
        }

        [Fact]
        public void TotalOrderByUserId_returnsnotFound_WhenOrderNotExist()
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<int>()
            {
                Success = false,

            };
            var mockProductService = new Mock<IOrderService>();
            mockProductService.Setup(service => service.TotalOrderByUser(1)).Returns(expectedServiceResponse);

            var target = new OrderController(mockProductService.Object);

            //Act
            var actual = target.TotalOrderByUserId(1) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.TotalOrderByUser(1), Times.Once);
        }

        [Fact]
        public void PlaceOrder_ReturnsOk_WhenProductAddedSuccessfully()
        {

            //Arrange

            int userId = 1;
            var responseString = new ServiceResponse<string>
            {
                Success = true,
                Message = "Product added successfully."
            };

            var mockProductService = new Mock<IOrderService>();

            var target = new OrderController(mockProductService.Object);
            mockProductService.Setup(c => c.PlaceOrder(1)).Returns(responseString);
            //Act
            var actual = target.PlaceOrder(userId) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockProductService.Verify(c => c.PlaceOrder(userId), Times.Once);
        }

        [Fact]
        public void PlaceOrder_ReturnsNotFound_WhenProductnotAdded()
        {

            //Arrange


            int userId = 1;
            var responseString = new ServiceResponse<string>
            {
                Success = false,
               
            };

            var mockProductService = new Mock<IOrderService>();

            var target = new OrderController(mockProductService.Object);
            mockProductService.Setup(c => c.PlaceOrder(1)).Returns(responseString);
            //Act
            var actual = target.PlaceOrder(userId) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockProductService.Verify(c => c.PlaceOrder(userId), Times.Once);
        }
    }
}
