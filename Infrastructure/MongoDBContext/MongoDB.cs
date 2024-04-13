using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Settings;
using MongoDB.Driver;
using Microsoft.Extensions.Options;

namespace Infrastructure.MongoDBContext
{
    public class MongoDBcontext:IMongoDB
    {
        private readonly IOptionsMonitor<MongoDbSetting> _options;
        private readonly MongoClient _client;
        public MongoDBcontext(IOptionsMonitor<MongoDbSetting> options)
        {
            _options = options;
            _client = new MongoClient(_options.CurrentValue.Conection);
        }

        public IMongoDatabase GetDataBase()
        {
           
            return _client.GetDatabase(_options.CurrentValue.Database);
        }

        public IMongoCollection<T> GetCollection<T>(string name)
        {
            try
            {
                var Database = GetDataBase();

                var Collection = Database.GetCollection<T>(name);

                return Collection;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        public MongoClient GetClient()
        {
            return _client;
        }
    }
}
