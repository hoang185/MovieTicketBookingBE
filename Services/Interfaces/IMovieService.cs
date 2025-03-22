using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IMovieService
    {
        Task<IEnumerable<MovieListDTO>> GetMoviesAsync();
        Task<MovieDetailDTO?> GetMovieByIdAsync(int movieId);
        Task<List<string>> LockSeatAsync(SeatSelectRequest seatSelectRequest);
        Task<bool> SaveSeatAsync(CheckoutRequest checkoutRequest);
        Task<string?> GetSeatOwnerAsync(string seatKey);
        Task ReleaseSeatAsync(string seatKey, string userId);
    }
}
