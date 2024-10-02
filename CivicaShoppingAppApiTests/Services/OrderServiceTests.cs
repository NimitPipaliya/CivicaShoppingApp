using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Implementation;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CivicaShoppingAppApiTests.Services
{
    public class OrderServiceTests
    {
        [Fact]
        public void PlaceOrder_ReturnsSuccess_WhenOrderPlacedSuccessfully()
        {
            // Arrange
            int userId = 1;
            // Mock dependencies
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();

            // Mock data
            var cartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, ProductId = 101, ProductQuantity = 2 , Product = new Product 
            {
                ProductId = 101, ProductName = "Product A", Quantity = 10, finalPrice = 10.0
            } 
            
            },
            new Cart { CartId = 2, UserId = userId, ProductId = 102, ProductQuantity = 3, Product = new Product {

                ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0 
            } }
        };

            var products = new List<Product>
        {
            new Product { ProductId = 101, ProductName = "Product A", Quantity = 10, finalPrice = 10.0 },
            new Product { ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0 }
        };

            mockCartRepository.Setup(c => c.GetCartItemsByUserId(userId)).Returns(cartItems);
            mockProductRepository.Setup(p => p.GetProductById(It.IsAny<int>())).Returns((int productId) =>
            {
                return products.Find(p => p.ProductId == productId);
            });
            mockOrderRepository.Setup(c => c.PlaceOrder(It.IsAny<Order>())).Returns(true);

            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.PlaceOrder(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Order placed successfully!", result.Message);
            mockOrderRepository.Verify(o => o.PlaceOrder(It.IsAny<Order>()), Times.Exactly(cartItems.Count));
            mockCartRepository.Verify(c => c.RemoveParticularItem(It.IsAny<int>(), It.IsAny<int>()), Times.Exactly(cartItems.Count));
            mockProductRepository.Verify(p => p.GetProductById(It.IsAny<int>()), Times.Exactly(cartItems.Count));
        }


        [Fact]
        public void PlaceOrder_ReturnsFalse_WhenOrderNotPlaced()
        {
            // Arrange
            int userId = 1;
            // Mock dependencies
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();

            // Mock data
            var cartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, ProductId = 101, ProductQuantity = 2 , Product = new Product
            {
                ProductId = 101, ProductName = "Product A", Quantity = 10, finalPrice = 10.0
            }

            },
            new Cart { CartId = 2, UserId = userId, ProductId = 102, ProductQuantity = 3, Product = new Product {

                ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0
            } }
        };

            var products = new List<Product>
        {
            new Product { ProductId = 101, ProductName = "Product A", Quantity = 10, finalPrice = 10.0 },
            new Product { ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0 }
        };

            mockCartRepository.Setup(c => c.GetCartItemsByUserId(userId)).Returns(cartItems);
            mockProductRepository.Setup(p => p.GetProductById(It.IsAny<int>())).Returns((int productId) =>
            {
                return products.Find(p => p.ProductId == productId);
            });
          

            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.PlaceOrder(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to place order for product Product A", result.Message);
           
            mockProductRepository.Verify(p => p.GetProductById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void PlaceOrder_ReturnsFalse_WhenQuantityMoreThan5()
        {
            // Arrange
            int userId = 1;
            // Mock dependencies
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();

            // Mock data
            var cartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, ProductId = 101, ProductQuantity = 6 , Product = new Product
            {
                ProductId = 101, ProductName = "Product A", Quantity = 10, finalPrice = 10.0
            }

            },
            new Cart { CartId = 2, UserId = userId, ProductId = 102, ProductQuantity = 3, Product = new Product {

                ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0
            } }
        };

            var products = new List<Product>
        {
            new Product { ProductId = 101, ProductName = "Product A", Quantity = 10, finalPrice = 10.0 },
            new Product { ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0 }
        };

            mockCartRepository.Setup(c => c.GetCartItemsByUserId(userId)).Returns(cartItems);
            mockProductRepository.Setup(p => p.GetProductById(It.IsAny<int>())).Returns((int productId) =>
            {
                return products.Find(p => p.ProductId == productId);
            });


            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.PlaceOrder(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cannot add more than 5 quantity for a product", result.Message);

            mockProductRepository.Verify(p => p.GetProductById(It.IsAny<int>()), Times.Once);
        }

        [Fact]
        public void PlaceOrder_ReturnsFalse_WhenQuantityNotAvailable()
        { 
            // Arrange
            int userId = 1;
            // Mock dependencies
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();

            // Mock data
            var cartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, ProductId = 101, ProductQuantity =4 , Product = new Product
            {
                ProductId = 101, ProductName = "Product A", Quantity = 3, finalPrice = 10.0
            }

            },
            new Cart { CartId = 2, UserId = userId, ProductId = 102, ProductQuantity = 3, Product = new Product {

                ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0
            } }
        };

            var products = new List<Product>
        {
            new Product { ProductId = 101, ProductName = "Product A", Quantity = 3, finalPrice = 10.0 },
            new Product { ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0 }
        };

            mockCartRepository.Setup(c => c.GetCartItemsByUserId(userId)).Returns(cartItems);
            mockProductRepository.Setup(p => p.GetProductById(It.IsAny<int>())).Returns((int productId) =>
            {
                return products.Find(p => p.ProductId == productId);
            });


            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.PlaceOrder(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Failed to place order for product Product B", result.Message);

            mockProductRepository.Verify(p => p.GetProductById(It.IsAny<int>()), Times.Exactly(2));
        }

        [Fact]
        public void PlaceOrder_ReturnsFailure_WhenProductIsNull()
        {
            // Arrange
            int userId = 1;
            var mockCartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, ProductId = 101, ProductQuantity = 2, Product = new Product
            {
                ProductId = 101, ProductName = "Product A", Quantity = 10, finalPrice = 10.0
            }
            }
        };

            // Mock dependencies
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();

            mockCartRepository.Setup(c => c.GetCartItemsByUserId(userId)).Returns(mockCartItems);

            // Simulate product being null
            mockProductRepository.Setup(p => p.GetProductById(It.IsAny<int>())).Returns((int productId) => null);

            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.PlaceOrder(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, please try after some time.", result.Message);
            Assert.Null(result.Data);

            mockCartRepository.Verify(c => c.GetCartItemsByUserId(userId), Times.Once);
            mockProductRepository.Verify(p => p.GetProductById(It.IsAny<int>()), Times.Once);
            mockOrderRepository.Verify(o => o.PlaceOrder(It.IsAny<Order>()), Times.Never);
            mockCartRepository.Verify(c => c.RemoveParticularItem(It.IsAny<int>(), It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public void GetOrderByOrderNumber_ReturnsOrders_WhenOrdersFound()
        {
            // Arrange
            int orderNumber = 12345; // Example order number
            var mockOrderItems = new List<Order>
    {
        new Order { OrderId = 1, OrderNumber = orderNumber, UserId = 1, OrderDate = DateTime.Now, OrderQuantity = 2, OrderAmount = 20.0, ProductId = 101,
                    Product = new Product { ProductId = 101, ProductName = "Product A", Quantity = 10, finalPrice = 10.0 } },
        new Order { OrderId = 2, OrderNumber = orderNumber, UserId = 1, OrderDate = DateTime.Now, OrderQuantity = 3, OrderAmount = 45.0, ProductId = 102,
                    Product = new Product { ProductId = 102, ProductName = "Product B", Quantity = 5, finalPrice = 15.0 } }
    };

            // Mock dependencies
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();

            mockOrderRepository.Setup(r => r.GetOrderByOrderNumber(orderNumber)).Returns(mockOrderItems);

            var orderService = new OrderService(mockOrderRepository.Object,mockCartRepository.Object,mockProductRepository.Object);

            // Act
            var result = orderService.GetOrderByOrderNumber(orderNumber);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Order details found", result.Message);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);

            // Check order details
            var orderDtos = result.Data.ToList();
            Assert.Equal(mockOrderItems.Count, orderDtos.Count);
            foreach (var expectedOrder in mockOrderItems)
            {
                var actualOrder = orderDtos.Find(o => o.OrderId == expectedOrder.OrderId);
                Assert.NotNull(actualOrder);
                Assert.Equal(expectedOrder.UserId, actualOrder.UserId);
                Assert.Equal(expectedOrder.OrderNumber, actualOrder.OrderNumber);
                Assert.Equal(expectedOrder.OrderDate, actualOrder.OrderDate);
                Assert.Equal(expectedOrder.OrderQuantity, actualOrder.OrderQuantity);
                Assert.NotNull(actualOrder.Product);
                Assert.Equal(expectedOrder.Product.ProductId, actualOrder.Product.ProductId);
                Assert.Equal(expectedOrder.Product.ProductName, actualOrder.Product.ProductName);
                Assert.Equal(expectedOrder.Product.Quantity, actualOrder.Product.Quantity);
                Assert.Equal(expectedOrder.Product.finalPrice, actualOrder.Product.finalPrice);
            }

            mockOrderRepository.Verify(r => r.GetOrderByOrderNumber(orderNumber), Times.Once);
        }

        [Fact]
        public void GetOrderByOrderNumber_ReturnsFailure_WhenNoOrdersFound()
        {
            // Arrange
            int orderNumber = 99999; // Example order number that doesn't exist
            var mockOrderItems = new List<Order>(); // Empty list to simulate no orders found

            // Mock dependencies
            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            mockOrderRepository.Setup(r => r.GetOrderByOrderNumber(orderNumber)).Returns(mockOrderItems);

            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.GetOrderByOrderNumber(orderNumber);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Order not found", result.Message);
            Assert.Null(result.Data);

            mockOrderRepository.Verify(r => r.GetOrderByOrderNumber(orderNumber), Times.Once);
        }

        [Fact]
        public void GetOrderByOrderNumber_ReturnsFailure_WhenOrderNumberIsZero()
        {
            // Arrange
            int orderNumber = 0; // Zero order number (invalid scenario)

            var mockOrderRepository = new Mock<IOrderRepository>();
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);
            // Act
            var result = orderService.GetOrderByOrderNumber(orderNumber);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Order not found", result.Message);
            Assert.Null(result.Data);

            // No repository interaction to verify in this case
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsOrders_Successfully()
        {
            // Arrange
            int userId = 1;
            int page = 1;
            int pageSize = 10;
            string sort_direction = "asc";

            var orders = new List<OrderListDto>
    {
        new OrderListDto { OrderNumber = 1, OrderDate = new DateTime(2023, 1, 15) },
        new OrderListDto { OrderNumber = 2, OrderDate = new DateTime(2023, 1, 10) }
    };

            
            

            var mockOrderRepository = new Mock<IOrderRepository>();
            mockOrderRepository.Setup(r => r.GetAllOrdersByUserId(userId, page, pageSize, sort_direction)).Returns(orders);
            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.GetAllOrdersByUserId(userId, page, pageSize, sort_direction);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal(1, result.Data.First().OrderNumber);
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsEmptyList_Successfully()
        {
            // Arrange
            int userId = 1;
            int page = 1;
            int pageSize = 10;
            string sort_direction = "asc";

            var orders = new List<OrderListDto>(); // Empty list

            var mockOrderRepository = new Mock<IOrderRepository>();
            mockOrderRepository.Setup(r => r.GetAllOrdersByUserId(userId, page, pageSize, sort_direction)).Returns(orders);

            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.GetAllOrdersByUserId(userId, page, pageSize, sort_direction);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No record found", result.Message);
        }

        [Fact]
        public void GetAllOrdersByUserId_ReturnsError_WhenRepositoryReturnsNull()
        {
            // Arrange
            int userId = 1;
            int page = 1;
            int pageSize = 10;
            string sort_direction = "asc";

            IEnumerable<OrderListDto> orders = null; // Repository returns null

            var mockOrderRepository = new Mock<IOrderRepository>();
            mockOrderRepository.Setup(r => r.GetAllOrdersByUserId(userId, page, pageSize, sort_direction)).Returns(orders);

            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.GetAllOrdersByUserId(userId, page, pageSize, sort_direction);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("No record found", result.Message);
        }

        [Fact]
        public void TotalOrderByUser_ReturnsTotalCount_Successfully()
        {
            // Arrange
            int userId = 1;
            int totalCount = 5;

            var mockOrderRepository = new Mock<IOrderRepository>();
            mockOrderRepository.Setup(r => r.TotalOrderByUserId(userId)).Returns(totalCount);

            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.TotalOrderByUser(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(totalCount, result.Data);
        }

        [Fact]
        public void TotalOrderByUser_ReturnsZero_WhenNoOrdersExist()
        {
            // Arrange
            int userId = 1;
            int totalCount = 0;

            var mockOrderRepository = new Mock<IOrderRepository>();
            mockOrderRepository.Setup(r => r.TotalOrderByUserId(userId)).Returns(totalCount);

            var mockCartRepository = new Mock<ICartRepository>();
            var mockProductRepository = new Mock<IProductRepository>();
            var orderService = new OrderService(mockOrderRepository.Object, mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = orderService.TotalOrderByUser(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal(totalCount, result.Data);
        }
    }
}
