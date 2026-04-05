using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.AiDtos
{
    /// <summary>Request for AI interview prep session</summary>
    public class InterviewPrepRQ
    {
        [Required(ErrorMessage = "Job title or role is required.")]
        [StringLength(200)]
        public string JobTitle { get; set; }

        [StringLength(50)]
        public string? Difficulty { get; set; } = "Intermediate"; // Beginner, Intermediate, Senior
    }
}
