using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IMovieRepository : IBaseRepository<Movie>
    {
        Task<IEnumerable<MovieDto>> GetAllMoviesAsync();
    }
}
