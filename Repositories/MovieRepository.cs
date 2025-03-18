using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Data;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Repositories.Interfaces;

namespace MovieTicketBooking.Repositories
{
    public class MovieRepository : BaseRepository<Movie>, IMovieRepository
    {
        public MovieRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
            
        }
        public async Task<IEnumerable<MovieListDTO>> GetAllMoviesAsync()
        {
            //Select trong LINQ không load hết rồi mới lọc, mà nó chuyển thành SQL ngay từ đầu!
            return await _context.Movies
                .Include(m => m.MovieType)
                .Include(m => m.AppRating)
                //.AsNoTracking() // Tăng hiệu suất nếu chỉ đọc dữ liệu, dùng select rồi thì ko cần asnotracking nữa
                .Select(m => new MovieListDTO
                {
                    Id = m.Id,
                    MovieName = m.MovieName,
                    ImageUrl = m.ImageUrl,
                    Rating = m.AppRating.Rating
                })
                .ToListAsync();
        }

        public async Task<MovieDetailDTO?> GetMovieByIdAsync(int movieId)
        {
            return await _context.Movies
                .Include(m => m.MovieType)
                .Include(m => m.AppRating)
                .Where(m => m.Id == movieId)
                .Select(m => new MovieDetailDTO
                {
                    Id = movieId,
                    MovieName = m.MovieName,
                    ImageUrl = m.ImageUrl,
                    Rating = m.AppRating.Rating,
                    MovieType = m.MovieType.TypeName,
                    Director = m.Director,
                    Actor = m.Actor
                })
                .FirstOrDefaultAsync();
        }
    }
}
