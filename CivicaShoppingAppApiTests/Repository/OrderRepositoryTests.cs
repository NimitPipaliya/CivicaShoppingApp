using CivicaShoppingAppApi.Data.Implementation;
using CivicaShoppingAppApi.Data;
using CivicaShoppingAppApi.Models;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppApiTests.Repository
{
    public class OrderRepositoryTests
    {
        [Fact]
        public void GetOrderByOrderNumber_ReturnsOrders_WhenOrdersExist()
        {
            // Arrange
            int orderNumber = 1;
            var orders = new List<Order>
        {
            new Order { OrderId = 1, OrderNumber = orderNumber, Product = new Product { ProductId = 101, ProductName = "Product A" } },
            new Order { OrderId = 2, OrderNumber = orderNumber, Product = new Product { ProductId = 102, ProductName = "Product B" } }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            var result = target.GetOrderByOrderNumber(orderNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsOrders_SortedByOrderDate_Ascending()
        {
            // Arrange
            int userId = 1;
            int page = 1;
            int pageSize = 10;
            string sort_direction = "asc";

            var orders = new List<Order>
    {
        new Order { OrderId = 1, UserId = userId, OrderDate = new DateTime(2023, 1, 15) },
        new Order { OrderId = 2, UserId = userId, OrderDate = new DateTime(2023, 1, 10) }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            var result = target.GetAllOrdersByUserId(userId, page, pageSize, sort_direction);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(new DateTime(2023, 1, 10), result.First().OrderDate);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsOrders_SortedByOrderDate_Descending()
        {
            // Arrange
            int userId = 1;
            int page = 1;
            int pageSize = 10;
            string sort_direction = "desc";

            var orders = new List<Order>
    {
        new Order { OrderId = 1, UserId = userId, OrderDate = new DateTime(2023, 1, 15) },
        new Order { OrderId = 2, UserId = userId, OrderDate = new DateTime(2023, 1, 10) }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            var result = target.GetAllOrdersByUserId(userId, page, pageSize, sort_direction);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal(new DateTime(2023, 1, 15), result.First().OrderDate);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            int userId = 1;
            int page = 1;
            int pageSize = 10;
            string sort_direction = "asc";

            var orders = new List<Order>().AsQueryable(); // Empty list

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            var result = target.GetAllOrdersByUserId(userId, page, pageSize, sort_direction);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsPaginatedResults()
        {
            // Arrange
            int userId = 1;
            int page = 2; // Second page
            int pageSize = 1; // Page size of 1
            string sort_direction = "asc";

            var orders = new List<Order>
    {
        new Order { OrderId = 1, UserId = userId, OrderDate = new DateTime(2023, 1, 15) },
        new Order { OrderId = 2, UserId = userId, OrderDate = new DateTime(2023, 1, 10) }
    }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            var result = target.GetAllOrdersByUserId(userId, page, pageSize, sort_direction);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result); // Should return one item due to pagination (page 2, pageSize 1)
            Assert.Equal(new DateTime(2023, 1, 15), result.First().OrderDate); // Check the order of the item returned
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }


        [Fact]
        public void GetOrderByOrderNumber_ReturnsEmptyList_WhenNoOrdersExist()
        {
            // Arrange
            int orderNumber = 999; 
            var orders = new List<Order>().AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            var result = target.GetOrderByOrderNumber(orderNumber);

            // Assert
            Assert.Empty(result);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

       
        [Fact]
        public void GetOrderByOrderNumber_ReturnsAllOrders_WhenMultipleOrdersExist()
        {
            // Arrange
            int orderNumber = 1;
            var orders = new List<Order>
        {
            new Order { OrderId = 1, OrderNumber = orderNumber, Product = new Product { ProductId = 101, ProductName = "Product A" } },
            new Order { OrderId = 2, OrderNumber = orderNumber, Product = new Product { ProductId = 102, ProductName = "Product B" } },
            new Order { OrderId = 3, OrderNumber = orderNumber, Product = new Product { ProductId = 103, ProductName = "Product C" } }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            var result = target.GetOrderByOrderNumber(orderNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count()); 
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

       
     

        [Fact]
        public void TotalOrderByUserId_ReturnsCorrectCount_WhenOrdersExist()
        {
            // Arrange
            int userId = 1;
            var orders = new List<Order>
        {
            new Order { OrderId = 1, UserId = userId },
            new Order { OrderId = 2, UserId = userId },
            new Order { OrderId = 3, UserId = userId }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            int totalCount = target.TotalOrderByUserId(userId);

            // Assert
            Assert.Equal(1, totalCount);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }

        [Fact]
        public void TotalOrderByUserId_ReturnsZero_WhenNoOrdersExist()
        {
            // Arrange
            int userId = 1;
            var orders = new List<Order>().AsQueryable(); 

            var mockDbSet = new Mock<DbSet<Order>>();
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockDbSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            int totalCount = target.TotalOrderByUserId(userId);

            // Assert
            Assert.Equal(0, totalCount);
            mockDbContext.Verify(c => c.Orders, Times.Once);
        }


        [Fact]
        public void PlaceOrder_ReturnsTrue_WhenOrderIsNotNull()
        {
            // Arrange
            var order = new Order { OrderId = 1, UserId = 1, ProductId = 101 }; 
            var mockDbSet = new Mock<DbSet<Order>>();

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            bool result = target.PlaceOrder(order);

            // Assert
            Assert.True(result);
            mockDbSet.Verify(m => m.Add(order), Times.Once);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void PlaceOrder_ReturnsFalse_WhenOrderIsNull()
        {
            // Arrange
            Order order = null;
            var mockDbSet = new Mock<DbSet<Order>>();

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Orders).Returns(mockDbSet.Object);

            var target = new OrderRepository(mockDbContext.Object);

            // Act
            bool result = target.PlaceOrder(order);

            // Assert
            Assert.False(result);
            mockDbSet.Verify(m => m.Add(It.IsAny<Order>()), Times.Never);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never); 
        }
    }
}
