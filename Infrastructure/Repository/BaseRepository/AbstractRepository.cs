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
        
        protected  IMongoCollection<ICollection>? _collection { get; set; }
        protected AbstractRepository(IMongoDB mongoDB)
        {
            
            
        }

        public  async Task<IAsyncCursor<ICollection>?> FindAsync(FilterDefinition<ICollection> filter)
        {
            var result = await _collection.FindAsync(filter);  
            
            return result;
        }

        public  async Task InsertAsync(ICollection collection)
        {
            await _collection!.InsertOneAsync(collection);
        }

        public  async Task RemoveAsync(FilterDefinition<ICollection> filter)
        {
            await _collection.FindOneAndDeleteAsync(filter);
        }

        public async Task<UpdateResult> UpdateAsync(FilterDefinition<ICollection> filter, UpdateDefinition<ICollection> update)
        {
            var result = await _collection!.UpdateOneAsync(filter, update);
            
            return result;
        }

        public async Task FindOneAndUpdateAsync(FilterDefinition<ICollection> filter, BsonDocument elements)
        {
            await _collection.FindOneAndUpdateAsync(filter, elements);
        }

        public async Task<UpdateResult> UpdateAsync(FilterDefinition<ICollection> filter, UpdateDefinition<ICollection> update, UpdateOptions options)
        {
            return await _collection!.UpdateOneAsync(filter,update,options);
        }
    }
}
