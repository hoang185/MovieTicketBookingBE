using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IMovieRepository : IBaseRepository<Movie>
    {
        Task<IEnumerable<MovieListDTO>> GetAllMoviesAsync();
        Task<MovieDetailDTO?> GetMovieByIdAsync(int movieId);
    }
}
