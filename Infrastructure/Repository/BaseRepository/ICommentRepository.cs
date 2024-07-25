using Domain.Entities;
using Domain.ResponeModel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface ICommentRepository:BaseRepository<PostCollection>
    {
      
        public  Task<UpdateResult> PushComment(string AccountId,string PostId, Comment comment);
        public  Task<UpdateResult> RepComment(string AccountId,string PostId, string ParentId, Comment comment);
        public  Task<UpdateResult> BlockComment(string AccountId, string PostId);
        public Task<UpdateResult> UnBlockComment(string AccountId, string PostId);
        public  Task<UpdateResult> HiddenComment(string AccountId, string PostId);
        public  Task<UpdateResult> UnHiddenComment(string AccountId, string PostId);


    }
}
