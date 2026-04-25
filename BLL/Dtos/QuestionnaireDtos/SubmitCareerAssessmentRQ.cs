using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.QuestionnaireDtos
{
    public class SubmitCareerAssessmentRQ
    {
        [Range(1, int.MaxValue)]
        public int QuestionnaireId { get; set; }

        [MinLength(1)]
        public List<CareerAssessmentAnswerRQ> Answers { get; set; } = new();
    }

    public class CareerAssessmentAnswerRQ
    {
        [Range(1, int.MaxValue)]
        public int QuestionId { get; set; }

        [MaxLength(4000)]
        public string? Answer { get; set; }
    }
}
