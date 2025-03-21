using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MovieTicketBooking.Common;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace MovieTicketBooking.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IAuthRepository _authRepository;
        private readonly IConfiguration _configuration;
        private readonly IRedisClient _redisClient;

        public AuthService(ILogger<AuthService> logger, IAuthRepository userRepository, IConfiguration configuration, IRedisClient redisClient)
        {
            _authRepository = userRepository;
            _logger = logger;
            _configuration = configuration;
            _redisClient = redisClient;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest loginDto)
        {
            try
            {
                var user = await _authRepository.GetUserByEmailAsync(loginDto.Email);
                if (user == null || !await _authRepository.CheckPasswordAsync(user, loginDto.Password))
                    return null;

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // Tạo `jti` duy nhất
                };

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(1).AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpiresInMinutes"])),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"]
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);
                return new LoginResponse { Token = tokenHandler.WriteToken(token), UserId = user.Id };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task AddTokenToBlacklistAsync(string token, TimeSpan expiryTimeSpan)
        {
            await _redisClient.SetAsync(token, "1", expiryTimeSpan);
        }

        public async Task<IdentityResult> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var user = new ApplicationUser
                {
                    UserName = request.Email,
                    Email = request.Email,
                    FullName = request.FullName
                };

                return await _authRepository.RegisterUserAsync(user, request.Password);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }

        public async Task<bool> IsTokenInBlacklistAsync(string tokenKey)
        {
            var token = await _redisClient.GetAsync(tokenKey);
            if (string.IsNullOrEmpty(token))
            {
                return false;
            }
            else return true;
        }
    }
}
