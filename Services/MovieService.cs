using Microsoft.Extensions.Configuration;
using MovieTicketBooking.Common;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<bool> SaveSeatAsync(CheckoutRequest checkoutRequest)
        {
            try
            {
                var movieId = checkoutRequest.MovieId;
                var cinemaId = checkoutRequest.CinemaId;
                var date = checkoutRequest.Date;
                var time = checkoutRequest.Time;
                var userId = checkoutRequest.UserId;
                var seatKeys = checkoutRequest.SeatIds.ToDictionary(x => x, x => Utility.CreateSeatKey(movieId, cinemaId, date, time, x));
                TimeSpan expiryBookedSeat = TimeSpan.FromDays(7);
                // Lua script để kiểm tra và cập nhật ghế
                var luaScript = @"
            for i, seatKey in ipairs(KEYS) do
                local currentOwner = redis.call('GET', seatKey)
                if currentOwner == ARGV[1] then
                    redis.call('SET', seatKey, ARGV[2], 'PX', ARGV[3])
                else
                    return 0 -- Trả về 0 nếu bất kỳ ghế nào không thuộc userId
                end
            end
            return 1 -- Thành công nếu tất cả ghế đều thuộc về userId
        ";

                // Chuẩn bị tham số
                var keys = seatKeys.Values.Select(x => (RedisKey)x).ToArray();
                var args = new RedisValue[]
                {
            userId,
            Constant.BOOKED_SEAT,
            expiryBookedSeat.TotalMilliseconds
                };

                // Thực thi script trong Redis
                var result = (long)await _redisClient.ScriptEvaluateAsync(luaScript, keys, args);

                return result == 1;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
