using Domain.Entites;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface BaseRepository<TCollection> where TCollection:BaseCollection
    {
        public Task InsertAsync(TCollection collection);
        public  Task<IAsyncCursor<TCollection>?> FindAsync(FilterDefinition<TCollection> filter);
        public  Task RemoveAsync(FilterDefinition<TCollection> filter);
        public Task<UpdateResult> UpdateAsync(FilterDefinition<TCollection> filter, UpdateDefinition<TCollection> update);
        public Task FindOneAndUpdateAsync(FilterDefinition<TCollection> filter, BsonDocument elements);
        public Task<UpdateResult> UpdateAsync(FilterDefinition<TCollection> filter, UpdateDefinition<TCollection> update, UpdateOptions options);
    }
}
