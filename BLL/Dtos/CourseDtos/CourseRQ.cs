using DAL.Helper.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseDtos
{
    public class CourseRQ
    {
        [Required(ErrorMessage ="Please Add Course Name..")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Course Name must be between 3 and 200 characters.")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? Instructor { get; set; }

        [Required(ErrorMessage ="Please Add Course Link")]
        [DataType(DataType.Url, ErrorMessage = "Please enter a valid URL.")]
        public string ExternalUrl { get; set; }
        [DataType(DataType.Currency)]
        public decimal? Price { get; set; }
        public decimal? DurationHours { get; set; }
        public int TotalLessons { get; set; } = 1;
        public string? DifficultyLevel { get; set; }
        public float? Rating { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        [Required(ErrorMessage ="Please Chosse Platform")]
        public int PlatformId { get; set; }

        public string? ThumbnailFile { get; set; } 
    }
}
