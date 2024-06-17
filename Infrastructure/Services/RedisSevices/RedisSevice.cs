using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.RedisSevices
{
    public class RedisSevice:IRedisService
    {
        private readonly IDatabase _database;

        public RedisSevice(IConnectionMultiplexer connection)
        {
           _database=connection.GetDatabase();
        }

        public async Task SetHashValueToRedis(string key,HashEntry[] hashes)
        {
            try
            {
                await _database.HashSetAsync(new RedisKey(key), hashes);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        

        public async Task<RedisValue[]?> GetHashValues(string key, RedisValue[] fields)
        {
            try
            {
                
                return await _database.HashGetAsync(new RedisKey(key), fields);

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveHash(string key)
        {
            try
            {
                var allkey =await _database.HashKeysAsync(new RedisKey(key));

                await _database.HashDeleteAsync(key, allkey);
            }
            catch (Exception)
            {

                throw;
            }
        }

        
    }
}
