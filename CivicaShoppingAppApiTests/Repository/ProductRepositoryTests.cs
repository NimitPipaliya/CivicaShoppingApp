using CivicaShoppingAppApi.Data;
using CivicaShoppingAppApi.Data.Implementation;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppApiTests.Repository
{
    public class ProductRepositoryTests
    {
        //--------------Get all products with pagination----------------
        [Fact]
        public void GetPaginationProducts_ReturnsCorrectProducts_WhenProductsExists()
        {
            //Arrange
            var products = new List<Product>
      {
          new Product()
          {
              ProductName = "Test",
              ProductDescription = "Test",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          },
           new Product()
          {
              ProductName = "Test 1",
              ProductDescription = "Test 1",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          }
      }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(products.Provider);

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(products.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            //Act
            var actual = target.GetPaginatedProducts(1, 2, "asc");
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(2, actual.Count());
            mockAppDbContext.Verify(c => c.Products, Times.AtLeastOnce);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);


        }
        [Fact]
        public void GetPaginationProducts_ReturnsCorrectProducts_WhenProductsNotExists()
        {
            //Arrange
      var products = new List<Product>{}.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(products.Provider);

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(products.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            //Act
            var actual = target.GetPaginatedProducts(1, 2, "desc");
            //Assert
            Assert.NotNull(actual);
            Assert.Equal(products.Count(), actual.Count());
            mockAppDbContext.Verify(c => c.Products, Times.AtLeastOnce);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);


        }
        //--------------Add Product----------------

        [Fact]
        public void InsertProduct_ReturnsTrue()
        {
            //Arrange
            var Product = new Product {
                ProductName = "Test",
                ProductDescription = "Test",
                ProductId = 1,
                ProductPrice = 1,
                GstPercentage = 1,
                finalPrice = 1,
                Quantity = 1,
            };
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Products).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);

            var target = new ProductRepository(mockAppDbContext.Object);

            //Act
            var actual = target.InsertProduct(Product);

            //Assert
            Assert.True(actual);
            mockAppDbContext.Verify(c => c.Products, Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);

        }
        [Fact]
        public void InsertProduct_ReturnsFalse()
        {
            //Arrange
            var Product = new Product();
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            //Act
            var actual = target.InsertProduct(null);

            //Assert
            Assert.False(actual);

        }
        //--------------Update product----------------
        [Fact]
        public void UpdateProduct_ReturnsTrue()
        {
            //Arrange
            var Product = new Product
            {
                ProductName = "Test",
                ProductDescription = "Test",
                ProductId = 1,
                ProductPrice = 1,
                GstPercentage = 1,
                finalPrice = 1,
                Quantity = 1,
            };

            var mockDbSet = new Mock<DbSet<Product>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.SetupGet(c => c.Products).Returns(mockDbSet.Object);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);

            var target = new ProductRepository(mockAppDbContext.Object);

            //Act
            var actual = target.UpdateProduct(Product);

            //Assert
            Assert.True(actual);
            mockAppDbContext.Verify(c => c.Products, Times.Once);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);
        }
        [Fact]
        public void UpdateProduct_ReturnsFalse()
        {
            //Arrange
            var Product = new List<Product>();
            var mockDbSet = new Mock<DbSet<Product>>();

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products).Returns<Product>(null);
            var target = new ProductRepository(mockAppDbContext.Object);

            //Act
            var actual = target.UpdateProduct(null);

            //Assert
            Assert.False(actual);
            mockAppDbContext.Verify(c => c.Products, Times.Never);

        }
        //--------------Delete product----------------
        [Fact]
        public void DeleteProduct_ReturnsTrue()
        {
            //Arrange
            var Product = new Product
            {
                ProductName = "Test",
                ProductDescription = "Test",
                ProductId = 1,
                ProductPrice = 1,
                GstPercentage = 1,
                finalPrice = 1,
                Quantity = 1,
            };
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products.Find(1)).Returns(Product);
            mockAppDbContext.Setup(c => c.SaveChanges()).Returns(1);

            var target = new ProductRepository(mockAppDbContext.Object);

            //Act
            var actual = target.DeleteProduct(Product.ProductId);

            //Assert
            Assert.True(actual);
            mockAppDbContext.Verify(c => c.Products, Times.AtLeastOnce);
            mockAppDbContext.Verify(c => c.SaveChanges(), Times.Once);

        }
        [Fact]
        public void DeleteProduct_ReturnFalse()
        {
            //Arrange
            var Product = new Product
            {
                ProductName = "Test",
                ProductDescription = "Test",
                ProductId = 1,
                ProductPrice = 1,
                GstPercentage = 1,
                finalPrice = 1,
                Quantity = 1,
            };
            var mockDbSet = new Mock<DbSet<Product>>();
            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products.Find(1)).Returns<Product>(null);
            var target = new ProductRepository(mockAppDbContext.Object);


            //Act
            var actual = target.DeleteProduct(1);

            //Assert
            Assert.False(actual);
            mockAppDbContext.Verify(c => c.Products, Times.AtLeastOnce);

        }
        //--------------Get product by id----------------
        [Fact]
        public void GetProduct_ReturnsProduct_WhenProductExists()
        {
            // Arrange
            int ProductId = 1;
            var Product = new Product
            {
                ProductName = "Test",
                ProductDescription = "Test",
                ProductId = 1,
                ProductPrice = 1,
                GstPercentage = 1,
                finalPrice = 1,
                Quantity = 1,
            };
            var categories = new List<Product> { Product }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(categories.Expression);
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetProductById(ProductId);

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(Product, actual);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Products, Times.Once);
        }

        [Fact]
        public void GetProduct_ReturnsNullProduct_WhenNoProductExists()
        {
            // Arrange
            int ProductId = 1;

            var categories = new List<Product>().AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(categories.Expression);
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            // Act
            var actual = target.GetProductById(ProductId);

            // Assert
            Assert.Null(actual);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Products, Times.Once);
        }

        //--------------Product Exist ----------------


        [Fact]
        public void ProductExists_ReturnsTrue()
        {
            // Arrange
            var name = "Test";
            var products = new List<Product>
      {
          new Product()
          {
              ProductName = "Test",
              ProductDescription = "Test",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          },
           new Product()
          {
              ProductName = "Test 1",
              ProductDescription = "Test 1",
              ProductId = 2,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          }
      }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(products.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            // Act
            var actual = target.ProductExists(name);

            // Assert
            Assert.True(actual);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Products, Times.Once);
        }

        [Fact]
        public void ProductExists_ReturnsFalse()
        {
            // Arrange
            var name = "Product 3";
            var products = new List<Product>
      {
          new Product()
          {
              ProductName = "Test",
              ProductDescription = "Test",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          },
           new Product()
          {
              ProductName = "Test 1",
              ProductDescription = "Test 1",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          }
      }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(products.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            // Act
            var actual = target.ProductExists(name);

            // Assert
            Assert.False(actual);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Products, Times.Once);
        }
        //--------------Product Exist with id----------------

        [Fact]
        public void ProductExists_WithProductId_ReturnsTrue()
        {
            // Arrange
            var id = 2;
            var name = "Test";
            var products = new List<Product>
      {
          new Product()
          {
              ProductName = "Test",
              ProductDescription = "Test",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          },
           new Product()
          {
              ProductName = "Test 1",
              ProductDescription = "Test 1",
              ProductId = 2,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          }
      }.AsQueryable();
            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(products.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            // Act
            var actual = target.ProductExists(id, name);

            // Assert
            Assert.True(actual);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Products, Times.Once);
        }

        [Fact]
        public void ProductExists_WithProductId_ReturnsFalse()
        {
            // Arrange
            var id = 1;
            var name = "Product 1";
            var products = new List<Product>
      {
          new Product()
          {
              ProductName = "Test",
              ProductDescription = "Test",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          },
           new Product()
          {
              ProductName = "Test 1",
              ProductDescription = "Test 1",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          }
      }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(products.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            // Act
            var actual = target.ProductExists(id, name);

            // Assert
            Assert.False(actual);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);
            mockAppDbContext.VerifyGet(c => c.Products, Times.Once);
        }

        //--------------Total products for pagination----------------

        [Fact]
        public void TotalCategories_ReturnsTotalCount_WhenCategoriesExist()
        {
            // Arrange
            var products = new List<Product>
      {
          new Product()
          {
              ProductName = "Test",
              ProductDescription = "Test",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          },
           new Product()
          {
              ProductName = "Test 1",
              ProductDescription = "Test 1",
              ProductId = 1,
              ProductPrice = 1,
              GstPercentage = 1,
              finalPrice = 1,
              Quantity = 1,
          }
      }.AsQueryable();


            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(products.Expression);

            var mockAppDbContext = new Mock<IAppDbContext>();
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            // Act
            var actual = target.TotalProducts();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(products.Count(), actual);
            mockAppDbContext.Verify(c => c.Products, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);
        }

        [Fact]
        public void TotalCategories_ReturnsZero_WhenNoCategoriesExist()
        {
            // Arrange
            var categories = new List<Product>().AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            var mockAppDbContext = new Mock<IAppDbContext>();

            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Provider).Returns(categories.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(c => c.Expression).Returns(categories.Expression);
            mockAppDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var target = new ProductRepository(mockAppDbContext.Object);

            // Act
            var actual = target.TotalProducts();

            // Assert
            Assert.NotNull(actual);
            Assert.Equal(categories.Count(), actual);
            mockAppDbContext.Verify(c => c.Products, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Provider, Times.Once);
            mockDbSet.As<IQueryable<Product>>().Verify(c => c.Expression, Times.Once);
        }

        //------------------Searching and sorting----------------
        //-------------------total products with search--------------

        [Fact]
        public void GetQuantityOfSpecificProducts_ReturnsProductsSortedByName_Ascending()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "asc";

            var products = new List<Product>
    {
        new Product { ProductId = 1, ProductName = "Product B" },
        new Product { ProductId = 2, ProductName = "Product A" }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.GetQuantityOfSpecificProducts(page, pageSize, sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Product A", result.First().ProductName);
            mockDbContext.Verify(c => c.Products, Times.Once);
        }

        [Fact]
        public void GetQuantityOfSpecificProducts_ReturnsProductsSortedByName_Descending()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "desc";

            var products = new List<Product>
    {
        new Product { ProductId = 1, ProductName = "Product B" },
        new Product { ProductId = 2, ProductName = "Product A" }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.GetQuantityOfSpecificProducts(page, pageSize, sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Product B", result.First().ProductName);
            mockDbContext.Verify(c => c.Products, Times.Once);
        }

        [Fact]
        public void GetQuantityOfSpecificProducts_ReturnsDefaultSorting_WhenSortOrderIsInvalid()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "invalid"; // Invalid sort order

            var products = new List<Product>
    {
        new Product { ProductId = 1, ProductName = "Product B" },
        new Product { ProductId = 2, ProductName = "Product A" }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.GetQuantityOfSpecificProducts(page, pageSize, sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Default order will be ascending by ProductName
            Assert.Equal("Product A", result.First().ProductName);
            mockDbContext.Verify(c => c.Products, Times.Once);
        }

        [Fact]
        public void GetQuantityOfSpecificProducts_ReturnsPaginatedResults()
        {
            // Arrange
            int page = 2; // Second page
            int pageSize = 1; // Page size of 1
            string sortOrder = "asc";

            var products = new List<Product>
    {
        new Product { ProductId = 1, ProductName = "Product B" },
        new Product { ProductId = 2, ProductName = "Product A" }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Product>>();
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Provider).Returns(products.Provider);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.Expression).Returns(products.Expression);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.ElementType).Returns(products.ElementType);
            mockDbSet.As<IQueryable<Product>>().Setup(m => m.GetEnumerator()).Returns(products.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Products).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.GetQuantityOfSpecificProducts(page, pageSize, sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Should return one item due to pagination (page 2, pageSize 1)
            Assert.Equal("Product B", result.First().ProductName); // Check the order of the item returned
            mockDbContext.Verify(c => c.Products, Times.Once);
        }


        [Fact]
        public void GetProductSalesReport_ReturnsReportSortedByName_Ascending()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "asc";

            var orders = new List<Order>
    {
        new Order { OrderId = 1, ProductId = 101, OrderDate = new DateTime(2023, 1, 15), OrderQuantity = 5, Product = new Product { ProductId = 101, ProductName = "Product B" } },
        new Order { OrderId = 2, ProductId = 102, OrderDate = new DateTime(2023, 1, 10), OrderQuantity = 3, Product = new Product { ProductId = 102, ProductName = "Product A" } }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.GetProductSalesReport(page, pageSize, sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Product A", result.First().ProductName);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void GetProductSalesReport_ReturnsReportSortedByName_Descending()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "desc";

            var orders = new List<Order>
    {
        new Order { OrderId = 1, ProductId = 101, OrderDate = new DateTime(2023, 1, 15), OrderQuantity = 5, Product = new Product { ProductId = 101, ProductName = "Product B" } },
        new Order { OrderId = 2, ProductId = 102, OrderDate = new DateTime(2023, 1, 10), OrderQuantity = 3, Product = new Product { ProductId = 102, ProductName = "Product A" } }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.GetProductSalesReport(page, pageSize, sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Product B", result.First().ProductName);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void GetProductSalesReport_ReturnsDefaultSorting_WhenSortOrderIsInvalid()
        {
            // Arrange
            int page = 1;
            int pageSize = 10;
            string sortOrder = "invalid"; // Invalid sort order

            var orders = new List<Order>
    {
        new Order { OrderId = 1, ProductId = 101, OrderDate = new DateTime(2023, 1, 15), OrderQuantity = 5, Product = new Product { ProductId = 101, ProductName = "Product B" } },
        new Order { OrderId = 2, ProductId = 102, OrderDate = new DateTime(2023, 1, 10), OrderQuantity = 3, Product = new Product { ProductId = 102, ProductName = "Product A" } }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.GetProductSalesReport(page, pageSize, sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count()); // Default order will be ascending by ProductName
            Assert.Equal("Product A", result.First().ProductName);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void GetProductSalesReport_ReturnsPaginatedResults()
        {
            // Arrange
            int page = 2; // Second page
            int pageSize = 1; // Page size of 1
            string sortOrder = "asc";

            var orders = new List<Order>
    {
        new Order { OrderId = 1, ProductId = 101, OrderDate = new DateTime(2023, 1, 15), OrderQuantity = 5, Product = new Product { ProductId = 101, ProductName = "Product B" } },
        new Order { OrderId = 2, ProductId = 102, OrderDate = new DateTime(2023, 1, 10), OrderQuantity = 3, Product = new Product { ProductId = 102, ProductName = "Product A" } }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.GetProductSalesReport(page, pageSize, sortOrder);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Should return one item due to pagination (page 2, pageSize 1)
            Assert.Equal("Product B", result.First().ProductName); // Check the order of the item returned
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void ProductsSoldCount_ReturnsTotalCount()
        {
            // Arrange
            var orders = new List<Order>
    {
        new Order { OrderId = 1, ProductId = 101, OrderDate = new DateTime(2023, 1, 15), OrderQuantity = 5, Product = new Product { ProductId = 101, ProductName = "Product A" } },
        new Order { OrderId = 2, ProductId = 102, OrderDate = new DateTime(2023, 1, 10), OrderQuantity = 3, Product = new Product { ProductId = 102, ProductName = "Product B" } },
        new Order { OrderId = 3, ProductId = 101, OrderDate = new DateTime(2023, 1, 12), OrderQuantity = 2, Product = new Product { ProductId = 101, ProductName = "Product A" } }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.ProductsSoldCount();

            // Assert
            Assert.Equal(3, result); // Expected total unique products sold
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void ProductsSoldCount_ReturnsZero_WhenNoOrdersExist()
        {
            // Arrange
            var orders = new List<Order>().AsQueryable(); // Empty list

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.ProductsSoldCount();

            // Assert
            Assert.Equal(0, result); // No products sold, so count should be 0
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void ProductsSoldCount_ReturnsCorrectCount_WhenMultipleOrdersExistForSameProduct()
        {
            // Arrange
            var orders = new List<Order>
    {
        new Order { OrderId = 1, ProductId = 101, OrderDate = new DateTime(2023, 1, 15), OrderQuantity = 5, Product = new Product { ProductId = 101, ProductName = "Product A" } },
        new Order { OrderId = 2, ProductId = 101, OrderDate = new DateTime(2023, 1, 10), OrderQuantity = 3, Product = new Product { ProductId = 101, ProductName = "Product A" } }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var service = new ProductRepository(mockDbContext.Object);

            // Act
            var result = service.ProductsSoldCount();

            // Assert
            Assert.Equal(2, result); // Only one unique product sold
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

    }
}
