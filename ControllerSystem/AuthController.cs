using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HotelDBFinal.LoginModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace HotelDBFinal.ControllerSystem
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        [HttpGet("login")]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View(); // This returns Views/Auth/Login.cshtml
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginModel login)
        {
            if (login.Username == "admin" && login.Password == "password123")
            {
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, login.Username)
        };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                // ✅ REDIRECT to Swagger UI
                return Redirect("/swagger/index.html");
            }

            // 👎 Invalid login
            ViewBag.Error = "Invalid credentials";
            return View(); // or return Unauthorized() if you're using API only
        }
        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/api/auth/login");
        }
    }
}

