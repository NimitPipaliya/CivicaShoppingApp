using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Services.Contract;
using CivicaShoppingAppApi.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CivicaShoppingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }
        [Authorize]
        [HttpGet("GetCartItemsByUserId")]
        public IActionResult GetCartItemsByUserId(int userId)
        {
            var response = _cartService.GetCartItemsByUserId(userId);

            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [Authorize]
        [HttpPost("AddToCart")]
        public IActionResult AddToCart(AddToCartDto addToCartDto)
        {
            var response = _cartService.AddToCart(addToCartDto);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [Authorize]
        [HttpPut("UpdateCart")]
        public IActionResult UpdateCart(UpdateCartDto updateCartDto)
        {
            var response = _cartService.UpdateCart(updateCartDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }
        [Authorize]
        [HttpDelete("RemoveItemsFromCart/{userId}")]
        public IActionResult RemoveAllItemsForUser(int userId)
        {
            if (userId > 0)
            {
                var response = _cartService.RemoveAllItemsForUser(userId);
                return response.Success ? Ok(response) : BadRequest(response);
            }
            else
            {
                return BadRequest("Please enter valid data.");
            }
        }
        [Authorize]
        [HttpDelete("RemoveParticularProductFromCart")]
        public IActionResult RemoveParticularProductFromCart(int userId, int productId)
        {
            if (userId > 0 && productId > 0)
            {
                var response = _cartService.RemoveParticularItemFromCart(userId, productId);
                return response.Success ? Ok(response) : BadRequest(response);
            }
            else
            {
                return BadRequest("Please enter valid data.");
            }
        }
    }
}
