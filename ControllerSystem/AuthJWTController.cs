
using System.Security.Claims;
using HotelDBFinal.DTONew;
using HotelDBMiddle.Interfaces_And_Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelDBFinal.ControllerSystem
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthJWTController : ControllerBase
    {
        private readonly IAuthJWTService _authjwtService;
        public AuthJWTController(IAuthJWTService authjwtService)
        {
            _authjwtService = authjwtService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> Test()
        {
            return Ok(new { message = "ok" });
        }

        [HttpPost("profile"), Authorize]//Thứ tự Roles ưu tiên trước, nếu không có sẽ bị error
        public async Task<IActionResult> GetUser()
        {
            try
            {
                var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var email = User.FindFirstValue(ClaimTypes.Email);
                var roleClaims = User.Claims
                                        .Where(c => c.Type == ClaimTypes.Role)
                                        .Select(c => c.Value)
                                        .ToList();
                return Ok(new
                {
                    message = "Cập nhật thành công",
                    id,
                    email,
                    roleClaims
                });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { ex.Message });
            }
        }

        [HttpPost("refresh-token"),]
        public async Task<IActionResult> RefreshToken()
        {
            try
            {
                var accessToken = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                var result = _authjwtService.RefreshToken(accessToken);
                return Ok(result);
            }
            catch (System.Exception)
            {
                throw;
            }
        }
        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromBody] AuthDTO dto)
        {
            var user = await _authjwtService.SignUp(dto);
            return Ok(new { user });
        }
        [HttpPost("login")]
        public async Task<IActionResult> Signin([FromBody] AuthDTO dto)
        {
            var authResult = await _authjwtService.SignIn(dto);
            return Ok(new
            {
                user = authResult.Item1, authResult.Item2
            });
        }
    }
}
