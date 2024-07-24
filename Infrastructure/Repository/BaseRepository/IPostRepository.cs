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
    public interface IPostRepository:BaseRepository<PostCollection>
    {
      
        public  Task<UpdateResult> InsertPost(string AccountId,  Post post);
        public  Task<UpdateResult> RemovePost(string AccountId, string PostId);
        public Task<UpdateResult> LikePost(string MyId, string AccountId,  string PostId);
        public Task<UpdateResult> UnLikePost(string MyId, string AccountId,  string PostId);
        public  Task<IEnumerable<PostResponseModel>> GetLastPostFromListFriend(string AccountId, int skip, int take);
        public  Task<IEnumerable<UserResponseModel>> GetListUserLikePost(string AccountId,  string PostId, int skip, int take);
        public  Task<IEnumerable<PostResponseModel>> GetPostById(string AccountId, int Skip, int Limit);
        
    }
}
