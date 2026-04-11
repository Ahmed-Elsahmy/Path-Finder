using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseProgressDtos
{
    public class CourseProgressRS
    {
        public int ProgressId { get; set; }
        public int CourseId { get; set; }
        public string CourseName { get; set; }
        public string? CourseThumbnailUrl { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int CompletedLessons { get; set; }
        public int TotalLessons { get; set; }
        public int ProgressPercentage { get; set; }
        public string Status { get; set; }
        public string? Notes { get; set; }
    }
}
