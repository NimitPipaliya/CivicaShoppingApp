using CivicaShoppingAppApi.Dtos;
using CivicaShoppingAppApi.Services.Contract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CivicaShoppingAppApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(LoginDto loginDto)
        {
            var response = _authService.LoginUserService(loginDto);
            return !response.Success ? BadRequest(response) : Ok(response);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpGet("GetAllUsers")]
        public IActionResult GetAllUsers(string? search, int page = 1, int pageSize = 4, string sortOrder = "asc")
        {
            var response = new ServiceResponse<IEnumerable<UserDto>>();

            response = _authService.GetAllUsers(page, pageSize, search, sortOrder);
            if (!response.Success)
            {
                return NotFound(response);
            }

            return Ok(response);
        }
        [Authorize]
        [HttpGet("GetUserById/{id}")]

        public IActionResult GetUserById(int id)
        {
            var response = _authService.GetUser(id);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpDelete("Remove/{id}")]
        public IActionResult RemoveUser(int id)
        {
            if (id > 0)
            {
                var response = _authService.RemoveUser(id);
                if (!response.Success)
                {
                    return BadRequest(response);
                }
                else
                {
                    return Ok(response);
                }
            }
            else
            {
                return BadRequest("Enter correct data please");
            }

        }
        [HttpGet("GetUsersCount")]
        public IActionResult GetTotalCountOfUsers(string? search)
        {
            var response = _authService.TotalUsers(search);
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult AddRegister(RegisterUserDto registerDto)
        {

            var response = _authService.RegisterUserService(registerDto);
            return !response.Success ? BadRequest(response) : Ok(response);


        }

        [HttpGet("GetAllSecurityQuestions")]
        public IActionResult GetAllSecurityQuestions()
        {
            var response = _authService.GetAllQuestions();
            if (!response.Success)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
        [Authorize(Policy ="UserPolicy")]
        [HttpPut("ChangePassword")]
        public IActionResult ChangePassword(ChangePasswordDto changePasswordDto)
        {
            var result = _authService.ChangePassword(changePasswordDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }
       [HttpPut("ForgetPassword")]
        public IActionResult ForgetPassword(ForgetPasswordDto forgetPasswordDto)
        {
            var result = _authService.ForgetPassword(forgetPasswordDto);
            if (!result.Success)
            {
                return BadRequest(result);
            }
            else
            {
                return Ok(result);
            }
        }

    }
}
