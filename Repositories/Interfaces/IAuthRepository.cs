using Microsoft.AspNetCore.Identity;
using MovieTicketBooking.DTOs;
using MovieTicketBooking.Entities;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IAuthRepository : IBaseRepository<User>
    {
        Task<IdentityResult> RegisterUserAsync(ApplicationUser user, string password);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(ApplicationUser user, string password);
    }
}
