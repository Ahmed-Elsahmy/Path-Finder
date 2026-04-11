using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CourseSkillDtos
{
    public class CourseSkillRS
    {
        public int CourseSkillId { get; set; }
        public int CourseId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public string? SkillCategory { get; set; }
        public bool IsTechnical { get; set; }
        public string? SkillLevel { get; set; }
    }
}
