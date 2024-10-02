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
    public class CartRepositoryTests
    {
        [Fact]
        public void GetCartItemsByUserId_ReturnsCartItems_WhenItemsExist()
        {
            // Arrange
            int userId = 1;
            var cartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, Product = new Product { ProductId = 101, ProductName = "Product A" } },
            new Cart { CartId = 2, UserId = userId, Product = new Product { ProductId = 102, ProductName = "Product B" } }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Cart>>();
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartItems.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartItems.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartItems.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartItems.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            var result = target.GetCartItemsByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Equal("Product A", result.First().Product.ProductName); 
            mockDbContext.Verify(c => c.Carts, Times.Once);
        }

        [Fact]
        public void GetCartItemsByUserId_ReturnsEmptyList_WhenNoItemsExist()
        {
            // Arrange
            int userId = 1;
            var cartItems = new List<Cart>().AsQueryable(); 

            var mockDbSet = new Mock<DbSet<Cart>>();
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartItems.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartItems.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartItems.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartItems.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            var result = target.GetCartItemsByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            mockDbContext.Verify(c => c.Carts, Times.Once);
        }

        [Fact]
        public void GetCartItemByUserIdAndProductId_ReturnsCartItem_WhenItemExists()
        {
            // Arrange
            int userId = 1;
            int productId = 101;
            var cartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, ProductId = productId, Product = new Product { ProductId = productId, ProductName = "Product A" } },
            new Cart { CartId = 2, UserId = 2, ProductId = 102, Product = new Product { ProductId = 102, ProductName = "Product B" } }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Cart>>();
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartItems.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartItems.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartItems.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartItems.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            var result = target.GetCartItemByUserIdAndProductId(userId, productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("Product A", result.Product.ProductName);
            mockDbContext.Verify(c => c.Carts, Times.Once);
        }

        [Fact]
        public void GetCartItemByUserIdAndProductId_ReturnsNull_WhenItemDoesNotExist()
        {
            // Arrange
            int userId = 1;
            int productId = 999; 

            var cartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = 1, ProductId = 101, Product = new Product { ProductId = 101, ProductName = "Product A" } },
            new Cart { CartId = 2, UserId = 2, ProductId = 102, Product = new Product { ProductId = 102, ProductName = "Product B" } }
        }.AsQueryable();

            var mockDbSet = new Mock<DbSet<Cart>>();
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartItems.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartItems.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartItems.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartItems.GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            var result = target.GetCartItemByUserIdAndProductId(userId, productId);

            // Assert
            Assert.Null(result);
            mockDbContext.Verify(c => c.Carts, Times.Once);
        }

        [Fact]
        public void AddToCart_ReturnsTrue_WhenCartIsNotNullAndBelowLimit()
        {
            // Arrange
            int userId = 1;
            var cart = new Cart { CartId = 1, UserId = userId, ProductId = 101 }; 

            var mockDbContext = new Mock<IAppDbContext>();
            var carts = new List<Cart>().AsQueryable();
            var mockDbSet = new Mock<DbSet<Cart>>();

            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(carts.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(carts.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(carts.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(carts.GetEnumerator());

            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);
            mockDbContext.Setup(c => c.Carts.Add(It.IsAny<Cart>())).Callback<Cart>(cart => carts.ToList().Add(cart));

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.AddToCart(cart);

            // Assert
            Assert.True(result);
            mockDbContext.Verify(m => m.Carts, Times.Exactly(2));
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void AddToCart_ReturnsFalse_WhenCartIsNull()
        {
            // Arrange
            Cart cart = null;
            var mockDbContext = new Mock<IAppDbContext>();

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.AddToCart(cart);

            // Assert
            Assert.False(result);
            mockDbContext.Verify(m => m.Carts, Times.Never);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        public void AddToCart_ReturnsFalse_WhenCartLimitExceeded()
        {
            // Arrange
            int userId = 1;
            var existingCarts = Enumerable.Range(1, 10).Select(i => new Cart { CartId = i, UserId = userId, ProductId = 100 + i }).ToList();
            var cart = new Cart { CartId = 11, UserId = userId, ProductId = 111 };

            var mockDbSet = new Mock<DbSet<Cart>>();
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(existingCarts.AsQueryable().Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(existingCarts.AsQueryable().Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(existingCarts.AsQueryable().ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(existingCarts.AsQueryable().GetEnumerator());

            var mockDbContext = new Mock<IAppDbContext>();
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.AddToCart(cart);

            // Assert
            Assert.False(result);
            mockDbSet.Verify(m => m.Add(cart), Times.Never);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never); 
        }

        [Fact]
        public void UpdateCart_ReturnsTrue_WhenCartIsNotNullAndSuccessfullyUpdated()
        {
            // Arrange
            var cart = new Cart { CartId = 1, UserId = 1, ProductId = 101 }; 

            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Cart>>();

            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.UpdateCart(cart);

            // Assert
            Assert.True(result);
            mockDbContext.Verify(m => m.Carts, Times.Once);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once); 
            mockDbContext.Verify(c => c.Carts.Update(cart), Times.Once);
        }

        [Fact]
        public void UpdateCart_ReturnsFalse_WhenCartIsNull()
        {
            // Arrange
            Cart cart = null;

            var mockDbContext = new Mock<IAppDbContext>();

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.UpdateCart(cart);

            // Assert
            Assert.False(result);
            mockDbContext.Verify(m => m.Carts, Times.Never); 
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never); 
        }

        [Fact]
        public void RemoveParticularItem_ReturnsTrue_WhenItemExistsAndSuccessfullyRemoved()
        {
            // Arrange
            int userId = 1;
            int productId = 101;
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Cart>>();
            var cartData = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, ProductId = productId }
        }.AsQueryable();
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartData.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartData.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartData.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartData.GetEnumerator());
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.RemoveParticularItem(userId, productId);

            // Assert
            Assert.True(result);
            mockDbSet.Verify(m => m.Remove(It.IsAny<Cart>()), Times.Once); 
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once); 
        }

        [Fact]
        public void RemoveParticularItem_ReturnsFalse_WhenItemNotFound()
        {
            // Arrange
            int userId = 1;
            int productId = 101;
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Cart>>();
            var cartData = new List<Cart>().AsQueryable();
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartData.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartData.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartData.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartData.GetEnumerator());
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.RemoveParticularItem(userId, productId);

            // Assert
            Assert.False(result);
            mockDbSet.Verify(m => m.Remove(It.IsAny<Cart>()), Times.Never); 
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Theory]
        [InlineData(0, 101)] // Invalid userId
        [InlineData(1, 0)]   // Invalid productId
        [InlineData(0, 0)]   // Invalid userId and productId
        public void RemoveParticularItem_ReturnsFalse_WhenUserIdOrProductIdIsInvalid(int userId, int productId)
        {
            // Arrange
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Cart>>();

            var cartData = new List<Cart>().AsQueryable();

          
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartData.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartData.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartData.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartData.GetEnumerator());

           
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.RemoveParticularItem(userId, productId);

            // Assert
            Assert.False(result);
            mockDbSet.Verify(m => m.Remove(It.IsAny<Cart>()), Times.Never); 
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never);
        }

        [Fact]
        public void RemoveAllItemsForUser_ReturnsTrue_WhenItemsExistAndSuccessfullyRemoved()
        {
            // Arrange
            int userId = 1;
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Cart>>();
            var cartData = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId },
            new Cart { CartId = 2, UserId = userId },
            new Cart { CartId = 3, UserId = userId }
        }.AsQueryable();
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartData.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartData.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartData.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartData.GetEnumerator());
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.RemoveAllItemsForUser(userId);

            // Assert
            Assert.True(result);
            mockDbSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<Cart>>()), Times.Once);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Once);
        }

        [Fact]
        public void RemoveAllItemsForUser_ReturnsFalse_WhenNoItemsFound()
        {
            // Arrange
            int userId = 1;

            // Mock DbContext and DbSet
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Cart>>();

            // In-memory data (empty)
            var cartData = new List<Cart>().AsQueryable();

            // Setup IQueryable methods
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Provider).Returns(cartData.Provider);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.Expression).Returns(cartData.Expression);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.ElementType).Returns(cartData.ElementType);
            mockDbSet.As<IQueryable<Cart>>().Setup(m => m.GetEnumerator()).Returns(cartData.GetEnumerator());

            // Setup DbContext to return DbSet
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.RemoveAllItemsForUser(userId);

            // Assert
            Assert.False(result);
            mockDbSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<Cart>>()), Times.Never);
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never); 
        }

        [Theory]
        [InlineData(0)] 
        [InlineData(-1)]
        public void RemoveAllItemsForUser_ReturnsFalse_WhenUserIdIsInvalid(int userId)
        {
            // Arrange
            var mockDbContext = new Mock<IAppDbContext>();
            var mockDbSet = new Mock<DbSet<Cart>>();
            mockDbContext.Setup(c => c.Carts).Returns(mockDbSet.Object);

            var target = new CartRepository(mockDbContext.Object);

            // Act
            bool result = target.RemoveAllItemsForUser(userId);

            // Assert
            Assert.False(result);
            mockDbSet.Verify(m => m.RemoveRange(It.IsAny<IEnumerable<Cart>>()), Times.Never); 
            mockDbContext.Verify(m => m.SaveChanges(), Times.Never); 
        }
    }
}
