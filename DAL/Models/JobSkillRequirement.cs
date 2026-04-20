using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class JobSkillRequirement
    {
        [Key]
        public int JobSkillId { get; set; }

        [Required]
        public int JobId { get; set; }

        [ForeignKey("JobId")]
        public virtual Job Job { get; set; } = null!;

        [Required]
        public int SkillId { get; set; }

        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; } = null!;

        [StringLength(50)]
        public string? RequiredLevel { get; set; } // Beginner, Intermediate, Advanced

        public bool IsMandatory { get; set; } = true;
    }
}
