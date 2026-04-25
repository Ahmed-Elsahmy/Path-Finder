using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class Question
    {
        [Key]
        public int QuestionId { get; set; }

        [Required]
        public int QuestionnaireId { get; set; }

        [ForeignKey(nameof(QuestionnaireId))]
        public Questionnaire Questionnaire { get; set; } = null!;

        [Required]
        [MaxLength(1000)]
        public string QuestionText { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string QuestionType { get; set; } = "single_choice";

        public List<string>? Options { get; set; } = new();

        [Required]
        public int OrderNumber { get; set; }

        public bool IsRequired { get; set; } = true;

        public ICollection<QuestionnaireResponse> Responses { get; set; } = new List<QuestionnaireResponse>();
    }
}
