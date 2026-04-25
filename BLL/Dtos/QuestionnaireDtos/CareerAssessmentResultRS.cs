using DAL.Helper.Enums;

namespace BLL.Dtos.QuestionnaireDtos
{
    public class CareerAssessmentResultRS
    {
        public int QuestionnaireId { get; set; }
        public string QuestionnaireTitle { get; set; } = string.Empty;
        public DateTime EvaluatedAt { get; set; }
        public string ProfileSummary { get; set; } = string.Empty;
        public string RecommendationStrategy { get; set; } = string.Empty;
        public List<string> TopTraits { get; set; } = new();
        public List<CareerAssessmentResponseInsightRS> ResponseInsights { get; set; } = new();
        public List<CareerAssessmentRecommendationRS> Recommendations { get; set; } = new();
    }

    public class CareerAssessmentRecommendationRS
    {
        public int CareerPathId { get; set; }
        public string CareerPathName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CategoryName { get; set; }
        public string? SubCategoryName { get; set; }
        public DifficultyLevel? DifficultyLevel { get; set; }
        public int? DurationInMonths { get; set; }
        public int TotalCourses { get; set; }
        public int SuitabilityScore { get; set; }
        public string MatchReason { get; set; } = string.Empty;
        public string WhyItFits { get; set; } = string.Empty;
        public List<string> StrengthSignals { get; set; } = new();
        public List<string> GrowthAreas { get; set; } = new();
        public string SuggestedNextStep { get; set; } = string.Empty;
        public bool IsAlreadyEnrolled { get; set; }
    }

    public class CareerAssessmentResponseInsightRS
    {
        public int QuestionId { get; set; }
        public string AIAnalysis { get; set; } = string.Empty;
    }
}
