using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserSkill
    {
        [Key]
        public int UserSkillId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        public int SkillId { get; set; }

        [ForeignKey("SkillId")]
        public virtual Skill Skill { get; set; }

        [StringLength(50)]
        public string? ProficiencyLevel { get; set; } // e.g., Beginner, Intermediate, Expert

        public DateTime AcquiredDate { get; set; } = DateTime.UtcNow;

        [StringLength(200)]
        public string? Source { get; set; } // e.g., "Manual", "AI Extracted from CV"

    }
}
