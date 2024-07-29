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
        public  Task<BulkWriteResult> RepComment(string AccountId,string PostId, string ParentId, Comment comment);
        public  Task<UpdateResult> BlockComment(string AccountId, string PostId);
        public Task<UpdateResult> UnBlockComment(string AccountId, string PostId);
        public  Task<UpdateResult> HiddenComment(string AccountId, string PostId);
        public  Task<UpdateResult> UnHiddenComment(string AccountId, string PostId);
        public  Task<IEnumerable<CommentResponseModel>> GetCommentPost(string AccountId, string PostId, int skip, int limit);
        public  Task<IEnumerable<CommentResponseModel>> GetRepComment(string AccountId, string PostId, string ParentId, int skip, int limit);
        public  Task<BulkWriteResult> RemoveComment(string AccountId, string MyId, string PostId, string CommentId, string ParentId);
    }
}
