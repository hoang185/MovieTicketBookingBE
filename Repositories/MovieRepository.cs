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
        public async Task<IEnumerable<MovieDto>> GetAllMoviesAsync()
        {
            //Select trong LINQ không load hết rồi mới lọc, mà nó chuyển thành SQL ngay từ đầu!
            return await _context.Movies
                .AsNoTracking() // Tăng hiệu suất nếu chỉ đọc dữ liệu
                .Select(m => new MovieDto
                {
                    MovieName = m.MovieName,
                    Director = m.Director,
                    Actor = m.Actor,
                    MovieType = m.MovieType.TypeName // Chỉ lấy tên thể loại
                })
                .ToListAsync();
        }
    }
}
