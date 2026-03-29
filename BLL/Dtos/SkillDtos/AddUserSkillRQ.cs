using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.SkillDtos
{
    public class AddUserSkillRQ
    {
        [Required]
        public int SkillId { get; set; }

        [StringLength(50)]
        public string? ProficiencyLevel { get; set; }

        [StringLength(200)]
        public string? Source { get; set; } = "Manual";
    }
}
