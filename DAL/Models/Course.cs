using DAL.Helper.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
        public string  Name { get; set; }
        public string? Description { get; set; }
        public string? Instructor { get; set; }
        public string ExternalUrl { get; set; }
        public decimal? Price { get; set; }
        public bool IsFree { get; set; } = true;
        public decimal? DurationHours { get; set; }
        public string? DifficultyLevel { get; set; }
        public float? Rating { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int TotalLessons { get; set; } = 1;
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }

        public int? SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }
        public int PlatformId { get; set; }
        [ForeignKey("PlatformId")]
        public CoursePlatform Platform { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastUpdatedAt { get; set; }
        public string ?BadgeUrl { get; set; }
        public virtual ICollection<CourseProgress> CourseProgresses { get; set; } = new List<CourseProgress>();
        public virtual ICollection<CourseSkill> CourseSkills { get; set; } = new List<CourseSkill>();
        public ICollection<CareerPathCourse> CareerPathCourses { get; set; } = new List<CareerPathCourse>();

    }
}
