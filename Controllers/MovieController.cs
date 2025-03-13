using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MovieTicketBooking.Controllers
{
    [Route("api/protected")]
    [ApiController]
    [Authorize]
    public class MovieController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetProtectedData()
        {
            return Ok(new { message = "Bạn đã xác thực thành công!" });
        }
    }
}
