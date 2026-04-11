using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseProgressDtos
{
    public class EnrollCourseRQ
    {
        [Required]
        public int CourseId { get; set; }
    }
}
