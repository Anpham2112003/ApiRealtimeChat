using Domain.Entites;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface BaseRepository<TCollection> where TCollection:BaseCollection
    {
        public Task InsertAsync(TCollection collection);

        

    }
}
