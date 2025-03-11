using Microsoft.AspNetCore.Identity;

namespace MovieTicketBooking.DTOs
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = "";
    }
}
