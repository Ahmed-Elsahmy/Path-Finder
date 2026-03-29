using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.SkillDtos
{
    public class CreateSkillRQ
    {
        [Required, StringLength(200)]
        public string SkillName { get; set; }
        [StringLength(100)]
        public string? Category { get; set; }
        [StringLength(500)]
        public string? Description { get; set; }
        public bool IsTechnical { get; set; }
    }
}
