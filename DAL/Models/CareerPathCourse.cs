using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class CareerPathCourse
    {
        public int CareerPathCourseId { get; set; }
        [Required]
        public int CareerPathId { get; set; }
        [ForeignKey("CareerPathId")]
        public CareerPath CareerPath { get; set; }
        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public Course Course { get; set; }
        public int OrderNumber { get; set; }
        public bool IsRequired { get; set; } = true;
        [MaxLength(500)]
        public string? CompletionCriteria { get; set; }

    }
}
