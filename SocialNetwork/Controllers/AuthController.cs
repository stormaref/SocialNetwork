using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid || request is null)
            {
                return BadRequest("payload is incorrect");
            }

            var result = await _userService.Login(request.Email, request.Password);
            if (!result.succeeded)
            {
                return BadRequest(new ErrorResponse("Username or password is wrong"));
            }
            return Ok(result.token);
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (!ModelState.IsValid || request is null)
            {
                return BadRequest("payload is incorrect");
            }

            var result = await _userService.SignUp(request.Email, request.Password, request.Name);
            if (!result.succeeded)
            {
                return BadRequest(new ErrorResponse(result.error));
            }
            return Ok(result.token);
        }
    }
}


public class SignUpRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email is invalid")]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}

public class SignUpResponse : LoginRequest
{
    public string Token { get; set; }
    public string Message { get; set; }
}

public class ErrorResponse
{
    public ErrorResponse(string error)
    {
        Error = new Error(error);
    }
    public Error Error { get; set; }
}

public class Error
{

    public string EnMessage { get; set; }

    public Error(string enMessage)
    {
        EnMessage = enMessage;
    }
}

public class LoginRequest
{
    [Required]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email is invalid")]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
