using Domain.Entites;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IUserRepository:BaseRepository<UserCollection>
    {
        Task InsertUserAsync(UserCollection user);

        Task<UserCollection?> FindUserByAccountId(ObjectId id);
    }
}
