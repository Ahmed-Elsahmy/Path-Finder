using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.JobDtos
{
    // ═══════════════════════════════════════════
    // JobSource DTOs
    // ═══════════════════════════════════════════

    public class JobSourceRQ
    {
        [Required(ErrorMessage = "Source name is required.")]
        [StringLength(100)]
        public string SourceName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? SourceType { get; set; }

        [StringLength(500)]
        public string? APIEndpoint { get; set; }

        public bool IsActive { get; set; } = true;
    }

    public class UpdateJobSourceRQ
    {
        [StringLength(100)]
        public string? SourceName { get; set; }

        [StringLength(50)]
        public string? SourceType { get; set; }

        [StringLength(500)]
        public string? APIEndpoint { get; set; }

        public bool? IsActive { get; set; }
    }

    public class JobSourceRS
    {
        public int SourceId { get; set; }
        public string SourceName { get; set; } = string.Empty;
        public string? SourceType { get; set; }
        public string? APIEndpoint { get; set; }
        public DateTime? LastSyncedAt { get; set; }
        public bool IsActive { get; set; }
        public int JobCount { get; set; }
    }

    // ═══════════════════════════════════════════
    // Job DTOs
    // ═══════════════════════════════════════════

    public class JobRQ
    {
        [Required(ErrorMessage = "Job title is required.")]
        [StringLength(300)]
        public string JobTitle { get; set; } = string.Empty;

        [Required(ErrorMessage = "Source ID is required.")]
        public int SourceId { get; set; }

        [StringLength(255)]
        public string? CompanyName { get; set; }

        public string? Description { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(50)]
        public string? JobType { get; set; }

        [StringLength(50)]
        public string? ExperienceLevel { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }

        [StringLength(500)]
        public string? ExternalUrl { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(500)]
        public string? ContactInfo { get; set; }
    }

    public class UpdateJobRQ
    {
        [StringLength(300)]
        public string? JobTitle { get; set; }

        [StringLength(255)]
        public string? CompanyName { get; set; }

        public string? Description { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [StringLength(50)]
        public string? JobType { get; set; }

        [StringLength(50)]
        public string? ExperienceLevel { get; set; }

        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }

        [StringLength(500)]
        public string? ExternalUrl { get; set; }

        public DateTime? ExpiryDate { get; set; }

        [StringLength(500)]
        public string? ContactInfo { get; set; }

        public bool? IsActive { get; set; }
    }

    public class JobRS
    {
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? JobType { get; set; }
        public string? ExperienceLevel { get; set; }
        public decimal? SalaryMin { get; set; }
        public decimal? SalaryMax { get; set; }
        public string? ExternalUrl { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsActive { get; set; }
        public string? ContactInfo { get; set; }

        // From navigation
        public string SourceName { get; set; } = string.Empty;
        public List<JobSkillRS> RequiredSkills { get; set; } = new();
    }

    public class JobSkillRS
    {
        public int SkillId { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? RequiredLevel { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class JobFilterRQ
    {
        public string? SearchTerm { get; set; }
        public string? Location { get; set; }
        public string? JobType { get; set; }
        public string? ExperienceLevel { get; set; }
        public int? SourceId { get; set; }
        public decimal? MinSalary { get; set; }
        public bool IsActive { get; set; }=true;
    }

    // ═══════════════════════════════════════════
    // JobApplication DTOs
    // ═══════════════════════════════════════════

    public class ApplyJobRQ
    {
        [Required(ErrorMessage = "Job ID is required.")]
        public int JobId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class JobApplicationRS
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public DateTime AppliedAt { get; set; }
        public string Status { get; set; } = "Applied";
        public float? MatchPercentage { get; set; }
        public string? Notes { get; set; }
        public string? SkillGapAnalysis { get; set; }
    }

    public class UpdateApplicationRQ
    {
        [StringLength(50)]
        public string? Status { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    // ═══════════════════════════════════════════
    // SavedJob DTOs
    // ═══════════════════════════════════════════

    public class SaveJobRQ
    {
        [Required(ErrorMessage = "Job ID is required.")]
        public int JobId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }

    public class SavedJobRS
    {
        public int SavedJobId { get; set; }
        public int JobId { get; set; }
        public string JobTitle { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? Location { get; set; }
        public string? JobType { get; set; }
        public DateTime SavedAt { get; set; }
        public string? Notes { get; set; }
    }
}

