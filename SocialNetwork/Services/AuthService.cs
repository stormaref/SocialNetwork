using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SocialNetwork.Infrastructure.Entities;

namespace SocialNetwork.Services
{
    public interface IAuthService
    {
        Task<ApplicationUser> GetUser(int userId);
        Task<(bool succeeded, string token)> Login(string username, string password);
        Task<(bool succeeded, string token, string error)> SignUp(string username, string password, string name);
    }

    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(IConfiguration configuration, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _configuration = configuration;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool succeeded, string token, string error)> SignUp(
            string username, string password, string name)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is not null)
            {
                return (false, "", "User exist");
            }
            var result = await _userManager.CreateAsync(new ApplicationUser(username, name), password);
            if (!result.Succeeded)
            {
                var error = string.Join(Environment.NewLine, result.Errors.Select(e => e.Description));
                return (false, "", error);
            }
            return (true, GenerateToken(await _userManager.FindByNameAsync(username)), "");
        }

        public async Task<(bool succeeded, string token)> Login(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user is null)
            {
                return (false, "");
            }
            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            if (!result.Succeeded)
            {
                return (false, "");
            }
            return (true, GenerateToken(user));
        }

        private string GenerateToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim("email", user.Email),
                new Claim("userId", user.Id.ToString()),
                new Claim(ClaimTypes.NameIdentifier,Guid.NewGuid().ToString())
             };

            var key = _configuration["Jwt:Key"];
            var issuer = _configuration["Jwt:Issuer"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims,
                expires: DateTime.Now.AddDays(30), signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
        }

        public async Task<ApplicationUser> GetUser(int userId)
        {
            return await _userManager.FindByIdAsync(userId.ToString());
        }
    }
}

