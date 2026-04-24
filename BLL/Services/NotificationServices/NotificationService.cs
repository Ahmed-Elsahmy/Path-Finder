using AutoMapper;
using BLL.Common;
using BLL.Dtos.NotificationDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.NotificationServices
{
    public class NotificationService : INotificationService
    {
        private readonly IRepository<Notification> _notificationRepo;
        private readonly INotificationPublisher _publisher;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IRepository<Notification> notificationRepo,
            INotificationPublisher publisher,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _notificationRepo = notificationRepo;
            _publisher = publisher;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<NotificationRS>>> GetUserNotificationsAsync(string userId, bool unreadOnly = false, int take = 50)
        {
            try
            {
                if (take <= 0) take = 50;
                take = Math.Clamp(take, 1, 200);

                var query = _notificationRepo.Query()
                    .Where(n => n.UserId == userId);

                if (unreadOnly)
                    query = query.Where(n => !n.IsRead);

                var items = await query
                    .OrderByDescending(n => n.CreatedAt)
                    .Take(take)
                    .ToListAsync();

                return ServiceResult<List<NotificationRS>>.Success(_mapper.Map<List<NotificationRS>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving notifications for user {UserId}", userId);
                return ServiceResult<List<NotificationRS>>.Failure("Error retrieving notifications.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<int>> GetUnreadCountAsync(string userId)
        {
            try
            {
                var count = await _notificationRepo.Query()
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .CountAsync();

                return ServiceResult<int>.Success(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unread notification count for user {UserId}", userId);
                return ServiceResult<int>.Failure("Error retrieving unread count.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<NotificationRS>> MarkAsReadAsync(string userId, int notificationId)
        {
            try
            {
                var notification = await _notificationRepo.Query()
                    .FirstOrDefaultAsync(n => n.NotificationId == notificationId && n.UserId == userId);

                if (notification == null)
                    return ServiceResult<NotificationRS>.Failure("Notification not found.", ServiceErrorCode.NotFound);

                if (!notification.IsRead)
                {
                    notification.IsRead = true;
                    _notificationRepo.Update(notification);
                    await _notificationRepo.SaveChangesAsync();
                }

                return ServiceResult<NotificationRS>.Success(_mapper.Map<NotificationRS>(notification));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking notification {NotificationId} as read for user {UserId}", notificationId, userId);
                return ServiceResult<NotificationRS>.Failure("Error marking notification as read.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<string>> MarkAllAsReadAsync(string userId)
        {
            try
            {
                var unread = await _notificationRepo.Query()
                    .Where(n => n.UserId == userId && !n.IsRead)
                    .ToListAsync();

                if (!unread.Any())
                    return ServiceResult<string>.Success("No unread notifications.");

                foreach (var n in unread)
                    n.IsRead = true;

                await _notificationRepo.SaveChangesAsync();
                return ServiceResult<string>.Success("All notifications marked as read.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking all notifications as read for user {UserId}", userId);
                return ServiceResult<string>.Failure("Error marking all notifications as read.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public Task<ServiceResult<NotificationRS>> CreateAsync(CreateNotificationRQ request)
        {
            return CreateForUserAsync(
                request.UserId,
                request.NotificationType,
                request.Title,
                request.Message,
                request.RelatedEntityType,
                request.RelatedEntityId);
        }

        public async Task<ServiceResult<NotificationRS>> CreateForUserAsync(
            string userId,
            string notificationType,
            string title,
            string? message = null,
            string? relatedEntityType = null,
            int? relatedEntityId = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(userId))
                    return ServiceResult<NotificationRS>.Failure("UserId is required.");

                if (string.IsNullOrWhiteSpace(notificationType))
                    return ServiceResult<NotificationRS>.Failure("NotificationType is required.");

                if (string.IsNullOrWhiteSpace(title))
                    return ServiceResult<NotificationRS>.Failure("Title is required.");

                var notification = new Notification
                {
                    UserId = userId,
                    NotificationType = notificationType.Trim(),
                    Title = title.Trim(),
                    Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
                    RelatedEntityType = string.IsNullOrWhiteSpace(relatedEntityType) ? null : relatedEntityType.Trim(),
                    RelatedEntityId = relatedEntityId,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                };

                await _notificationRepo.AddAsync(notification);
                await _notificationRepo.SaveChangesAsync();

                var dto = _mapper.Map<NotificationRS>(notification);

                try
                {
                    await _publisher.PublishToUserAsync(userId, dto);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Notification created but failed to publish in real-time (UserId={UserId}, NotificationId={NotificationId})", userId, notification.NotificationId);
                }

                return ServiceResult<NotificationRS>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notification for user {UserId}", userId);
                return ServiceResult<NotificationRS>.Failure("Error creating notification.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<int>> CreateForUsersAsync(
            IEnumerable<string> userIds,
            string notificationType,
            string title,
            string? message = null,
            string? relatedEntityType = null,
            int? relatedEntityId = null)
        {
            try
            {
                var users = userIds
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Select(u => u.Trim())
                    .Distinct()
                    .ToList();

                if (!users.Any())
                    return ServiceResult<int>.Success(0);

                if (string.IsNullOrWhiteSpace(notificationType))
                    return ServiceResult<int>.Failure("NotificationType is required.");

                if (string.IsNullOrWhiteSpace(title))
                    return ServiceResult<int>.Failure("Title is required.");

                var notifications = users.Select(userId => new Notification
                {
                    UserId = userId,
                    NotificationType = notificationType.Trim(),
                    Title = title.Trim(),
                    Message = string.IsNullOrWhiteSpace(message) ? null : message.Trim(),
                    RelatedEntityType = string.IsNullOrWhiteSpace(relatedEntityType) ? null : relatedEntityType.Trim(),
                    RelatedEntityId = relatedEntityId,
                    CreatedAt = DateTime.UtcNow,
                    IsRead = false
                }).ToList();

                await _notificationRepo.AddRangeAsync(notifications);
                await _notificationRepo.SaveChangesAsync();

                var publishTasks = notifications.Select(n =>
                {
                    var dto = _mapper.Map<NotificationRS>(n);
                    return SafePublishAsync(n.UserId, dto);
                });

                await Task.WhenAll(publishTasks);

                return ServiceResult<int>.Success(notifications.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating notifications for multiple users");
                return ServiceResult<int>.Failure("Error creating notifications.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        private async Task SafePublishAsync(string userId, NotificationRS dto)
        {
            try
            {
                await _publisher.PublishToUserAsync(userId, dto);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to publish notification in real-time (UserId={UserId}, NotificationId={NotificationId})", userId, dto.NotificationId);
            }
        }
    }
}

