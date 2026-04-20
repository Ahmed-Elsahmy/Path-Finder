namespace BLL.Dtos.CourseDtos
{
    public class CourseRecommendationRS
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PlatformName { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? DifficultyLevel { get; set; }
        public bool IsFree { get; set; }
        public decimal? Price { get; set; }
        public string? ExternalUrl { get; set; }
        public string AIRecommendationReason { get; set; } = string.Empty;
        public int MatchScore { get; set; } // 0-100 how relevant to user
        public List<string> SkillsYouWillLearn { get; set; } = new();
    }

    public class CourseRecommendationListRS
    {
        public List<CourseRecommendationRS> Recommendations { get; set; } = new();
        public string OverallAdvice { get; set; } = string.Empty;
    }
}
