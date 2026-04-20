using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.AiDtos
{
    public class ResumeBuilderRQ
    {
        /// <summary>Target job title to tailor the resume for (optional)</summary>
        [StringLength(200)]
        public string? TargetJobTitle { get; set; }

        /// <summary>Resume style: "Professional", "Modern", "ATS-Friendly", "Creative"</summary>
        [StringLength(50)]
        public string Style { get; set; } = "Professional";

        /// <summary>Language for the resume: "English" or "Arabic"</summary>
        [StringLength(20)]
        public string Language { get; set; } = "English";

        /// <summary>Additional notes or focus areas for the resume</summary>
        [StringLength(500)]
        public string? AdditionalNotes { get; set; }
    }

    public class ResumeBuilderRS
    {
        public string FullName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Location { get; set; }
        public string ProfessionalSummary { get; set; } = string.Empty;
        public List<ResumeSkillSection> SkillSections { get; set; } = new();
        public List<ResumeExperience> Experience { get; set; } = new();
        public List<ResumeEducation> Education { get; set; } = new();
        public List<string> Certifications { get; set; } = new();
        public string? AdditionalSections { get; set; }
        public string FullResumeText { get; set; } = string.Empty;
        public string AITips { get; set; } = string.Empty;
    }

    public class ResumeSkillSection
    {
        public string Category { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
    }

    public class ResumeExperience
    {
        public string Position { get; set; } = string.Empty;
        public string Company { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public List<string> BulletPoints { get; set; } = new();
    }

    public class ResumeEducation
    {
        public string Degree { get; set; } = string.Empty;
        public string Institution { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? FieldOfStudy { get; set; }
    }
}
