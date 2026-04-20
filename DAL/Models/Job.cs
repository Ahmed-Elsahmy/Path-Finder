using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Job
    {
        [Key]
        public int JobId { get; set; }

        [Required]
        public int SourceId { get; set; }

        [ForeignKey("SourceId")]
        public virtual JobSource Source { get; set; } = null!;

        [Required]
        [StringLength(300)]
        public string JobTitle { get; set; } = string.Empty;

        [StringLength(255)]
        public string? CompanyName { get; set; }

        public string? Description { get; set; } // nvarchar(max)

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(50)]
        public string? JobType { get; set; } // Full-time, Part-time, Contract, Remote

        [StringLength(50)]
        public string? ExperienceLevel { get; set; } // Entry, Mid, Senior

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SalaryMin { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? SalaryMax { get; set; }

        [StringLength(500)]
        public string? ExternalUrl { get; set; }

        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ExpiryDate { get; set; }

        public bool IsActive { get; set; } = true;

        [StringLength(500)]
        public string? ContactInfo { get; set; }

        // Navigation
        public virtual ICollection<JobSkillRequirement> SkillRequirements { get; set; } = new List<JobSkillRequirement>();
        public virtual ICollection<JobApplication> Applications { get; set; } = new List<JobApplication>();
        public virtual ICollection<SavedJob> SavedByUsers { get; set; } = new List<SavedJob>();
    }
}
