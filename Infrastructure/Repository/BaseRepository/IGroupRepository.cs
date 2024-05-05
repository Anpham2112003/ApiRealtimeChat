using Domain.Entites;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface IGroupRepository:BaseRepository<GroupCollection>
    {
        public Task CreateGroup(ObjectId UserId,string GroupName);
        public Task AddMemberToGroup(ObjectId GroupId, ObjectId UserId);
    }
}
