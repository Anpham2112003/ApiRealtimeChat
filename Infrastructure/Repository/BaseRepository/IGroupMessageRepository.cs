using Domain.Entites;
using Domain.Entities;
using Domain.ResponeModel.BsonConvert;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IGroupMessageRepository:BaseRepository<GroupCollection>
    {
        public  Task<MesssageCollection?> GetLastMessageCollection(ObjectId GroupId);
        public Task AddMessageToPage(ObjectId GroupId, int Page,int Count,Message message);
        public Task AddMessageCollection(ObjectId GroupId, int Page, Message message);
        public  Task<ResultQueyMessage> GetMessageAsync(ObjectId GroupId, int page, int skip, int limit);
        public  Task<UpdateResult> RemoveMessageAsync(ObjectId GroupId, int Page, ObjectId MessageId);
        public Task UpdateContentMessageAsync(ObjectId GroupId, int Page , ObjectId MessageId, string Content);
    }
}
