using BLL.Dtos.CareerPathCourseDtos;
using DAL.Helper.Enums;

namespace BLL.Dtos.CareerPathDtos
{
    public class CareerPathRS
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DifficultyLevel? DifficultyLevel { get; set; }
        public int? DurationInMonths { get; set; }
        public string? Prerequisites { get; set; }
        public string? ExpectedOutcomes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalCourses { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public List<CareerPathCourseRS> Courses { get; set; } = new();
    }
}
