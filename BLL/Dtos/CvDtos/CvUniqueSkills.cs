using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CvDtos
{
    public class CvUniqueSkills
    {
        public int CVId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public List<string> Skills { get; set; } = new();
    }
}
