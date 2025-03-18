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
        private readonly IConfiguration _configuration;
        private readonly int _expiryTime;
        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
            _expiryTime = Convert.ToInt16(_configuration["Jwt:ExpiresInMinutes"]);
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
                //Mặc định, mỗi lần user đăng nhập, ASP.NET Core sẽ tạo một token mới
                var token = await _authService.LoginAsync(loginRequest);

                if (token == null)
                    return Unauthorized(new { message = "Email or Password is not correct" });

                // Thiết lập cookie
                Response.Cookies.Append(Constant.JWT_TOKEN_NAME, token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,//Secure = true chỉ hoạt động trên HTTPS
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.Now.AddMinutes(_expiryTime) // Cookie tồn tại 1 giờ
                });

                return Ok(new ApiResponse<string>(token, message: "Login successfully"));
            }
            catch
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, "Server Error", false));
            }
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                if (Request.Cookies.TryGetValue(Constant.JWT_TOKEN_NAME, out var token))
                {
                    //redis: thêm token vào blacklist
                    var expiryTimeSpan = TimeSpan.FromMinutes(_expiryTime);
                    var jti = Utility.GetJtiFromToken(token);
                    if (string.IsNullOrEmpty(jti))
                    {
                        return StatusCode(401, new ApiResponse<IdentityUser>(null!, "Token does not contain jti", false));
                    }
                    await _authService.AddTokenToBlacklistAsync(jti, expiryTimeSpan);
                }
                else
                {
                    return StatusCode(401, new ApiResponse<IdentityUser>(null!, "Cookie does not contain token", false));
                }

                Response.Cookies.Append(Constant.JWT_TOKEN_NAME, "", new CookieOptions
                {
                    Expires = DateTime.UtcNow.AddDays(-1) // Đặt thời gian hết hạn về quá khứ
                });

                return Ok(new ApiResponse<IdentityUser>(null, message: "Logged out successfully"));
            }
            catch
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, "Server Error", false));
            }

        }
        [Authorize] // Yêu cầu token hợp lệ
        [HttpGet("validate")]
        public async Task<IActionResult> ValidateUser()
        {

            if (Request.Cookies.TryGetValue(Constant.JWT_TOKEN_NAME, out var token))
            {
                var expiryTimeSpan = TimeSpan.FromMinutes(_expiryTime);
                var jti = Utility.GetJtiFromToken(token);

                if (string.IsNullOrEmpty(jti))
                {
                    return StatusCode(401, new ApiResponse<IdentityUser>(null!, "Token does not contain jti", false));
                }

                if (await _authService.IsTokenInBlacklistAsync(jti))
                {
                    return StatusCode(401, new ApiResponse<IdentityUser>(null!, "Token was revoked", false));
                }

                return Ok(new ApiResponse<string>("", message: "User is authenticated"));
            }
            else
            {
                return StatusCode(401, new ApiResponse<IdentityUser>(null!, "Cookie does not contain token", false));
            }
        }
    }
}
