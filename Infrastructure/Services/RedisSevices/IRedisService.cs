using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.RedisSevices
{
    public interface IRedisService
    {
        public  Task SetHashValueToRedis(string key, HashEntry[] hashes);
        public  Task<RedisValue[]?> GetHashValues(string key, RedisValue[] fields);
        public  Task RemoveHash(string key);
    }
}
