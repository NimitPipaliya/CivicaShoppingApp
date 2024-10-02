using CivicaShoppingAppClient.Infrastructure;
using CivicaShoppingAppClient.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Linq;

namespace CivicaShoppingAppClient.Controllers
{
    public class OrderController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private string endPoint;


        public OrderController(IHttpClientService _httpClientService, IConfiguration _configuration)
        {
            this._httpClientService = _httpClientService;
            this._configuration = _configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetAllOrdersByUserId(int userId, int page = 1, int pageSize = 6, string sort_direction = "desc")
        {
           var apiGetUsersUrl = $"{endPoint}Order/GetAllOrdersByUserId/?userId={userId}&page={page}&pageSize={pageSize}&sort_direction={sort_direction}";
            var apiGetCountUrl = $"{endPoint}Order/TotalOrderByUserId?userId={userId}";

            ServiceResponse<int> countOfUser = new ServiceResponse<int>();

            countOfUser = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (apiGetCountUrl, HttpMethod.Get, HttpContext.Request);

            int totalCount = countOfUser.Data;

            if (totalCount == 0)
            {
                ViewBag.CurrentPage = 1;
                ViewBag.TotalPages = 1;
                // Return an empty view
                return View(new List<OrderListViewModel>());
            }
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.userId = userId;

            if (page > totalPages)
            {
                // Redirect to the first page with the new page size
                return RedirectToAction("GetAllOrdersByUserId", new { page = 1, pageSize });
            }
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.sort_direction = sort_direction;
            ServiceResponse<IEnumerable<OrderListViewModel>> response = new ServiceResponse<IEnumerable<OrderListViewModel>>();

            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<OrderListViewModel>>>
                (apiGetUsersUrl, HttpMethod.Get, HttpContext.Request);

            if (response.Success)
            {
                return View(response.Data);
            }

            return View(new List<OrderListViewModel>());
        }

        [HttpGet]
        public IActionResult OrderSummary(int id)
        {
            var apiUrl = $"{endPoint}Order/GetOrderByOrderNumber/" + id;
            var response = _httpClientService.GetHttpResponseMessage<OrderSummaryViewModel>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<IEnumerable<OrderSummaryViewModel>>>(data);

                if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                {
                    return View(serviceResponse.Data);
                }
                else
                {
                    TempData["errorMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }

            }
            else
            {
                string errorData = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<OrderSummaryViewModel>>(errorData);

                if (errorResponse != null)
                {
                    TempData["errorMessage"] = errorResponse.Message;
                }
                else
                {
                    TempData["errorMessage"] = "Something went wrong. Please try after sometime.";
                }
                return RedirectToAction("Index");
            }
        }

        public IActionResult OrderPlaced(int id)
        {
            var apiUrl = $"{endPoint}Order/GetOrderByOrderNumber/" + id;
            var response = _httpClientService.GetHttpResponseMessage<OrderSummaryViewModel>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<IEnumerable<OrderSummaryViewModel>>>(data);

                if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                {
                    return View(serviceResponse.Data);
                }
                else
                {
                    TempData["errorMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }

            }
            else
            {
                string errorData = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<OrderSummaryViewModel>>(errorData);

                if (errorResponse != null)
                {
                    TempData["errorMessage"] = errorResponse.Message;
                }
                else
                {
                    TempData["errorMessage"] = "Something went wrong. Please try after sometime";
                }
                return RedirectToAction("Index");
            }
        }
    }
}
