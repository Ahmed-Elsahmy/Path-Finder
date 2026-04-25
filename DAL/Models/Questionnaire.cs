using System.ComponentModel.DataAnnotations;

namespace DAL.Models
{
    public class Questionnaire
    {
        [Key]
        public int QuestionnaireId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(50)]
        public string? QuestionnaireType { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
