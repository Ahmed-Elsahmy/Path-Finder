namespace BLL.Dtos.QuestionnaireDtos
{
    public class AssessmentAiResult
    {
        public string ProfileSummary { get; set; } = string.Empty;
        public string RecommendationStrategy { get; set; } = string.Empty;
        public List<string> TopTraits { get; set; } = new();
        public List<CareerAssessmentResponseInsightRS> ResponseInsights { get; set; } = new();
        public List<AssessmentAiRecommendation> Recommendations { get; set; } = new();
    }

    public class AssessmentAiRecommendation
    {
        public int CareerPathId { get; set; }
        public int SuitabilityScore { get; set; }
        public string MatchReason { get; set; } = string.Empty;
        public string WhyItFits { get; set; } = string.Empty;
        public List<string> StrengthSignals { get; set; } = new();
        public List<string> GrowthAreas { get; set; } = new();
        public string SuggestedNextStep { get; set; } = string.Empty;
    }
}
