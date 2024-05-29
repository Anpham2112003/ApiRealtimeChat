using Domain.Ultils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Errors
{
    public static class  FriendError
    {
        public static Error FriendNotFound(string id) 

            => new Error("FriendError.FriendIdNotFound", $"Friend with Id :{id} not exist!");
        public static Error DocumentNotFound

            => new Error("FriendError.DocumentNorFound", $"  FriendId or AccountId was not Exists !");

        public static Error NotFoundInWaitList
            => new Error("NotFound", "Not found in wait list!");
      
    }
}
