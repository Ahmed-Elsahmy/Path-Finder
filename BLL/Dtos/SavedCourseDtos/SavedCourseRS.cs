using DAL.Models;

namespace BLL.Dtos.SavedCourseDtos
{
    public class SavedCourseRS
    {
        public int SavedCourseId { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public DateTime SavedAt { get; set; }
        public string? Notes { get; set; }
    }
}
