using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ResponeModel
{
    public class UploadAvatarResponeModel
    {
        public string? Message { get; set; }
        public string? AccountId { get; set; }
        public string? AvatarUrl { get; set; }

        public UploadAvatarResponeModel(string? message, string? accountId, string? avatarUrl)
        {
            Message = message;
            AccountId = accountId;
            AvatarUrl = avatarUrl;
        }
    }
}
