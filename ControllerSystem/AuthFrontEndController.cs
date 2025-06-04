using HotelDBFinal.LoginModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelDBFinal.ControllerSystem
{
    [Route("api/auth-frontend")]
    [ApiController]
    public class AuthFrontEndController : ControllerBase
    {
        // Simple GET to check server is alive or show message
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok(new { message = "AuthFrontEndController is working" });
        }

        // Optionally you can create a simple POST login endpoint
        // that just validates hardcoded username/password for demonstration only:
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginModel login) // <== FromBody for JSON
        {
            if (login.Username == "admin" && login.Password == "password123")
            {
                // your login success logic here
                return Ok(new { success = true });
            }

            return Unauthorized(new { success = false, message = "Invalid credentials" });
        }
    }
}