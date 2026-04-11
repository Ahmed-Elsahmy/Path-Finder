using DAL.Helper.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseDtos
{
    public class CourseFilterRQ
    {
        public string? SearchTerm { get; set; }
        public int? PlatformId { get; set; }
        public bool? IsFreeOnly { get; set; }
        public string? DifficultyLevel { get; set; }

        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
    }
}
