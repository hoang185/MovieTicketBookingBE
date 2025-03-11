using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.Common;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;
using MovieTicketBooking.Repositories.Interfaces;
using MovieTicketBooking.Services.Interfaces;
using System.Text;

namespace MovieTicketBooking.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IAuthRepository _authRepository;

        public AuthService(ILogger<AuthService> logger, IAuthRepository userRepository)
        {
            _authRepository = userRepository;
            _logger = logger;
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
    }
}
