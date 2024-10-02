using CivicaShoppingAppApi.Controllers;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
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
    public class ProductControllerTests
    {
        //---------------All products with pagination
        [Fact]
        public void GetPaginatedProduct_returnsOk_WhenProductListExist()
        {
            //Arrange
            var expectedProductList = new List<ProductListDto>()
            {
               new ProductListDto  {
                  ProductId=1,
                ProductDescription = "test",
                ProductName = "test",
                Quantity = 10,
                ProductPrice = 100,
                GstPercentage = 18,
                finalPrice = 118

               },
              new ProductListDto  {
                  ProductId=2,
                ProductDescription = "test",
                ProductName = "test",
                Quantity = 10,
                ProductPrice = 123 ,
                GstPercentage = 18,
                finalPrice = 118
              },

            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductListDto>>()
            {
                Success = true,
                Data = expectedProductList
            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetPaginatedProducts(1, 5, "asc")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetAllProducts(1, 5, "asc") as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetPaginatedProducts(1, 5, "asc"), Times.Once);
        }

        [Theory]
        [InlineData("No record found")]
        public void GetPaginatedProducts_returnsbadRequest_WhenProductListExist(string errorMessage)
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductListDto>>()
            {
                Success = false,
                Message = errorMessage

            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetPaginatedProducts(1, 5, "asc")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetAllProducts(1, 5, "asc") as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetPaginatedProducts(1, 5, "asc"), Times.Once);
        }

        //--------------Total Product for pagination--------------
        [Fact]
        public void GetTotalProducts_returnsOk_WhenProductExist()
        {
            //Arrange
            var expectedServiceResponse = new ServiceResponse<int>()
            {
                Success = true,
                Data = 1
            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.TotalProduct()).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.TotalProducts() as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.TotalProduct(), Times.Once);
        }

        [Fact]
        public void GetTotalProducts_returnsnotFound_WhenContatNotExist()
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<int>()
            {
                Success = false,

            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.TotalProduct()).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.TotalProducts() as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.TotalProduct(), Times.Once);
        }
        //---------------Get produt by id-----------------
        [Fact]
        public void GetProductById_ReturnsOkWithProduct_WhenProductExists()
        {
            //Arrange

            var product = new ProductListDto()
            {
                ProductDescription = "test",
                ProductName = "test",
                Quantity = 10,
                ProductPrice = 123
            };

            var response = new ServiceResponse<ProductListDto>
            {
                Success = true,
                Data = product // Convert to ProductDto
            };

            var mockProductService = new Mock<IProductService>();
            var target = new ProductController(mockProductService.Object);
            mockProductService.Setup(c => c.GetProduct(1)).Returns(response);

            //Act
            var actual = target.GetProductById(1) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockProductService.Verify(c => c.GetProduct(1), Times.Once);
        }

        [Fact]
        public void GetProductById_ReturnsNotFoundWithProduct_WhenProductNotExists()
        {
            //Arrange
            var response = new ServiceResponse<ProductListDto>
            {
                Success = false,

            };

            var mockProductService = new Mock<IProductService>();
            var target = new ProductController(mockProductService.Object);
            mockProductService.Setup(c => c.GetProduct(1)).Returns(response);

            //Act
            var actual = target.GetProductById(1) as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(response, actual.Value);
            mockProductService.Verify(c => c.GetProduct(1), Times.Once);
        }

        //--------------Add Product--------------
        [Fact]
        public void AddProduct_ReturnsOk_WhenProductAddedSuccessfully()
        {

            //Arrange
            var AddProductDto = new AddProductDto()
            {
                ProductDescription = "test",
                ProductName = "test",
                Quantity = 10,
                ProductPrice = 123
            };

            var responseString = new ServiceResponse<string>
            {
                Success = true,
                Message = "Product added successfully."
            };

            var mockProductService = new Mock<IProductService>();

            var target = new ProductController(mockProductService.Object);
            mockProductService.Setup(c => c.AddProduct(It.IsAny<AddProductDto>())).Returns(responseString);
            //Act
            var actual = target.AddProduct(AddProductDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockProductService.Verify(c => c.AddProduct(It.IsAny<AddProductDto>()), Times.Once);
        }

        [Theory]
        [InlineData("Product already exists.")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void AddProduct_ReturnsNotFound_WhenProductnotAdded(string errorMessage)
        {

            //Arrange
            var AddProductDto = new AddProductDto()
            {
              ProductDescription= "test",
              ProductName="test",
              Quantity=10,
              ProductPrice=123
            };


            var responseString = new ServiceResponse<string>
            {
                Success = false,
                /* Data = AddProductDto */// Convert to ProductDto
                Message = errorMessage
            };

            var mockProductService = new Mock<IProductService>();

            var target = new ProductController(mockProductService.Object);
            mockProductService.Setup(c => c.AddProduct(It.IsAny<AddProductDto>())).Returns(responseString);
            //Act
            var actual = target.AddProduct(AddProductDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockProductService.Verify(c => c.AddProduct(It.IsAny<AddProductDto>()), Times.Once);
        }

        //--------------Update Product--------------
        [Fact]
        public void UpdateProduct_ReturnsOk_WhenProductUpdatedSuccessfully()
        {

            //Arrange
            var updateProductDto = new UpdateProductDto()
            {
                ProductId=1,
                ProductDescription = "test",
                ProductName = "test",
                Quantity = 10,
                ProductPrice = 123
            };
            var responseString = new ServiceResponse<string>
            {
                Success = true,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = "Product updated successfully."
            };

            var mockProductService = new Mock<IProductService>();

            var target = new ProductController(mockProductService.Object);
            mockProductService.Setup(c => c.ModifyProduct(It.IsAny<UpdateProductDto>())).Returns(responseString);
            //Act
            var actual = target.UpdateProduct(updateProductDto) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockProductService.Verify(c => c.ModifyProduct(It.IsAny<UpdateProductDto>()), Times.Once);
        }



        [Theory]
        [InlineData("Product already exists.")]
        [InlineData("Something went wrong, please try after sometime.")]
        public void UpdateProduct_ReturnsNotFound_WhenProductNotUpdatedSuccessfully(string errorMessage)
        {

            //Arrange
            var updateProductDto = new UpdateProductDto()
            {
                ProductId = 1,
                ProductDescription = "test",
                ProductName = "test",
                Quantity = 10,
                ProductPrice = 123
            };
            var responseString = new ServiceResponse<string>
            {
                Success = false,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = errorMessage
            };

            var mockProductService = new Mock<IProductService>();

            var target = new ProductController(mockProductService.Object);

            mockProductService.Setup(c => c.ModifyProduct(It.IsAny<UpdateProductDto>())).Returns(responseString);
            //Act
            var actual = target.UpdateProduct(updateProductDto) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockProductService.Verify(c => c.ModifyProduct(It.IsAny<UpdateProductDto>()), Times.Once);
        }


        //--------------Delete Product--------------
        [Fact]
        public void RemoveProduct_ReturnBadRequest_WhenProductNotDeleted()
        {

            var responseString = new ServiceResponse<string>
            {
                Success = false,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = "Something went wrong, please try after sometime."
            };

            var mockProductService = new Mock<IProductService>();

            var target = new ProductController(mockProductService.Object);

            mockProductService.Setup(c => c.RemoveProduct(1)).Returns(responseString);

            //Act
            var actual = target.DeleteProduct(1) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockProductService.Verify(c => c.RemoveProduct(1), Times.Once);

        }

        [Fact]
        public void RemoveProduct_ReturnBadRequest_WhenProperIdISNotValid()
        {
            var mockProductService = new Mock<IProductService>();

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.DeleteProduct(0) as BadRequestObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(400, actual.StatusCode);
            Assert.NotNull(actual.Value);

        }

        [Fact]
        public void RemoveProduct_Returnok_WhenProductDeleted()
        {
            var responseString = new ServiceResponse<string>
            {
                Success = true,
                /*Data = AddProductDto // Convert to ProductDto*/
                Message = "Product deleted successfully."
            };

            var mockProductService = new Mock<IProductService>();

            var target = new ProductController(mockProductService.Object);

            mockProductService.Setup(c => c.RemoveProduct(1)).Returns(responseString);

            //Act
            var actual = target.DeleteProduct(1) as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(responseString, actual.Value);
            mockProductService.Verify(c => c.RemoveProduct(1), Times.Once);

        }
        //-------------Searched Product-------------------
        [Fact]
        public void GetPaginatedProductsWithChar_returnsOk_WhenProductListExist()
        {
            //Arrange
            var expectedProductList = new List<ProductListDto>()
            {
               new ProductListDto  {
                  ProductId=1,
                ProductDescription = "test",
                ProductName = "test",
                Quantity = 10,
                ProductPrice = 100,
                GstPercentage = 18,
                finalPrice = 118
               
               },
              new ProductListDto  {   
                  ProductId=2,
                ProductDescription = "test",
                ProductName = "test",
                Quantity = 10,
                ProductPrice = 123 ,
                GstPercentage = 18,
                finalPrice = 118
              },

            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductListDto>>()
            {
                Success = true,
                Data = expectedProductList
            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetPaginatedProductsWithSearch("x", 1, 5, "asc")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetAllSearchedProducts("x", 1, 5, "asc") as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetPaginatedProductsWithSearch("x", 1, 5, "asc"), Times.Once);
        }

        [Theory]
        [InlineData("No record found")]
        public void GetPaginatedProductsWithChar_returnsbadRequest_WhenProductListExist(string errorMessage)
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductListDto>>()
            {
                Success = false,
                Message = errorMessage

            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetPaginatedProductsWithSearch("x", 1, 5, "asc")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetAllSearchedProducts("x", 1, 5, "asc") as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetPaginatedProductsWithSearch("x", 1, 5, "asc"), Times.Once);
        }

        //--------------Total Searched Product for pagination--------------
        [Fact]
        public void TotalSearchedProducts_returnsOk_WhenProductExist()
        {
            //Arrange
            var expectedServiceResponse = new ServiceResponse<int>()
            {
                Success = true,
                Data = 1
            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.TotalProductStartingWithString("s")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.TotalSearchedProducts("s") as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.TotalProductStartingWithString("s"), Times.Once);
        }

        [Fact]
        public void TotalSearchedProducts_returnsnotFound_WhenContatNotExist()
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<int>()
            {
                Success = false,

            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.TotalProductStartingWithString("s")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.TotalSearchedProducts("s") as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.TotalProductStartingWithString("s"), Times.Once);
        }

        //---------------All products with pagination
        [Fact]
        public void GetQuantityOfSpecificProduct_returnsOk_WhenProductListExist()
        {
            //Arrange
            var expectedProductList = new List<ProductQuantityDto>()
            {
               new ProductQuantityDto  {
                  ProductId=1,
                ProductName = "test",
                Quantity = 10,
               },
              new ProductQuantityDto  {
                  ProductId=2,
                ProductName = "test",
                Quantity = 10,
              },

            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductQuantityDto>>()
            {
                Success = true,
                Data = expectedProductList
            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetQuantityOfSpecificProduct(1, 5, "asc")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetQuantityOfSpecificProduct(1, 5, "asc") as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetQuantityOfSpecificProduct(1, 5, "asc"), Times.Once);
        }

        [Theory]
        [InlineData("No record found")]
        public void GetQuantityOfSpecificProduct_returnsbadRequest_WhenProductListExist(string errorMessage)
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductQuantityDto>>()
            {
                Success = false,
                Message = errorMessage

            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetQuantityOfSpecificProduct(1, 5, "asc")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetQuantityOfSpecificProduct(1, 5, "asc") as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetQuantityOfSpecificProduct(1, 5, "asc"), Times.Once);
        }

        [Fact]
        public void GetProductSalesReport_returnsOk_WhenProductListExist()
        {
            //Arrange
            var expectedProductList = new List<ProductSaleReportDto>()
            {
               new ProductSaleReportDto  {
                  ProductId=1,
                ProductName = "test",
               },
              new ProductSaleReportDto  {
                  ProductId=2,
                ProductName = "test",
              },

            };

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductSaleReportDto>>()
            {
                Success = true,
                Data = expectedProductList
            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetProductSalesReport(1, 5, "asc")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetProductSalesReport(1, 5, "asc") as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetProductSalesReport(1, 5, "asc"), Times.Once);
        }

        [Theory]
        [InlineData("No record found")]
        public void GetProductSalesReport_returnsbadRequest_WhenProductListExist(string errorMessage)
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<IEnumerable<ProductSaleReportDto>>()
            {
                Success = false,
                Message = errorMessage

            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.GetProductSalesReport(1, 5, "asc")).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetProductSalesReport(1, 5, "asc") as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.GetProductSalesReport(1, 5, "asc"), Times.Once);
        }

        [Fact]
        public void GetTotalCountOfUsers_returnsOk_WhenProductExist()
        {
            //Arrange
            var expectedServiceResponse = new ServiceResponse<int>()
            {
                Success = true,
                Data = 1
            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.ProductsSoldCount()).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetTotalCountOfUsers() as OkObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(200, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.ProductsSoldCount(), Times.Once);
        }

        [Fact]
        public void GetTotalCountOfUsers_returnsnotFound_WhenContatNotExist()
        {
            //Arrange

            var expectedServiceResponse = new ServiceResponse<int>()
            {
                Success = false,

            };
            var mockProductService = new Mock<IProductService>();
            mockProductService.Setup(service => service.ProductsSoldCount()).Returns(expectedServiceResponse);

            var target = new ProductController(mockProductService.Object);

            //Act
            var actual = target.GetTotalCountOfUsers() as NotFoundObjectResult;

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(404, actual.StatusCode);
            Assert.NotNull(actual.Value);
            Assert.Equal(expectedServiceResponse, actual.Value);
            mockProductService.Verify(c => c.ProductsSoldCount(), Times.Once);
        }
    }
}
