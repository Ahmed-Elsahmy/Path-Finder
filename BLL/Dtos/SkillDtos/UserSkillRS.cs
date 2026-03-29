using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.SkillDtos
{
    public class UserSkillRS
    {
        public int UserSkillId { get; set; }
        public int SkillId { get; set; }
        public string SkillName { get; set; }
        public bool IsTechnical { get; set; }
        public string Category { get; set; }
        public string ProficiencyLevel { get; set; }
        public DateTime AcquiredDate { get; set; }
        public string Source { get; set; }
    }
}
