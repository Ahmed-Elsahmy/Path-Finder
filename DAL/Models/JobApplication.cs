using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class JobApplication
    {
        [Key]
        public int ApplicationId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [Required]
        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null!;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Applied"; // Applied, Interviewing, Rejected, Accepted

        public float? MatchPercentage { get; set; } // 0-100, AI calculated

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(2000)]
        public string? SkillGapAnalysis { get; set; } // AI generated
    }
}
