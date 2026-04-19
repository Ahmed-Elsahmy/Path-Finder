using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Helper.Enums;

namespace DAL.Models
{
    public class UserCareerPath
    {
        [Key]
        public int UserCareerPathId { get; set; }
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [Required]
        public int CareerPathId { get; set; }
        [ForeignKey("CareerPathId")]
        public virtual CareerPath CareerPath { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
        public CareerPathStatus Status { get; set; } = CareerPathStatus.InProgress;
        [Range(0, 100)]
        public int ProgressPercentage { get; set; } = 0;
        public DateTime? CompletedAt { get; set; }
        public string? AIRecommendationReason { get; set; }

    }
}
