using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Services.Interfaces;
using System.Threading.Tasks;

namespace MovieTicketBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly IMovieService _movieService;
        public MovieController(IMovieService movieService)
        {
            _movieService = movieService;
        }

        [HttpGet("index")]
        public async Task<IActionResult> GetMovies()
        {
            try
            {
                var movies = await _movieService.GetMoviesAsync();
                return Ok(new ApiResponse<IEnumerable<MovieListDTO>>(movies, message: "GetMovies successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, ex.Message, false));
            }
        }

        [HttpGet("detail/{id}")]
        public async Task<IActionResult> GetMovieById(int id)
        {
            try
            {
                var movie = await _movieService.GetMovieByIdAsync(id);
                if (movie == null)
                {
                    return NotFound(new ApiResponse<MovieDetailDTO>(null!, $"Movie {id} not found", false));
                }

                return Ok(new ApiResponse<MovieDetailDTO>(movie, "Get movie successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, ex.Message, false));
            }
        }

        [HttpPost("seat-select")]
        public async Task<IActionResult> SelectSeat([FromBody] SeatSelectRequest seatSelectRequest)
        {
            try
            {
                var failedSeats = await _movieService.LockSeatAsync(seatSelectRequest);
                if (failedSeats.Any())
                {
                    return Conflict(new ApiResponse<string>(null!, $"Ghế {string.Join(',', failedSeats)} đã được chọn bởi người khác.", false));
                }
                return Ok(new ApiResponse<string>(null!, "Ghế được giữ thành công"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, ex.Message, false));
            }
        }

        [Authorize]
        [HttpPost("checkout")]
        public async Task<IActionResult> ProcessPayment([FromBody] CheckoutRequest request)
        {
            if (request.SeatIds == null || !request.SeatIds.Any())
            {
                return BadRequest(new ApiResponse<string>(null!, "Vui lòng chọn ít nhất một ghế!", false));
            }

            // Giả lập quá trình thanh toán thành công
            bool paymentSuccess = await _movieService.SaveSeatAsync(request);

            if (paymentSuccess)
            {
                return Ok(new ApiResponse<string>(null!, "Thanh toán thành công!"));
            }
            else
            {
                return StatusCode(500, new { message = "Thanh toán thất bại, vui lòng thử lại!" });
            }
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult GetProtectedData()
        {
            return Ok(new { message = "Bạn đã xác thực thành công!GetPublicData" });
        }
    }
}
