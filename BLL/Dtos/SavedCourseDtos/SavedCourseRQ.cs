using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.SavedCourseDtos
{
    public class SavedCourseRQ
    {
        [Required(ErrorMessage = "Course ID is required.")]
        public int CourseId { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
