using CivicaShoppingAppApi.Data.Contract;
using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Services.Contract;
using CivicaShoppingAppApi.Services.Implementation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CivicaShoppingAppApi.Controllers
{   
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("GetAllProducts")]
        public IActionResult GetAllProducts(int page = 1, int pageSize = 5, string sort_dir="asc")
        {
            var response = _productService.GetPaginatedProducts(page, pageSize, sort_dir);

            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpGet("TotalProducts")]
        public IActionResult TotalProducts()
        {
            var response = _productService.TotalProduct();
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("GetProductById/{id}")]
        public IActionResult GetProductById(int id)
        {
            var response = _productService.GetProduct(id);

            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("Create")]
        public IActionResult AddProduct(AddProductDto addProduct)
        {
            var response = _productService.AddProduct(addProduct);
            if (!response.Success)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPut("ModifyProduct")]
        public IActionResult UpdateProduct(UpdateProductDto productDto)
        {
            var response = _productService.ModifyProduct(productDto);
            return response.Success ? Ok(response) : BadRequest(response);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("Remove/{id}")]
        public IActionResult DeleteProduct(int id)
        {
            if (id > 0)
            {
                var response = _productService.RemoveProduct(id);
                return response.Success ? Ok(response) : BadRequest(response);
            }
            else
            {
                return BadRequest("Please enter valid data.");
            }
        }

        [HttpGet("GetAllSearchedProducts")]
        public IActionResult GetAllSearchedProducts(string searchString,int page,int pageSize,string sort_dir)
        {
            var response = _productService.GetPaginatedProductsWithSearch(searchString,page,pageSize,sort_dir);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        [HttpGet("TotalSearchedProducts")]
        public IActionResult TotalSearchedProducts(string search)
        {
            var response = _productService.TotalProductStartingWithString(search);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("GetQuantityOfProducts")]
        public IActionResult GetQuantityOfSpecificProduct( int page = 1, int pageSize = 4, string sortOrder = "asc")
        {
            

            var response = _productService.GetQuantityOfSpecificProduct(page, pageSize, sortOrder);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("ProductSalesReport")]
        public IActionResult GetProductSalesReport(int page = 1, int pageSize = 4, string sortOrder = "asc")
        {
            var response = _productService.GetProductSalesReport(page, pageSize, sortOrder);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [HttpGet("GetProductsSoldCount")]
        public IActionResult GetTotalCountOfUsers()
        {
            var response = _productService.ProductsSoldCount();
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
