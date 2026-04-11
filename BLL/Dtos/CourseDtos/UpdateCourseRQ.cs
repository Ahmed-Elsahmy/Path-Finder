using DAL.Helper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseDtos
{
    public class UpdateCourseRQ
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Instructor { get; set; }
        public string? ExternalUrl { get; set; }
        public decimal? Price { get; set; }
        public decimal? DurationHours { get; set; }
        public int TotalLessons { get; set; } = 1;
        public string? DifficultyLevel { get; set; }
        public float? Rating { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? PlatformId { get; set; }

        public string? ThumbnailUrl { get; set; } // رابط الصورة كنص عادي
    }

}
