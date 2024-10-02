using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppApiTests.Services
{
    public class ProductServiceTests
    {
        //------------------Get all Products with pagination----------------
        
        [Fact]
        public void GetAllPaginatedProducts_returnProduccts_WhenProductExist()
        {

            var expectedProducctList = new List<Product>() {
            new Product {
                ProductId=1,
                ProductDescription="Test 2",
                ProductName="Test",
                Quantity=1,
                ProductPrice =1,
                finalPrice=1,
                   GstPercentage=1,
            },
                     new Product {
                ProductId=2,
                ProductDescription="Test",
                ProductName="Test",
                Quantity=1,
                ProductPrice =1,
                finalPrice=1,
                   GstPercentage=1,
            }
            };

            var productList = new List<ProductListDto>() {
            new ProductListDto {
                  ProductId=1,
                ProductDescription="Test",
                ProductName="Test",
                Quantity=1,
                ProductPrice =1,
                finalPrice=1,
                   GstPercentage=1,
            },
                        new ProductListDto {
                  ProductId=2,
                ProductDescription="Test",
                ProductName="Test",
                Quantity=1,
                ProductPrice =1,
                finalPrice=1,
                   GstPercentage=1,
            },
            };

            var serviceResponse = new ServiceResponse<IEnumerable<ProductListDto>>()
            {
                Success = true,
                Data = productList
            };

            var mockProductRepository = new Mock<IProductRepository>();

            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.GetPaginatedProducts(1, 2, "asc")).Returns(expectedProducctList);

            //Act
            var actual = target.GetPaginatedProducts(1, 2, "asc");

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(productList.Count(), actual.Data.Count());
            Assert.Equal(serviceResponse.Data.ToString(), actual.Data.ToString());
            mockProductRepository.Verify(c => c.GetPaginatedProducts(1, 2, "asc"), Times.Once);
        }

        [Fact]
        public void GetAllPaginatedProducts_returnEmptyList_WhenProductNotExist()
        {

            var expectedProducctList = new List<Product>() {};

            var productList = new List<ProductListDto>() {};

            var serviceResponse = new ServiceResponse<IEnumerable<ProductListDto>>()
            {
                Success = false,
                Data = productList,
                Message = "No record found"
            };

            var mockProductRepository = new Mock<IProductRepository>();

            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.GetPaginatedProducts(1, 2, "asc")).Returns<Product>(null);

            //Act
            var actual = target.GetPaginatedProducts(1, 2, "asc");

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.Message, actual.Message);
            mockProductRepository.Verify(c => c.GetPaginatedProducts(1, 2, "asc"), Times.Once);
        }

        [Fact]
        public void TotalProducts_ReturnsSuccess_WhenProductsExist()
        {
            //Arrange
            var mockProductRepository = new Mock<IProductRepository>();

            mockProductRepository.Setup(c => c.TotalProducts()).Returns(2);
            var target = new ProductService(mockProductRepository.Object);
            var serviceResponse = new ServiceResponse<int>()
            {
                Success = true,
                Data = 2,

            };

            //Act
            var actual = target.TotalProduct();

            //Assert
            Assert.Equal(serviceResponse.Data, actual.Data);
            mockProductRepository.Verify(c => c.TotalProducts(), Times.Once);
        }


        //------------------Get product by id----------------
        [Fact]
        public void GetProduct_ReturnsProductById_WhenProductExists()
        {
            var Product = new Product
            {
                ProductId = 1,
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1,
                finalPrice = 1,
                GstPercentage = 1,
            };
            var ProductDto = new ProductListDto
            {
                ProductId = 1,
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1,
                finalPrice = 1,
                GstPercentage = 1,
            };
            var response = new ServiceResponse<ProductListDto>
            {
                Success = true,
                Data = ProductDto
            };
            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(c => c.GetProductById(1)).Returns(Product);
            var target = new ProductService(mockProductRepository.Object);

            // Act 
            var actual = target.GetProduct(1);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(ProductDto.ToString(), actual.Data.ToString());
            mockProductRepository.Verify(c => c.GetProductById(1), Times.Once);
        }

        [Fact]
        public void GetProduct_ReturnsErrorMeesssag_WhenProductisNull()
        {
            var ProductDto = new ProductListDto
            {
                ProductId = 1,
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1,
                finalPrice = 1,
                GstPercentage = 1,
            };
            var response = new ServiceResponse<ProductListDto>
            {
                Success = false,
                Message = "No record found!"
            };
            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(c => c.GetProductById(1)).Returns<Product>(null);
            var target = new ProductService(mockProductRepository.Object);

            // Act 
            var actual = target.GetProduct(1);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(response.Message, actual.Message);
            mockProductRepository.Verify(c => c.GetProductById(1), Times.Once);
        }

        //------------------Add Product----------------
        [Fact]
        public void AddProduct_ReturnSuccess_WhenProductAddedSuccessfully()
        {
            var product = new Product
            {
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1
            };
            var addProductDto = new AddProductDto
            {
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1
            };

            var serviceResponse = new ServiceResponse<string>()
            {
                Success = true,
                Message = "Product added successfully."
            };

            var mockProductRepository = new Mock<IProductRepository>();


            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.ProductExists(addProductDto.ProductName)).Returns(false);

            mockProductRepository.Setup(c => c.InsertProduct(It.IsAny<Product>())).Returns(true);

            //Act

            var actual = target.AddProduct(addProductDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.Message, actual.Message);

            mockProductRepository.Verify(c => c.ProductExists(product.ProductName), Times.Once);

            mockProductRepository.Verify(c => c.InsertProduct(product), Times.Never);


        }

        [Fact]
        public void AddProduct_ReturnError_WhenProductNotAddedSuccessfully()
        {
            var product = new Product
            {
                ProductId = 1,
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1,
                finalPrice = 1,
                GstPercentage = 1,
            };
            var addProductDto = new AddProductDto
            {
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1
            };

            var serviceResponse = new ServiceResponse<string>()
            {
                Success = false,
                Message = "Something went wrong, please try after sometime."
            };

            var mockProductRepository = new Mock<IProductRepository>();

            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.ProductExists(product.ProductName)).Returns(false);

            mockProductRepository.Setup(c => c.InsertProduct(It.IsAny<Product>())).Returns(false);

            //Act

            var actual = target.AddProduct(addProductDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.ToString(), actual.ToString());

            mockProductRepository.Verify(c => c.ProductExists(product.ProductName), Times.Once);

            mockProductRepository.Verify(c => c.InsertProduct(product), Times.Never);


        }

        [Fact]
        public void AddProduct_ReturnError_WhenProductAlreadyExists()
        {
            var product = new Product
            {
                ProductId = 1,
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1,
                finalPrice = 1,
                GstPercentage = 1,
            };
            var addProductDto = new AddProductDto
            {
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1
            };

            var serviceResponse = new ServiceResponse<string>()
            {
                Success = false,
                Message = "Product already exists."
            };

            var mockProductRepository = new Mock<IProductRepository>();
            var target = new ProductService(mockProductRepository.Object);


            mockProductRepository.Setup(c => c.ProductExists(product.ProductName)).Returns(true);

            //Act

            var actual = target.AddProduct(addProductDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.ToString(), actual.ToString());

            mockProductRepository.Verify(c => c.ProductExists(product.ProductName), Times.Once);


        }

        //------------------Update Product----------------
        [Fact]
        public void ModifyProduct_ReturnSuccess_WhenProductAddedSuccessfully()
        {
            var existingProduct = new Product
            {
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1
            };
            var updateProductDto = new UpdateProductDto
            {
                ProductDescription = "Test 3",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1,
                ProductId = 1
            };

            var serviceResponse = new ServiceResponse<string>()
            {
                Success = true,
                Message = "Product updated successfully."
            };

            var mockProductRepository = new Mock<IProductRepository>();


            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.ProductExists(updateProductDto.ProductId, updateProductDto.ProductName)).Returns(false);

            mockProductRepository.Setup(c => c.GetProductById(updateProductDto.ProductId)).Returns(existingProduct);

            mockProductRepository.Setup(c => c.UpdateProduct(It.IsAny<Product>())).Returns(true);

            //Act

            var actual = target.ModifyProduct(updateProductDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.Message, actual.Message);

            mockProductRepository.Verify(c => c.ProductExists(updateProductDto.ProductId,updateProductDto.ProductName), Times.Once);

            mockProductRepository.Verify(c => c.UpdateProduct(It.IsAny<Product>()), Times.Once);
            mockProductRepository.Verify(c => c.GetProductById(updateProductDto.ProductId), Times.Once);

        }


        [Fact]
        public void ModifyProduct_ReturnError_WhenProductNotupdated()
        {
            var existingProduct = new Product
            {
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1
            };
            var updateProductDto = new UpdateProductDto
            {
                ProductDescription = "Test 3",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1,
                ProductId = 1
            };

            var serviceResponse = new ServiceResponse<string>()
            {
                Success = false,
                Message = "Something went wrong, please try after sometime."
            };

            var mockProductRepository = new Mock<IProductRepository>();


            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.ProductExists(updateProductDto.ProductId, updateProductDto.ProductName)).Returns(false);

            mockProductRepository.Setup(c => c.GetProductById(updateProductDto.ProductId)).Returns(existingProduct);

            mockProductRepository.Setup(c => c.UpdateProduct(It.IsAny<Product>())).Returns(false);

            //Act

            var actual = target.ModifyProduct(updateProductDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.Message, actual.Message);

            mockProductRepository.Verify(c => c.ProductExists(updateProductDto.ProductId, updateProductDto.ProductName), Times.Once);

            mockProductRepository.Verify(c => c.UpdateProduct(It.IsAny<Product>()), Times.Once);
            mockProductRepository.Verify(c => c.GetProductById(updateProductDto.ProductId), Times.Once);

        }

        [Fact]
        public void ModifyProduct_ReturnError_WhenProductAlreadyExists()
        {
            var product = new Product
            {
                ProductId = 1,
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1,
                finalPrice = 1,
                GstPercentage = 1,
            };
            var updateProductDto = new UpdateProductDto
            {
                ProductId  = 1,
                ProductDescription = "Test 2",
                ProductName = "Test",
                Quantity = 1,
                ProductPrice = 1
            };

            var serviceResponse = new ServiceResponse<string>()
            {
                Success = false,
                Message = "Product already exists."
            };

            var mockProductRepository = new Mock<IProductRepository>();
            var target = new ProductService(mockProductRepository.Object);


            mockProductRepository.Setup(c => c.ProductExists(updateProductDto.ProductId,updateProductDto.ProductName)).Returns(true);

            //Act

            var actual = target.ModifyProduct(updateProductDto);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.Message, actual.Message);

            mockProductRepository.Verify(c => c.ProductExists(updateProductDto.ProductId,updateProductDto.ProductName), Times.Once);


        }
        //------------------Delete Product----------------

        [Fact]
        public void RemoveProduct_ReturnsResponse_WhenProductDeletedSuccessfully()
        {
            //Arrange
            var ProductId = 1;
            var mockProductrepository = new Mock<IProductRepository>();
            mockProductrepository.Setup(c => c.DeleteProduct(ProductId)).Returns(true);
            var target = new ProductService(mockProductrepository.Object);
            //Act
            var actual = target.RemoveProduct(ProductId);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Product deleted successfully.", actual.Message);
            mockProductrepository.Verify(c => c.DeleteProduct(ProductId), Times.Once);
        }
        [Fact]
        public void RemoveProduct_ReturnsResponse_WhenProductDeletedFails()
        {
            //Arrange
            var ProductId = 1;
            var mockProductrepository = new Mock<IProductRepository>();
            mockProductrepository.Setup(c => c.DeleteProduct(ProductId)).Returns(false);
            var target = new ProductService(mockProductrepository.Object);
            //Act
            var actual = target.RemoveProduct(ProductId);

            //Assert
            Assert.NotNull(actual);
            Assert.Equal("Something went wrong, please try after sometime.", actual.Message);
            mockProductrepository.Verify(c => c.DeleteProduct(ProductId), Times.Once);
        }

        //---------------------------Searching and sorting---------------------

        [Fact]
        public void GetPaginatedProductsWithSearch_returnProduccts_WhenProductExist()
        {

            var expectedProducctList = new List<Product>() {
            new Product {
                ProductId=1,
                ProductDescription="Test 2",
                ProductName="Test",
                Quantity=1,
                ProductPrice =1,
                finalPrice=1,
                   GstPercentage=1,
            },
                     new Product {
                ProductId=2,
                ProductDescription="Test",
                ProductName="Test",
                Quantity=1,
                ProductPrice =1,
                finalPrice=1,
                   GstPercentage=1,
            }
            };

            var productList = new List<ProductListDto>() {
            new ProductListDto {
                  ProductId=1,
                ProductDescription="Test",
                ProductName="Test",
                Quantity=1,
                ProductPrice =1,
                finalPrice=1,
                   GstPercentage=1,
            },
                        new ProductListDto {
                  ProductId=2,
                ProductDescription="Test",
                ProductName="Test",
                Quantity=1,
                ProductPrice =1,
                finalPrice=1,
                   GstPercentage=1,
            },
            };

            var serviceResponse = new ServiceResponse<IEnumerable<ProductListDto>>()
            {
                Success = true,
                Data = productList
            };

            var mockProductRepository = new Mock<IProductRepository>();

            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.GetPaginatedProductsWithFilter("s",1, 2, "asc")).Returns(expectedProducctList);

            //Act
            var actual = target.GetPaginatedProductsWithSearch("s",1, 2, "asc");

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(productList.Count(), actual.Data.Count());
            Assert.Equal(serviceResponse.Data.ToString(), actual.Data.ToString());
            mockProductRepository.Verify(c => c.GetPaginatedProductsWithFilter("s",1, 2, "asc"), Times.Once);
        }

        [Fact]
        public void GetPaginatedProductsWithSearch_returnEmptyList_WhenProductNotExist()
        {

            var expectedProducctList = new List<Product>() { };

            var productList = new List<ProductListDto>() { };

            var serviceResponse = new ServiceResponse<IEnumerable<ProductListDto>>()
            {
                Success = false,
                Data = productList,
                Message = "No record found"
            };

            var mockProductRepository = new Mock<IProductRepository>();

            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.GetPaginatedProductsWithFilter("s",1, 2, "asc")).Returns<Product>(null);

            //Act
            var actual = target.GetPaginatedProductsWithSearch("s",1, 2, "asc");

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.Message, actual.Message);
            mockProductRepository.Verify(c => c.GetPaginatedProductsWithFilter("s",1, 2, "asc"), Times.Once);
        }
        [Fact]
        public void TotalSearchedProducts_ReturnsSuccess_WhenProductsExist()
        {
            //Arrange
            var mockProductRepository = new Mock<IProductRepository>();

            mockProductRepository.Setup(c => c.TotalProductsStartingWithLetter("s")).Returns(2);
            var target = new ProductService(mockProductRepository.Object);
            var serviceResponse = new ServiceResponse<int>()
            {
                Success = true,
                Data = 2,

            };

            //Act
            var actual = target.TotalProductStartingWithString("s");

            //Assert
            Assert.Equal(serviceResponse.Data, actual.Data);
            mockProductRepository.Verify(c => c.TotalProductsStartingWithLetter("s"), Times.Once);
        }
        //------------------Get all Products with pagination----------------

        [Fact]
        public void GetQuantityOfSpecificProduct_returnProduccts_WhenProductExist()
        {

            var expectedProducctList = new List<Product>() {
            new Product {
                ProductId=1,
                ProductName="Test",
                Quantity=1,
            },
                     new Product {
                ProductId=2,
                ProductName="Test",
                Quantity=1,
             
            }
            };

            var productList = new List<ProductQuantityDto>() {
            new ProductQuantityDto {
                  ProductId=1,
                ProductName="Test",
                Quantity=1,
            },
                        new ProductQuantityDto {
                  ProductId=2,
                ProductName="Test",
                Quantity=1,
            },
            };

            var serviceResponse = new ServiceResponse<IEnumerable<ProductQuantityDto>>()
            {
                Success = true,
                Data = productList
            };

            var mockProductRepository = new Mock<IProductRepository>();

            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.GetQuantityOfSpecificProducts(1, 2, "asc")).Returns(expectedProducctList);

            //Act
            var actual = target.GetQuantityOfSpecificProduct(1, 2, "asc");

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(productList.Count(), actual.Data.Count());
            Assert.Equal(serviceResponse.Data.ToString(), actual.Data.ToString());
            mockProductRepository.Verify(c => c.GetQuantityOfSpecificProducts(1, 2, "asc"), Times.Once);
        }

        [Fact]
        public void GetQuantityOfSpecificProduct_returnEmptyList_WhenProductNotExist()
        {

            var expectedProducctList = new List<Product>() { };

            var productList = new List<ProductQuantityDto>() { };

            var serviceResponse = new ServiceResponse<IEnumerable<ProductQuantityDto>>()
            {
                Success = false,
                Data = productList,
                Message = "No record found"
            };

            var mockProductRepository = new Mock<IProductRepository>();

            var target = new ProductService(mockProductRepository.Object);

            mockProductRepository.Setup(c => c.GetQuantityOfSpecificProducts(1, 2, "asc")).Returns<Product>(null);

            //Act
            var actual = target.GetQuantityOfSpecificProduct(1, 2, "asc");

            //Assert
            Assert.NotNull(actual);
            Assert.Equal(serviceResponse.Message, actual.Message);
            mockProductRepository.Verify(c => c.GetQuantityOfSpecificProducts(1, 2, "asc"), Times.Once);
        }

        [Fact]
        public void GetProductSalesReport_ReturnsData_Successfully()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "asc";

            var productSales = new List<ProductSaleReportDto>
    {
        new ProductSaleReportDto { ProductId = 101, OrderDate = new DateTime(2023, 1, 15), TotalQuantitySold = 5, ProductName = "Product A" },
        new ProductSaleReportDto { ProductId = 102, OrderDate = new DateTime(2023, 1, 10), TotalQuantitySold = 3, ProductName = "Product B" }
    };

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductSalesReport(page, pageSize, sortOrder)).Returns(productSales);

            var service = new ProductService(mockProductRepository.Object);

            // Act
            var result = service.GetProductSalesReport(page, pageSize, sortOrder);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal("Product A", result.Data.First().ProductName);
            mockProductRepository.Verify(r => r.GetProductSalesReport(page, pageSize, sortOrder), Times.Once);
        }

        [Fact]
        public void GetProductSalesReport_ReturnsNoRecordFound_WhenNoData()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "asc";

            List<ProductSaleReportDto> productSales = new List<ProductSaleReportDto>(); // Empty list

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductSalesReport(page, pageSize, sortOrder)).Returns(productSales);

            var service = new ProductService(mockProductRepository.Object);

            // Act
            var result = service.GetProductSalesReport(page, pageSize, sortOrder);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No record found", result.Message);
            Assert.Null(result.Data);
            mockProductRepository.Verify(r => r.GetProductSalesReport(page, pageSize, sortOrder), Times.Once);
        }

        [Fact]
        public void GetProductSalesReport_ReturnsDataWithDefaultSorting_WhenSortOrderIsInvalid()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "invalid"; // Invalid sort order

            var productSales = new List<ProductSaleReportDto>
    {
        new ProductSaleReportDto { ProductId = 101, OrderDate = new DateTime(2023, 1, 15), TotalQuantitySold = 5, ProductName = "Product A" },
        new ProductSaleReportDto { ProductId = 102, OrderDate = new DateTime(2023, 1, 10), TotalQuantitySold = 3, ProductName = "Product B" }
    };

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductSalesReport(page, pageSize, "asc")).Returns(productSales); // Default order is ascending

            var service = new ProductService(mockProductRepository.Object);

            // Act
            var result = service.GetProductSalesReport(page, pageSize, sortOrder);

            // Assert
            Assert.False(result.Success);
           
            mockProductRepository.Verify(r => r.GetProductSalesReport(page, pageSize, "asc"), Times.Never);
        }

        [Fact]
        public void ProductsSoldCount_ReturnsTotalCount()
        {
            // Arrange
            int expectedCount = 5; // Example: expected count of products sold

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.ProductsSoldCount()).Returns(expectedCount);

            var service = new ProductService(mockProductRepository.Object);

            // Act
            var result = service.ProductsSoldCount();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedCount, result.Data);
            mockProductRepository.Verify(r => r.ProductsSoldCount(), Times.Once);
        }

        [Fact]
        public void ProductsSoldCount_ReturnsZero_WhenNoProductsSold()
        {
            // Arrange
            int expectedCount = 0;

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.ProductsSoldCount()).Returns(expectedCount);

            var service = new ProductService(mockProductRepository.Object);

            // Act
            var result = service.ProductsSoldCount();

            // Assert
            Assert.True(result.Success);
            Assert.Equal(expectedCount, result.Data);
            mockProductRepository.Verify(r => r.ProductsSoldCount(), Times.Once);
        }

       
    }
}
