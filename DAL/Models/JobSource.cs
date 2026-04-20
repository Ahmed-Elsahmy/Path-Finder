using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class JobSource
    {
        [Key]
        public int SourceId { get; set; }

        [Required]
        [StringLength(100)]
        public string SourceName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? SourceType { get; set; } // "API", "Scraper", "Manual"

        [StringLength(500)]
        public string? APIEndpoint { get; set; }

        public DateTime? LastSyncedAt { get; set; }

        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
