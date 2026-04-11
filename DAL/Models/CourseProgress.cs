using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class CourseProgress
    {
        [Key]
        public int ProgressId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;

        public DateTime? CompletedAt { get; set; }
        public double ProgressPercentage { get; set; } = 0;
        public int CompletedLessons { get; set; } = 0;

        [StringLength(50)]
        public string Status { get; set; } = "Not Started";

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}