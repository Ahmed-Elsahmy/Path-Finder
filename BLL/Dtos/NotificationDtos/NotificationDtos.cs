using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.NotificationDtos
{
    public class NotificationRS
    {
        public int NotificationId { get; set; }
        public string NotificationType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsRead { get; set; }
        public string? RelatedEntityType { get; set; }
        public int? RelatedEntityId { get; set; }
    }

    public class CreateNotificationRQ
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string NotificationType { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000)]
        public string? Message { get; set; }

        [StringLength(100)]
        public string? RelatedEntityType { get; set; }

        public int? RelatedEntityId { get; set; }
    }
}

