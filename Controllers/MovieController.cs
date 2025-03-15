using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Services.Interfaces;

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
        [Authorize]
        public async Task<IActionResult> GetMovies()
        {
            try
            {
                var movies = await _movieService.GetMoviesAsync();
                return Ok(new ApiResponse<IEnumerable<MovieDto>>(movies, message: "GetMovies successfully"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<IdentityUser>(null!, ex.Message, false));
            }
        }
        [HttpGet("public")]

        public IActionResult GetPublicData()
        {
            return Ok(new { message = "Bạn đã xác thực thành công!GetPublicData" });
        }
    }
}
