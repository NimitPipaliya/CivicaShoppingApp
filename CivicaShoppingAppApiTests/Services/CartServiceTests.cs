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
    public class CartServiceTests
    {
        [Fact]
        public void GetCartItemsByUserId_ReturnsCartItems_Successfully()
        {
            // Arrange
            int userId = 1;
            var cartItems = new List<Cart>
        {
            new Cart { CartId = 1, UserId = userId, ProductId = 101, ProductQuantity = 2, Product = new Product { ProductId = 101, ProductName = "Product A", Quantity = 10, ProductPrice = 200 } },
            new Cart { CartId = 2, UserId = userId, ProductId = 102, ProductQuantity = 1, Product = new Product { ProductId = 102, ProductName = "Product B", Quantity = 5, ProductPrice = 200 } }
        };

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.GetCartItemsByUserId(userId)).Returns(cartItems);

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(It.IsAny<int>()))
                                 .Returns((int productId) => cartItems.FirstOrDefault(c => c.ProductId == productId)?.Product);

            var service = new CartService(mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = service.GetCartItemsByUserId(userId);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Count());

            // Verify individual cart items
            var cartDtoList = result.Data.ToList();
            Assert.Equal(1, cartDtoList[0].CartId);
            Assert.Equal(userId, cartDtoList[0].UserId);
            Assert.Equal(101, cartDtoList[0].ProductId);
            Assert.Equal(2, cartDtoList[0].ProductQuantity);
            Assert.Equal("Product A", cartDtoList[0].Product.ProductName);

            Assert.Equal(2, cartDtoList[1].CartId);
            Assert.Equal(userId, cartDtoList[1].UserId);
            Assert.Equal(102, cartDtoList[1].ProductId);
            Assert.Equal(1, cartDtoList[1].ProductQuantity);
            Assert.Equal("Product B", cartDtoList[1].Product.ProductName);

            mockCartRepository.Verify(r => r.GetCartItemsByUserId(userId), Times.Once);
        }

        [Fact]
        public void GetCartItemsByUserId_ReturnsNoItemsInCart_Message()
        {
            // Arrange
            int userId = 1;
            List<Cart> cartItems = new List<Cart>(); // Empty list

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.GetCartItemsByUserId(userId)).Returns(cartItems);

            var service = new CartService(mockCartRepository.Object, Mock.Of<IProductRepository>());

            // Act
            var result = service.GetCartItemsByUserId(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("No items in cart", result.Message);
            Assert.Null(result.Data);
            mockCartRepository.Verify(r => r.GetCartItemsByUserId(userId), Times.Once);
        }

        [Fact]
        public void AddToCart_SuccessfullyAddsProductToCart()
        {
            // Arrange
            var addToCartDto = new AddToCartDto
            {
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 3
            };

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Product A",
                Quantity = 5 // Available quantity
            };

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(addToCartDto.ProductId)).Returns(product);

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.AddToCart(It.IsAny<Cart>())).Returns(true);

            var service = new CartService(mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = service.AddToCart(addToCartDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Product added to cart", result.Message);
            mockProductRepository.Verify(r => r.GetProductById(addToCartDto.ProductId), Times.Once);
            mockCartRepository.Verify(r => r.AddToCart(It.IsAny<Cart>()), Times.Once);
        }

        [Fact]
        public void AddToCart_FailsWhenProductQuantityExceedsAvailability()
        {
            // Arrange
            var addToCartDto = new AddToCartDto
            {
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 10 // More than available quantity
            };

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Product A",
                Quantity = 5 // Available quantity
            };

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(addToCartDto.ProductId)).Returns(product);

            var service = new CartService(Mock.Of<ICartRepository>(), mockProductRepository.Object);

            // Act
            var result = service.AddToCart(addToCartDto);

            // Assert
            Assert.False(result.Success);
            Assert.Contains("The available quantity of product Product A is only 5", result.Message);
            mockProductRepository.Verify(r => r.GetProductById(addToCartDto.ProductId), Times.Once);
        }

        [Fact]
        public void AddToCart_FailsWhenProductQuantityExceedsLimit()
        {
            // Arrange
            var addToCartDto = new AddToCartDto
            {
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 6 
            };

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Product A",
                Quantity = 10 
            };

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(addToCartDto.ProductId)).Returns(product);

            var service = new CartService(Mock.Of<ICartRepository>(), mockProductRepository.Object);

            // Act
            var result = service.AddToCart(addToCartDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cannot add more than 5 products of the same type", result.Message);
            mockProductRepository.Verify(r => r.GetProductById(addToCartDto.ProductId), Times.Once);
        }

        [Fact]
        public void AddToCart_FailsWhenCartRepositoryFailsToAdd()
        {
            // Arrange
            var addToCartDto = new AddToCartDto
            {
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 2
            };

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Product A",
                Quantity = 10 // Available quantity
            };

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(addToCartDto.ProductId)).Returns(product);

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.AddToCart(It.IsAny<Cart>())).Returns(false); // Simulate failure

            var service = new CartService(mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = service.AddToCart(addToCartDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cannot add more than 10 items to cart", result.Message);
            mockProductRepository.Verify(r => r.GetProductById(addToCartDto.ProductId), Times.Once);
            mockCartRepository.Verify(r => r.AddToCart(It.IsAny<Cart>()), Times.Once);
        }

        [Fact]
        public void UpdateCart_SuccessfullyUpdatesCart()
        {
            // Arrange
            var updateCartDto = new UpdateCartDto
            {
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 3
            };

            var cartItem = new Cart
            {
                CartId = 1,
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 1, // Existing quantity in cart
                Product = new Product { ProductId = 101, ProductName = "Product A", Quantity = 5 }
            };

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Product A",
                Quantity = 5 // Available quantity
            };

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId)).Returns(cartItem);
            mockCartRepository.Setup(r => r.UpdateCart(It.IsAny<Cart>())).Returns(true);

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(updateCartDto.ProductId)).Returns(product);

            var service = new CartService(mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = service.UpdateCart(updateCartDto);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Cart updated successfully", result.Message);
            Assert.Equal(updateCartDto.ProductQuantity, cartItem.ProductQuantity); // Ensure cart item quantity updated
            mockCartRepository.Verify(r => r.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId), Times.Once);
            mockCartRepository.Verify(r => r.UpdateCart(It.IsAny<Cart>()), Times.Once);
        }

       
        [Fact]
        public void UpdateCart_FailsWhenProductQuantityExceedsAvailability()
        {
            // Arrange
            var updateCartDto = new UpdateCartDto
            {
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 10 // More than available quantity
            };

            var cartItem = new Cart
            {
                CartId = 1,
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 1, // Existing quantity in cart
                Product = new Product { ProductId = 101, ProductName = "Product A", Quantity = 5 }
            };

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Product A",
                Quantity = 5 // Available quantity
            };

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId)).Returns(cartItem);

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(updateCartDto.ProductId)).Returns(product);

            var service = new CartService(mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = service.UpdateCart(updateCartDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("The available quantity of product Product A is only 5", result.Message);
            mockCartRepository.Verify(r => r.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId), Times.Once);
        }

        [Fact]
        public void UpdateCart_FailsWhenProductQuantityExceedsLimit()
        {
            // Arrange
            var updateCartDto = new UpdateCartDto
            {
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 6 // More than allowed quantity (5)
            };

            var cartItem = new Cart
            {
                CartId = 1,
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 1, // Existing quantity in cart
                Product = new Product { ProductId = 101, ProductName = "Product A", Quantity = 10 }
            };

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Product A",
                Quantity = 10 // Available quantity
            };

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId)).Returns(cartItem);

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(updateCartDto.ProductId)).Returns(product);

            var service = new CartService(mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = service.UpdateCart(updateCartDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Cannot add more than 5 products of the same type", result.Message);
            mockCartRepository.Verify(r => r.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId), Times.Once);
        }

        [Fact]
        public void UpdateCart_FailsWhenCartRepositoryFailsToUpdate()
        {
            // Arrange
            var updateCartDto = new UpdateCartDto
            {
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 3
            };

            var cartItem = new Cart
            {
                CartId = 1,
                UserId = 1,
                ProductId = 101,
                ProductQuantity = 1, // Existing quantity in cart
                Product = new Product { ProductId = 101, ProductName = "Product A", Quantity = 10 }
            };

            var product = new Product
            {
                ProductId = 101,
                ProductName = "Product A",
                Quantity = 10 // Available quantity
            };

            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId)).Returns(cartItem);
            mockCartRepository.Setup(r => r.UpdateCart(It.IsAny<Cart>())).Returns(false); // Simulate failure

            var mockProductRepository = new Mock<IProductRepository>();
            mockProductRepository.Setup(r => r.GetProductById(updateCartDto.ProductId)).Returns(product);

            var service = new CartService(mockCartRepository.Object, mockProductRepository.Object);

            // Act
            var result = service.UpdateCart(updateCartDto);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, please try after sometime.", result.Message);
            mockCartRepository.Verify(r => r.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId), Times.Once);
            mockCartRepository.Verify(r => r.UpdateCart(It.IsAny<Cart>()), Times.Once);
        }

        [Fact]
        public void RemoveAllItemsForUser_SuccessfullyRemovesItems()
        {
            // Arrange
            var userId = 1;
            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.RemoveAllItemsForUser(userId)).Returns(true);

            var service = new CartService(mockCartRepository.Object, Mock.Of<IProductRepository>());

            // Act
            var result = service.RemoveAllItemsForUser(userId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Items removed successfully", result.Message);
            mockCartRepository.Verify(r => r.RemoveAllItemsForUser(userId), Times.Once);
        }

        [Fact]
        public void RemoveAllItemsForUser_FailsWhenNoItemsToRemove()
        {
            // Arrange
            var userId = 1;
            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.RemoveAllItemsForUser(userId)).Returns(false);

            var service = new CartService(mockCartRepository.Object, Mock.Of<IProductRepository>());

            // Act
            var result = service.RemoveAllItemsForUser(userId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, please try after sometime.", result.Message);
            mockCartRepository.Verify(r => r.RemoveAllItemsForUser(userId), Times.Once);
        }

        [Fact]
        public void RemoveParticularItemFromCart_SuccessfullyRemovesItems()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.RemoveParticularItem(userId,productId)).Returns(true);

            var service = new CartService(mockCartRepository.Object, Mock.Of<IProductRepository>());

            // Act
            var result = service.RemoveParticularItemFromCart(userId,productId);

            // Assert
            Assert.True(result.Success);
            Assert.Equal("Item removed successfully", result.Message);
            mockCartRepository.Verify(r => r.RemoveParticularItem(userId,productId), Times.Once);
        }

        [Fact]
        public void RemoveParticularItemFromCart_FailsWhenNoItemsToRemove()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var mockCartRepository = new Mock<ICartRepository>();
            mockCartRepository.Setup(r => r.RemoveParticularItem(userId,productId)).Returns(false);

            var service = new CartService(mockCartRepository.Object, Mock.Of<IProductRepository>());

            // Act
            var result = service.RemoveParticularItemFromCart(userId,productId);

            // Assert
            Assert.False(result.Success);
            Assert.Equal("Something went wrong, please try after sometime.", result.Message);
            mockCartRepository.Verify(r => r.RemoveParticularItem(userId, productId), Times.Once);
        }
    }
}
