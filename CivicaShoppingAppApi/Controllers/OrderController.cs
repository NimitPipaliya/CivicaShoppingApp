using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Models;
using CivicaShoppingAppApi.Services.Contract;
using CivicaShoppingAppApi.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CivicaShoppingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService) 
        { 
            _orderService = orderService;
        }
        [Authorize]
        [HttpGet("GetOrderByOrderNumber/{orderNumber}")]
        public IActionResult GetOrderByOrderNumber(int orderNumber) 
        {
            var response = _orderService.GetOrderByOrderNumber(orderNumber);

            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [Authorize]
        [HttpGet("GetAllOrdersByUserId")]
        public IActionResult GetAllOrdersByUserId(int userId, int page=1, int pageSize=5, string sort_direction="desc")
        {
            var response = _orderService.GetAllOrdersByUserId(userId,page,pageSize,sort_direction);

            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [Authorize]
        [HttpGet("TotalOrderByUserId")]
        public IActionResult TotalOrderByUserId(int userId)
        {
            var response = _orderService.TotalOrderByUser(userId);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        [Authorize]
        [HttpPost("PlaceOrder/{userId}")]
        public IActionResult PlaceOrder(int userId)
        {
            var response = _orderService.PlaceOrder(userId);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


    }
}
