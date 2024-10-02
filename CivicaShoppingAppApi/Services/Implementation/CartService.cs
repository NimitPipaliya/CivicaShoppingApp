using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Data.Implementation;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Contract;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CivicaShoppingAppApi.Services.Implementation
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;

        public CartService(ICartRepository cartRepository, IProductRepository productRepository)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
        }

        public ServiceResponse<IEnumerable<UserCartDto>> GetCartItemsByUserId(int userId) 
        {
            var response = new ServiceResponse<IEnumerable<UserCartDto>>();

            var cartItems = _cartRepository.GetCartItemsByUserId(userId);
            if (cartItems != null && cartItems.Any())
            {
                List<UserCartDto> userCartDtos = new List<UserCartDto>();
                foreach (var item in cartItems)
                {
                    UserCartDto userCartDto = new UserCartDto();

                    userCartDto.CartId = item.CartId;
                    userCartDto.UserId = item.UserId;
                    userCartDto.ProductId = item.ProductId;
                    userCartDto.ProductQuantity = item.ProductQuantity;
                    userCartDto.Product = new Product()
                    {
                        ProductId = item.ProductId,
                        ProductName = item.Product.ProductName,
                        ProductDescription = item.Product.ProductDescription,
                        Quantity = item.Product.Quantity,
                        ProductPrice = item.Product.ProductPrice,
                        GstPercentage = item.Product.GstPercentage,
                        finalPrice = item.Product.finalPrice,
                    };

                    userCartDtos.Add(userCartDto);
                }

                response.Data = userCartDtos;
            }
            else
            {
                response.Success = false;
                response.Message = "No items in cart";
            }

            return response;
        }

        public ServiceResponse<string> AddToCart(AddToCartDto addToCartDto)
        {
            var response = new ServiceResponse<string>();

            var cart = new Cart()
            {
                UserId = addToCartDto.UserId,
                ProductId = addToCartDto.ProductId,
                ProductQuantity = addToCartDto.ProductQuantity,
            };

            var product = _productRepository.GetProductById(cart.ProductId);
            if(cart.ProductQuantity > product.Quantity)
            {
                response.Success = false;
                response.Message = "The available quantity of product " + product.ProductName + " is only " + product.Quantity;
                return response;
            }
            if(cart.ProductQuantity > 5)
            {
                response.Success = false;
                response.Message = "Cannot add more than 5 products of the same type";
                return response;
            }

            var result = _cartRepository.AddToCart(cart);
            if (result)
            {
                response.Message = "Product added to cart";
            }
            else
            {
                response.Success = false;
                response.Message = "Cannot add more than 10 items to cart";
            }
            return response;
        }

        public ServiceResponse<string> UpdateCart(UpdateCartDto updateCartDto)
        {
            var response = new ServiceResponse<string>();
            var cart = _cartRepository.GetCartItemByUserIdAndProductId(updateCartDto.UserId, updateCartDto.ProductId);
            var result = false;
            
            var product = _productRepository.GetProductById(updateCartDto.ProductId);
            if (updateCartDto.ProductQuantity > product.Quantity)
            {
                response.Success = false;
                response.Message = "The available quantity of product " + product.ProductName + " is only " + product.Quantity;
                return response;
            }
            if (updateCartDto.ProductQuantity > 5)
            {
                response.Success = false;
                response.Message = "Cannot add more than 5 products of the same type";
                return response;
            }

            if (cart != null)
            {
                cart.ProductQuantity = updateCartDto.ProductQuantity;

                result = _cartRepository.UpdateCart(cart);
            }

            if(result)
            {
                response.Success = true;
                response.Message = "Cart updated successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime.";
            }

            return response;
        }

        public ServiceResponse<string> RemoveAllItemsForUser(int userId)
        {
            var response = new ServiceResponse<string>();
            var result = _cartRepository.RemoveAllItemsForUser(userId);
            if (result)
            {
                response.Success = true;
                response.Message = "Items removed successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime.";
            }
            return response;
        }

        public ServiceResponse<string> RemoveParticularItemFromCart(int userId, int productId)
        {
            var response = new ServiceResponse<string>();
            var result = _cartRepository.RemoveParticularItem(userId, productId);
            if(result)
            {
                response.Success = true;
                response.Message = "Item removed successfully";
            }
            else
            {
                response.Success = false;
                response.Message = "Something went wrong, please try after sometime.";
            }
            return response;
        }
    }
}
