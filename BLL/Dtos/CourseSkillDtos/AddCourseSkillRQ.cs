using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseSkillDtos
{
    public class AddCourseSkillRQ
    {
        [Required]
        public int SkillId { get; set; }

        [StringLength(50)]
        public string? SkillLevel { get; set; }
    }
}
