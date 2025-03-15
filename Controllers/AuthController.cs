using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Common;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                var result = await _authService.RegisterAsync(request);
                if (!result.Succeeded)
                {
                    return StatusCode(500, new ApiResponse<IdentityUser>(null!, string.Join(',', result.Errors.Select(x => x.Description).ToList()), false));
                }

                return Ok(new ApiResponse<IdentityUser>(null, message: "User registered successfully"));
            }
            catch
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, "Server Error", false));
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var token = await _authService.LoginAsync(loginRequest);
                
                if (token == null)
                    return Unauthorized(new { message = "Email or Password is not correct" });

                // Thiết lập cookie
                Response.Cookies.Append(Constant.JWT_TOKEN_NAME, token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,//Secure = true chỉ hoạt động trên HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.Now.AddDays(1).AddMinutes(30) // Cookie tồn tại 1 giờ
                });

                return Ok(new ApiResponse<string>(token, message: "Login successfully"));
            }
            catch
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, "Server Error", false));
            }
        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                Response.Cookies.Append(Constant.JWT_TOKEN_NAME, "", new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,//Secure = true chỉ hoạt động trên HTTPS
                    SameSite = SameSiteMode.Lax,
                    Expires = DateTime.UtcNow.AddDays(-1) // Đặt thời gian hết hạn về quá khứ
                });

                return Ok(new ApiResponse<IdentityUser>(null, message: "Logged out successfully" ));
            }
            catch
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, "Server Error", false));
            }
           
        }
        [Authorize] // Yêu cầu token hợp lệ
        [HttpGet("validate")]
        public IActionResult ValidateUser()
        {
            return Ok(new ApiResponse<string>("", message: "User is authenticated"));
        }
    }
}
