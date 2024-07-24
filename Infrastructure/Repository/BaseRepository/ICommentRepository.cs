using Domain.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface ICommentRepository:BaseRepository<CommentCollection>
    {
        public  Task RemoveCommentCollection(string PostId);
        public  Task<UpdateResult> PushComment(string PostId, Comment comment);
        public  Task<UpdateResult> RepComment(string PostId, string ParentId, Comment comment);
    }
}
