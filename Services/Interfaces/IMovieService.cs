using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieListDTO>> GetMoviesAsync();
        Task<MovieDetailDTO?> GetMovieByIdAsync(int movieId);
    }
}
