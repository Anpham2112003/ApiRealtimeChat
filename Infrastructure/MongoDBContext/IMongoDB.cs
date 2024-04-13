using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.MongoDBContext
{
    public interface IMongoDB
    {
        public IMongoDatabase GetDataBase();
        public IMongoCollection<T> GetCollection<T>(string name);

        public MongoClient GetClient();
    }
}
