using StackExchange.Redis;

namespace MovieTicketBooking.Repositories.Interfaces
{
    public interface IRedisClient
    {
        Task<string?> GetAsync(string key);
        Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null);
        Task<bool> SetNotExistAsync(string key, string value, TimeSpan? expiry = null, When when = When.NotExists);
        Task RemoveAsync(string key);
        Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] key, RedisValue[] redisValue, CommandFlags commandFlags = CommandFlags.None);
    }
}
