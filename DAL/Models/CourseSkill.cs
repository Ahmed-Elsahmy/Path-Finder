using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class CourseSkill
    {
        [Key]
        public int CourseSkillId { get; set; }

        [Required]
        public int CourseId { get; set; }
        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; }

        [Required]
        public int SkillId { get; set; }

        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; }

        [StringLength(50)]
        public string? SkillLevel { get; set; } 
    }
}