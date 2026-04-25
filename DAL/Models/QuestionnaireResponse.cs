using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class QuestionnaireResponse
    {
        [Key]
        public int ResponseId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey(nameof(UserId))]
        public User User { get; set; } = null!;

        [Required]
        public int QuestionId { get; set; }

        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; } = null!;

        public string? Answer { get; set; }

        public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;

        public string? AIAnalysis { get; set; }
    }
}
