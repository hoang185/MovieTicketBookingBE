namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IRedisClient
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value, TimeSpan? expiry = null);
        Task RemoveAsync(string key);
    }
}
