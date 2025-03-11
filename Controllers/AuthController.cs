using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    }
}
