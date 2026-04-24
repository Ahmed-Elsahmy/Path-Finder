using BLL.Dtos.NotificationDtos;
using BLL.Services.NotificationServices;
using Microsoft.AspNetCore.SignalR;
using Path_Finder.Hubs;

namespace Path_Finder.Realtime
{
    public class SignalRNotificationPublisher : INotificationPublisher
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public SignalRNotificationPublisher(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task PublishToUserAsync(string userId, NotificationRS notification)
        {
            return _hubContext.Clients.Group(userId).SendAsync("notificationReceived", notification);
        }
    }
}

