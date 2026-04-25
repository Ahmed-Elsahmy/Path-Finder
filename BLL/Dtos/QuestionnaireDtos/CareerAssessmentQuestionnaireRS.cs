namespace BLL.Dtos.QuestionnaireDtos
{
    public class CareerAssessmentQuestionnaireRS
    {
        public int QuestionnaireId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string QuestionnaireType { get; set; } = string.Empty;
        public int TotalQuestions { get; set; }
        public int AnsweredQuestions { get; set; }
        public int CompletionPercentage { get; set; }
        public DateTime? LastUpdatedAt { get; set; }
        public bool HasSavedResponses => AnsweredQuestions > 0;
        public List<CareerAssessmentQuestionRS> Questions { get; set; } = new();
    }

    public class CareerAssessmentQuestionRS
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = "single_choice";
        public List<string> Options { get; set; } = new();
        public int OrderNumber { get; set; }
        public bool IsRequired { get; set; }
        public string? SavedAnswer { get; set; }
        public string? SavedInsight { get; set; }
    }
}
