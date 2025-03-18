using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;

namespace MovieTicketBooking.Common
{
    public static class Utility
    {
        public static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }
        public static string? GetJtiFromToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            // Lấy JTI từ claims
            return jwtToken?.Claims.FirstOrDefault(c => c.Type == "jti")?.Value;
        }

        public static string? GetJtiFromRequestCookies(HttpContext httpContext)
        {
            if (httpContext.Request.Cookies.TryGetValue(Constant.JWT_TOKEN_NAME, out var token))
            {
                return GetJtiFromToken(token);
            }
            return null;
        }
    }
}
