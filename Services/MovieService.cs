using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;

namespace MovieTicketBooking.Services
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<IEnumerable<MovieDto>> GetMoviesAsync()
        {
            return await _movieRepository.GetAllMoviesAsync();
        }
    }
}
