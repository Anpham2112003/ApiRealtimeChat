using Domain.Entites;
using Domain.Entities;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IMessageRepository:BaseRepository<GroupCollection>
    {
        public  Task<MesssageCollection?> GetLastMessageCollection(ObjectId GroupId);
        public Task AddMessageToPage(ObjectId GroupId, int Page,int Count,Message message);
    }
}
