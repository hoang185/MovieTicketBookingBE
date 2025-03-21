using Microsoft.Extensions.Configuration;
using MovieTicketBooking.Common;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MovieTicketBooking.Services
{
    public class MovieService : IMovieService
    {
        private readonly ILogger<MovieService> _logger;
        private readonly IMovieRepository _movieRepository;
        private readonly IRedisClient _redisClient;
        private readonly TimeSpan _seatExpireTime;
        public MovieService(ILogger<MovieService> logger, IMovieRepository movieRepository, IRedisClient redisClient, IConfiguration configuration)
        {
            _logger = logger;
            _movieRepository = movieRepository;
            _redisClient = redisClient;
            _seatExpireTime = TimeSpan.FromMinutes(configuration.GetValue<int>("Redis:SeatExpireMinutes"));
        }

        public async Task<MovieDetailDTO?> GetMovieByIdAsync(int movieId)
        {
            return await _movieRepository.GetMovieByIdAsync(movieId);
        }

        public async Task<IEnumerable<MovieListDTO>> GetMoviesAsync()
        {
            return await _movieRepository.GetAllMoviesAsync();
        }

        public async Task<List<string>> LockSeatAsync(SeatSelectRequest seatSelectRequest)
        {
            try
            {
                var movieId = seatSelectRequest.MovieId;
                var cinemaId = seatSelectRequest.CinemaId;
                var date = seatSelectRequest.Date;
                var time = seatSelectRequest.Time;
                var userId = seatSelectRequest.UserId;
                var failedSeats = new Dictionary<string, string>();
                var lockedSeats = new Dictionary<string, string>();

                var seatKeys = seatSelectRequest.SeatIds.ToDictionary(x => x, x => Utility.CreateSeatKey(movieId, cinemaId, date, time, x));

                foreach (var seatKey in seatKeys)
                {
                    //check xem ghế đã được giữ chỗ chưa
                    var isSeatEmpty = string.IsNullOrEmpty(await _redisClient.GetAsync(seatKey.Value));
                    if (!isSeatEmpty)
                    {
                        failedSeats.Add(seatKey.Key, seatKey.Value);
                        continue;
                    }

                    //nếu ghế chưa có ai giữ thì thực hiện giữ chỗ cho user hiện tại
                    var isSuccess = await _redisClient.SetNotExistAsync(seatKey.Value, userId, _seatExpireTime, When.NotExists);

                    if (isSuccess)
                    {
                        lockedSeats.Add(seatKey.Key, seatKey.Value);
                    }
                    else
                    {
                        failedSeats.Add(seatKey.Key, seatKey.Value);
                    }
                }

                if (failedSeats.Any())
                {
                    foreach (var seatKey in lockedSeats)
                    {
                        await ReleaseSeatAsync(seatKey.Value, userId);
                    }
                }

                return failedSeats.Select(x => x.Key).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task<string?> GetSeatOwnerAsync(string seatKey)
        {
            return await _redisClient.GetAsync(seatKey);
        }
        public async Task ReleaseSeatAsync(string seatKey, string userId)
        {
            var currentOwner = await _redisClient.GetAsync(seatKey);
            if (currentOwner == userId) // Chỉ xóa nếu đúng user
            {
                await _redisClient.RemoveAsync(seatKey);
            }
        }
    }
}
