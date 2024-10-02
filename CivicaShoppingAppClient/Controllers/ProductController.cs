using CivicaShoppingAppClient.Infrastructure;
using CivicaShoppingAppClient.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace CivicaShoppingAppClient.Controllers
{
    public class ProductController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private string endPoint;
        public ProductController(IHttpClientService httpClientService, IConfiguration configuration)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];

        }
        [HttpGet]
        public IActionResult Index(string? searchedProduct, int page = 1, int pageSize = 6, string sort_dir = "asc")
        {

            ViewBag.Ch = searchedProduct;

            var apiUrl = string.Empty;
            var totalCountApiUrl = string.Empty;
            if (searchedProduct != null)
            {
                apiUrl = $"{endPoint}Product/GetAllSearchedProducts"
                   + "?searchString=" + searchedProduct
                   + "&page=" + page
                   + "&pageSize=" + pageSize
                      + "&sort_dir=" + sort_dir;

                totalCountApiUrl = $"{endPoint}Product/TotalSearchedProducts?search=" + searchedProduct;

            }
            else
            {
                apiUrl = $"{endPoint}Product/GetAllProducts"
                   + "?page=" + page
                   + "&pageSize=" + pageSize
                      + "&sort_dir=" + sort_dir;

                totalCountApiUrl = $"{endPoint}Product/TotalProducts";
            }

            ServiceResponse<int> countResponse = new ServiceResponse<int>();
            ServiceResponse<IEnumerable<ProductListViewModel>> response = new ServiceResponse<IEnumerable<ProductListViewModel>>();

            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductListViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);

            countResponse = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (totalCountApiUrl, HttpMethod.Get, HttpContext.Request);

            var totalCount = countResponse.Data;

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.Ch = searchedProduct;
            ViewBag.Sort_dir = sort_dir;

            if (response.Success)
            {
                return View(response.Data);
            }

            return View(new List<ProductListViewModel>());
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AddProductViewModel addProductViewModel)
        {

            var apiUrl = $"{endPoint}Product/Create";

            var response = _httpClientService.PostHttpResponseMessage<AddProductViewModel>(apiUrl, addProductViewModel, HttpContext.Request);
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<AddProductViewModel>>(data);

                if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                {
                    return View(serviceResponse.Data);
                }
                else
                {
                    TempData["successMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }

            }
            else
            {
                string errorData = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<AddProductViewModel>>(errorData);

                if (errorResponse != null)
                {
                    TempData["errorMessage"] = errorResponse.Message;
                }
                else
                {
                    TempData["errorMessage"] = "Something went wrong, please try after sometime.";
                }

            }
            return View(addProductViewModel);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            var apiUrl = $"{endPoint}Product/GetProductById/" + id;
            var response = _httpClientService.GetHttpResponseMessage<ProductListViewModel>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<ProductListViewModel>>(data);

                if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                {
                    return View(serviceResponse.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                string errorData = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<ProductListViewModel>>(errorData);

                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                }

                return RedirectToAction("Index");

            }


        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var apiUrl = $"{endPoint}Product/GetProductById/" + id;
            var response = _httpClientService.GetHttpResponseMessage<UpdateProductViewModel>(apiUrl, HttpContext.Request);

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<UpdateProductViewModel>>(data);

                if (serviceResponse != null && serviceResponse.Success && serviceResponse.Data != null)
                {
                    return View(serviceResponse.Data);
                }
                else
                {
                    TempData["ErrorMessage"] = serviceResponse?.Message;
                    return RedirectToAction("Index");
                }
            }
            else
            {
                string errorData = response.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<UpdateProductViewModel>>(errorData);

                if (errorResponse != null)
                {
                    TempData["ErrorMessage"] = errorResponse.Message;
                }
                else
                {
                    TempData["ErrorMessage"] = "Something went wrong please try after some time.";
                }

                return RedirectToAction("Index");

            }
        }
        [HttpPost]
        public IActionResult Edit(UpdateProductViewModel updateProductViewModel)
        {

            string endPoint = _configuration["EndPoint:CivicaApi"];
            var apiUrl = $"{endPoint}Product/ModifyProduct";

            HttpResponseMessage responseMessage = _httpClientService.PutHttpResponseMessage(apiUrl, updateProductViewModel, HttpContext.Request);

            if (responseMessage.IsSuccessStatusCode)
            {
                string successMessage = responseMessage.Content.ReadAsStringAsync().Result;
                var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successMessage);

                TempData["successMessage"] = serviceResponse.Message;
                return RedirectToAction("Index");
            }
            else
            {
                string errorMessage = responseMessage.Content.ReadAsStringAsync().Result;
                var errorResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorMessage);
                if (errorResponse != null)
                {
                    TempData["errorMessage"] = errorResponse?.Message;
                }
                else
                {
                    TempData["errorMessage"] = "Something went wrong, please try after sometime.";
                }
            }

            return View(updateProductViewModel);
        }
        [HttpPost]
        public IActionResult Delete(int productId)
        {
            var apiUrl = $"{endPoint}Product/Remove/" + productId;
            var response = _httpClientService.ExecuteApiRequest<ServiceResponse<string>>($"{apiUrl}", HttpMethod.Delete, HttpContext.Request);
            if (response.Success)
            {
                TempData["successMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["errorMessage"] = "Something went wrong, please try after sometime.";
                return RedirectToAction("Index");
            }

        }
        [HttpGet]
        public IActionResult QuantityOfProducts(int page = 1, int pageSize = 6, string sortOrder = "asc")
        {
            var apiUrl = $"{endPoint}Product/GetQuantityOfProducts"
                + "?page=" + page
                + "&pageSize=" + pageSize
                + "&sortOrder=" + sortOrder;

            var totalCountApiUrl = $"{endPoint}Product/TotalProducts";


            ServiceResponse<int> countResponse = new ServiceResponse<int>();
            ServiceResponse<IEnumerable<QuantityViewModel>> response = new ServiceResponse<IEnumerable<QuantityViewModel>>();

            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<QuantityViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);

            countResponse = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (totalCountApiUrl, HttpMethod.Get, HttpContext.Request);

            var totalCount = countResponse.Data;

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.SortOrder = sortOrder;
            if (response!= null && response.Success)
            {
                return View(response.Data);
            }
            else if(response == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new List<QuantityViewModel>());
        }

        [HttpGet]
        public IActionResult ProductsSold(int page = 1, int pageSize = 6, string sortOrder = "asc")
        {
            var apiUrl = $"{endPoint}Product/ProductSalesReport"
                + "?page=" + page
                + "&pageSize=" + pageSize
                + "&sortOrder=" + sortOrder;

            var totalCountApiUrl = $"{endPoint}Product/GetProductsSoldCount";


            ServiceResponse<int> countResponse = new ServiceResponse<int>();
            ServiceResponse<IEnumerable<ProductsSoldViewModel>> response = new ServiceResponse<IEnumerable<ProductsSoldViewModel>>();

            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<ProductsSoldViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);

            countResponse = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (totalCountApiUrl, HttpMethod.Get, HttpContext.Request);

            var totalCount = countResponse.Data;

            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.PageSize = pageSize;
            ViewBag.SortOrder = sortOrder;
            if (response!=null && response.Success)
            {
                return View(response.Data);
            }
            else if(response == null)
            {
                return RedirectToAction("Index", "Home");

            }

            return View(new List<ProductsSoldViewModel>());
        }
    } 
    }

