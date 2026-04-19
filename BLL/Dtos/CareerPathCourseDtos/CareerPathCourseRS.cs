using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CareerPathCourseDtos
{
    public class CareerPathCourseRS
    {
        public int CareerPathCourseId { get; set; }
        public int CareerPathId { get; set; }
        public string CareerPathName { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public int OrderNumber { get; set; }
        public bool IsRequired { get; set; }
        public string? CompletionCriteria { get; set; }
    }
}
