using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.DTOs;

namespace MovieTicketBooking.Services.Interfaces
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterAsync(RegisterRequest registerModel);
        Task<string?> LoginAsync(LoginRequest loginRequest);
    }
}
