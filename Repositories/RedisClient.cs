using MovieTicketBooking.Repositories.Interfaces;
using StackExchange.Redis;

namespace MovieTicketBooking.Repositories
{
    public class RedisClient : IRedisClient
    {
        private readonly IDatabase _db;

        public RedisClient(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<string?> GetAsync(string key)
        {
            return await _db.StringGetAsync(key);
        }

        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            return await _db.StringSetAsync(key, value, expiry);
        }

        public async Task RemoveAsync(string key)
        {
            await _db.KeyDeleteAsync(key);
        }

        public async Task<bool> SetNotExistAsync(string key, string value, TimeSpan? expiry = null, When when = When.NotExists)
        {
            return await _db.StringSetAsync(key, value, expiry, when);
        }

        public async Task<RedisResult> ScriptEvaluateAsync(string script, RedisKey[] key, RedisValue[] redisValue, CommandFlags commandFlags = CommandFlags.None)
        {
            return await _db.ScriptEvaluateAsync(script, key, redisValue, commandFlags);
        }
    }
}
