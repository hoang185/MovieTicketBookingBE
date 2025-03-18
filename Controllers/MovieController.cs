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

        [Authorize]
        [HttpGet("protected")]
        public IActionResult GetProtectedData()
        {
            return Ok(new { message = "Bạn đã xác thực thành công!GetPublicData" });
        }
    }
}
