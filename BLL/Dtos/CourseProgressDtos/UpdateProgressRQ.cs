using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.CourseProgressDtos
{
    public class UpdateProgressRQ
    {
        [Required(ErrorMessage = "Completed lessons count is required.")]
        [Range(0, 10000, ErrorMessage = "Lessons count cannot be negative.")]
        public int CompletedLessons { get; set; } // 🟢 تم التعديل هنا[StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
    }
}