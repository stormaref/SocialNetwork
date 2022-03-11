using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using SocialNetwork.Models;
using SocialNetwork.Services;

namespace SocialNetwork.Controllers
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _userService;

        public AuthController(IAuthService userService)
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
            return Ok(new SignUpResponse(result.token, "successfull"));
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
            return Ok(new SignUpResponse(result.token, "successfull"));
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

public class SignUpResponse
{
    public SignUpResponse(string token, string message)
    {
        Token = token;
        Message = message;
    }

    public string Token { get; set; }
    public string Message { get; set; }
}



public class LoginRequest
{
    [Required]
    [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$", ErrorMessage = "Email is invalid")]
    public string Email { get; set; }
    [Required]
    public string Password { get; set; }
}
