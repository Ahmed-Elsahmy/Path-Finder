using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseProgressDtos
{
    public class UpdateProgressRQ
    {
        [Required(ErrorMessage = "Newly completed lessons are required.")]
        public int NewlyCompletedLessons { get; set; }
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string? Notes { get; set; }
    }
}
