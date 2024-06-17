using Domain.Entities;
using Domain.Enums;
using Domain.ResponeModel;
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
        public  Task UpdateAvatarUser(string AccountId, string avatarUrl);
        public  Task UpdateProfileUser(string AccountId, BsonDocument document);
        public  Task RemoveAvatarUser(string AccountId);
        public  Task<List<UserConvert>> SearchUser(string name);
        public Task<ViewProfileResponeModel?> ViewProfileUser(string Id, string UserId);
    }
}
