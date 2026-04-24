using BLL.Dtos.NotificationDtos;

namespace BLL.Services.NotificationServices
{
    public interface INotificationPublisher
    {
        Task PublishToUserAsync(string userId, NotificationRS notification);
    }
}

