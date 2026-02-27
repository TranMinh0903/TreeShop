using Microsoft.AspNetCore.Mvc;
using PRN232.LaptopShop.Services.Commons.Results;
using PRN232.LaptopShop.Services.Request;
using PRN232.LaptopShop.Services.Response;
using PRN232.LaptopShop.Services.Services;

namespace PRN232.LaptopShop.API.Controllers
{
    [Route("api/v1/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticationController(AuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _authenticationService.Login(loginRequest);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<LoginResponse>.Ok(result.Value!, "Login successful"));
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
        {
            var result = await _authenticationService.Register(registerRequest);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<RegisterResponse>.Ok(result.Value!, "Registration successful"));
        }
    }
}
