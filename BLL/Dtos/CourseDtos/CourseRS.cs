using BLL.Dtos.CourseSkillDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseDtos
{
    public class CourseRS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Instructor { get; set; }
        public string ExternalUrl { get; set; }
        public decimal? Price { get; set; }
        public bool IsFree { get; set; }
        public decimal? DurationHours { get; set; }
        public string? DifficultyLevel { get; set; }
        public int TotalLessons { get; set; } 

        public float? Rating { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int PlatformId { get; set; }
        public string PlatformName { get; set; }
        public string? PlatformLogo { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdatedAt { get; set; }

        // 🟢 التعديل هنا:
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? SubCategoryId { get; set; }
        public string? SubCategoryName { get; set; }
        public List<CourseSkillRS> CourseSkills { get; set; } = new List<CourseSkillRS>();
    }
}
