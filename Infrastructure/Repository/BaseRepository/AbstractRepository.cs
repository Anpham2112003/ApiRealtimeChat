using Domain.Entites;
using Infrastructure.MongoDBContext;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public abstract class AbstractRepository<ICollection> :BaseRepository<ICollection> where ICollection : BaseCollection
    {
        private readonly IMongoDB _mongoDB;
        protected readonly IMongoCollection<ICollection> _collection;
        protected AbstractRepository(IMongoDB mongoDB)
        {
            _mongoDB = mongoDB;
            _collection = _mongoDB.GetCollection<ICollection>(nameof(ICollection));
        }

        public  async Task<IAsyncCursor<ICollection>> FindAsync(FilterDefinition<ICollection> filter)
        {
            var result = await _collection.FindAsync(filter);  
            
            return result;
        }

        public  async Task InsertAsync(ICollection collection)
        {
            await _collection.InsertOneAsync(collection);
        }

        public  async Task RemoveAsync(FilterDefinition<ICollection> filter)
        {
            await _collection.FindOneAndDeleteAsync(filter);
        }

        public  async Task UpdateAsync(FilterDefinition<ICollection> filter, BsonDocument elements)
        {
            await _collection.FindOneAndUpdateAsync(filter, elements);
        }
    }
}
