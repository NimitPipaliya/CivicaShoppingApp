using CivicaShoppingAppClient.Infrastructure;
using CivicaShoppingAppClient.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing.Printing;

namespace CivicaShoppingAppClient.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private string endPoint;


        public CartController(IHttpClientService _httpClientService, IConfiguration _configuration)
        {
            this._httpClientService = _httpClientService;
            this._configuration = _configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }
        public IActionResult GetCartItemsByUserId(int userId)
        {
            var apiGetUsersUrl = $"{endPoint}Cart/GetCartItemsByUserId/?userId={userId}";
            ServiceResponse<IEnumerable<UserCartViewModel>> response = new ServiceResponse<IEnumerable<UserCartViewModel>>();

            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<UserCartViewModel>>>
                (apiGetUsersUrl, HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                return View(response.Data);
            }

            return View(new List<UserCartViewModel>());
        }

        [HttpPost]
        public IActionResult DeleteFromCart(int userId, int productId)
        {
            var apiUrl = $"{endPoint}Cart/RemoveParticularProductFromCart?userId={userId}&productId={productId}";
            var response = _httpClientService.ExecuteApiRequest<ServiceResponse<string>>($"{apiUrl}", HttpMethod.Delete, HttpContext.Request);
            if (response.Success)
            {
                TempData["successMessage"] = response.Message;
                return RedirectToAction("GetCartItemsByUserId", new { userId = userId });
            }
            else
            {
                TempData["errorMessage"] = response.Message;
                return RedirectToAction("GetCartItemsByUserId", new { userId = userId });
            }

        }
        public IActionResult BuyProducts(int page = 1, int pageSize = 9, string sortDir = "asc")
        {
            var apiUrl = $"{endPoint}Product/GetAllProducts"
                + "?page=" + page
                + "&pageSize=" + pageSize
                + "&sort_dir=" + sortDir;
            var countApiUrl = $"{endPoint}Product/TotalProducts";


            var response = new ServiceResponse<IEnumerable<BuyProductViewModel>>();
            var countResponse = new ServiceResponse<int>();
            var cartItemsResponse = new ServiceResponse<IEnumerable<CartItemViewModel>>()
            {
                Success = false
            };

            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<BuyProductViewModel>>>(apiUrl, HttpMethod.Get, HttpContext.Request);
            countResponse = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>(countApiUrl, HttpMethod.Get, HttpContext.Request);

            var totalCount = countResponse.Data;

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.SortDir = sortDir;
            ViewBag.TotalPages = totalPages;

            if (User.Identity.IsAuthenticated)
            {
                var cartItemsApiUrl = $"{endPoint}Cart/GetCartItemsByUserId" +
                    "?userId=" + User.FindFirst("UserId").Value;

                cartItemsResponse = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<CartItemViewModel>>>(cartItemsApiUrl, HttpMethod.Get, HttpContext.Request);

            }

            if (response.Success)
            {
                if (cartItemsResponse.Success)
                {
                    foreach (var cartItem in cartItemsResponse.Data)
                    {
                        var product = response.Data.FirstOrDefault(p => p.ProductId == cartItem.ProductId);
                        if (product != null)
                        {
                            product.isAddedToCart = true;
                        }
                    }
                }
                return View(response.Data);
            }

            return View(new List<BuyProductViewModel>());
        }

    }
}
