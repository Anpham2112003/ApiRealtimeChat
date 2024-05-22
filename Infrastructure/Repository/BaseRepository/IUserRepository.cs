using Domain.Entities;
using Domain.Enums;
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
        public  Task ChangeStateUserAsync(string AccountId, UserState state);
        Task<UserCollection?> FindUserByAccountId(string id);
    }
}
