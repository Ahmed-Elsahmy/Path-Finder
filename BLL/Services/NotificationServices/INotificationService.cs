using BLL.Common;
using BLL.Dtos.NotificationDtos;

namespace BLL.Services.NotificationServices
{
    public interface INotificationService
    {
        Task<ServiceResult<List<NotificationRS>>> GetUserNotificationsAsync(string userId, bool unreadOnly = false, int take = 50);
        Task<ServiceResult<int>> GetUnreadCountAsync(string userId);
        Task<ServiceResult<NotificationRS>> MarkAsReadAsync(string userId, int notificationId);
        Task<ServiceResult<string>> MarkAllAsReadAsync(string userId);

        Task<ServiceResult<NotificationRS>> CreateAsync(CreateNotificationRQ request);

        Task<ServiceResult<NotificationRS>> CreateForUserAsync(
            string userId,
            string notificationType,
            string title,
            string? message = null,
            string? relatedEntityType = null,
            int? relatedEntityId = null);

        Task<ServiceResult<int>> CreateForUsersAsync(
            IEnumerable<string> userIds,
            string notificationType,
            string title,
            string? message = null,
            string? relatedEntityType = null,
            int? relatedEntityId = null);
    }
}

