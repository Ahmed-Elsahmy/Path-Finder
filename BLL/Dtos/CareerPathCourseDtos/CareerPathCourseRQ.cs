using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CareerPathCourseDtos
{
    public class CareerPathCourseRQ
    {
        [Required]
        public int CareerPathId { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public int OrderNumber { get; set; }
        public bool IsRequired { get; set; } = true;
        [MaxLength(500)]
        public string? CompletionCriteria { get; set; }
    }
}
