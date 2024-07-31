using Domain.Entities;
using Domain.Enums;
using Domain.ResponeModel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repository.BaseRepository
{
    public interface INotificationRepository:BaseRepository<NotificationCollection>
    {
        public Task AddNotification(string Id, Notification notification);
        public  Task<IEnumerable<NotificationResponseModel>> GetNotification(string Id, int skip, int limit);
        public  Task<UpdateResult> RemoveNotification(string Id, string NotificationId);
        public Task RemoveNotification(string Id, string FromId, NotificationType type);
       
    }
}
