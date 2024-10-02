using CivicaShoppingAppClient.Infrastructure;
using CivicaShoppingAppClient.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace CivicaShoppingAppClient.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientService _httpClientService;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenHandler _tokenHandler;
        private string endPoint;
        public AuthController(IHttpClientService httpClientService, IConfiguration configuration, IJwtTokenHandler tokenHandler)
        {
            _httpClientService = httpClientService;
            _configuration = configuration;
            endPoint = _configuration["EndPoint:CivicaApi"];
            _tokenHandler = tokenHandler;
        }
        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LoginUser(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                string apiUrl = $"{endPoint}Auth/Login";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, login, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    string token = serviceResponse.Data;
                    Response.Cookies.Append("jwtToken", token, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTime.UtcNow.AddHours(1)
                    });
                    var jwtToken = _tokenHandler.ReadJwtToken(token);
                    var userId = jwtToken.Claims.First(claim => claim.Type == "UserId").Value;
                    Response.Cookies.Append("userId", userId, new CookieOptions
                    {
                        HttpOnly = false,
                        Secure = true,
                        SameSite = SameSiteMode.None,
                        Expires = DateTime.UtcNow.AddDays(1),
                    });
                    //var id = Convert.ToInt32(userId);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse?.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong, please try after sometime";
                    }
                }
            }
            return View(login);
        }
        public IActionResult LogoutUser()
        {
            Response.Cookies.Delete("jwtToken");
            Response.Cookies.Delete("userId");
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult ForgetPassword()
        {
            ForgetPasswordViewModel forgetPasswordViewModel = new ForgetPasswordViewModel();
            forgetPasswordViewModel.SecurityQuestion = GetSecurityQuestions();
            return View(forgetPasswordViewModel);
        }
        [HttpPost]
        public IActionResult ForgetPassword(ForgetPasswordViewModel forgetPasswordViewModel)
        {
            forgetPasswordViewModel.SecurityQuestion = GetSecurityQuestions();
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}Auth/ForgetPassword";
                HttpResponseMessage response = _httpClientService.PutHttpResponseMessage(apiUrl, forgetPasswordViewModel, HttpContext.Request);

                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    TempData["SuccessMessage"] = serviceResponse?.Message;
                    //User.Identity.Name = userViewModel.LoginId;
                    //Response.Cookies.Delete("jwtToken");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);

                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse?.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong, please try after sometime";
                    }
                }
            }
            return View(forgetPasswordViewModel);
        }
        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ChangePassword(ChangePasswordViewModel changeViewModel)
        {
            if (ModelState.IsValid)
            {
                var apiUrl = $"{endPoint}Auth/ChangePassword";
                HttpResponseMessage response = _httpClientService.PutHttpResponseMessage(apiUrl, changeViewModel, HttpContext.Request);

                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    TempData["SuccessMessage"] = serviceResponse?.Message;
                    //User.Identity.Name = userViewModel.LoginId;
                    Response.Cookies.Delete("jwtToken");
                    return RedirectToAction("LoginUser", "Auth");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);

                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse?.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong, please try after sometime";
                    }
                }
            }
            return View(changeViewModel);
        }
        private IEnumerable<SecurityQuestionViewModel> GetSecurityQuestions()
        {
            var apiUrl = $"{endPoint}Auth/GetAllSecurityQuestions";

            ServiceResponse<IEnumerable<SecurityQuestionViewModel>> response = new ServiceResponse<IEnumerable<SecurityQuestionViewModel>>();

            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<SecurityQuestionViewModel>>>
                (apiUrl, HttpMethod.Get, HttpContext.Request);

            return response.Data;
        }

        public IActionResult Index(string? search, int page = 1, int pageSize = 6, string sortOrder = "asc")
        {
            var apiGetUsersUrl = "";

            var apiGetCountUrl = "";

            ViewBag.Search = search;

            if (search != null)
            {
                apiGetUsersUrl = $"{endPoint}Auth/GetAllUsers/?search={search}&page={page}&pageSize={pageSize}&sortOrder={sortOrder}";
                apiGetCountUrl = $"{endPoint}Auth/GetUsersCount/?search={search}";
            }

            else
            {
                apiGetUsersUrl = $"{endPoint}Auth/GetAllUsers/?page={page}&pageSize={pageSize}&sortOrder={sortOrder}";
                apiGetCountUrl = $"{endPoint}Auth/GetUsersCount";

            }
            ServiceResponse<int> countOfUser = new ServiceResponse<int>();

            countOfUser = _httpClientService.ExecuteApiRequest<ServiceResponse<int>>
                (apiGetCountUrl, HttpMethod.Get, HttpContext.Request);

            int totalCount = countOfUser.Data;

            if (totalCount == 0)
            {
                // Return an empty view
                return View(new List<UserViewModel>());
            }
            var totalPages = (int)Math.Ceiling((double)totalCount / pageSize);


            if (page > totalPages)
            {
                // Redirect to the first page with the new page size
                return RedirectToAction("Index", new { page = 1, pageSize });
            }
            ViewBag.CurrentPage = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.SortOrder = sortOrder;
            ServiceResponse<IEnumerable<UserViewModel>> response = new ServiceResponse<IEnumerable<UserViewModel>>();

            response = _httpClientService.ExecuteApiRequest<ServiceResponse<IEnumerable<UserViewModel>>>
                (apiGetUsersUrl, HttpMethod.Get, HttpContext.Request);

            if (response!=null && response.Success)
            {
                return View(response.Data);
            }
            else if(response == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new List<UserViewModel>());
        }

        [HttpPost]
        public IActionResult Delete(int userId)
        {
            var apiUrl = $"{endPoint}Auth/Remove/" + userId;
            var response = _httpClientService.ExecuteApiRequest<ServiceResponse<string>>($"{apiUrl}", HttpMethod.Delete, HttpContext.Request);
            if (response.Success)
            {
                TempData["successMessage"] = response.Message;
                return RedirectToAction("Index");
            }
            else
            {
                TempData["errorMessage"] = response.Message;
                return RedirectToAction("Index");
            }

        }
        [HttpGet]
        public IActionResult RegisterUser()
        {
            RegisterViewModel registerViewModel = new RegisterViewModel();
            registerViewModel.SecurityQuestion = GetSecurityQuestions();
            return View(registerViewModel);
        }
        [HttpPost]
        public IActionResult RegisterUser(RegisterViewModel viewModel)
        {
            viewModel.SecurityQuestion = GetSecurityQuestions();
            if (ModelState.IsValid)
            {
                string apiUrl = $"{endPoint}Auth/Register";
                var response = _httpClientService.PostHttpResponseMessage(apiUrl, viewModel, HttpContext.Request);
                if (response.IsSuccessStatusCode)
                {
                    string successResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(successResponse);
                    TempData["successMessage"] = serviceResponse?.Message;
                    return RedirectToAction("RegisterSuccess");
                }
                else
                {
                    string errorResponse = response.Content.ReadAsStringAsync().Result;
                    var serviceResponse = JsonConvert.DeserializeObject<ServiceResponse<string>>(errorResponse);
                    if (serviceResponse != null)
                    {
                        TempData["ErrorMessage"] = serviceResponse?.Message;
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Something went wrong, please try after sometime";
                    }
                }
            }

            return View(viewModel);
        }
        public IActionResult RegisterSuccess()
        {
            return View();
        }
    }
}
