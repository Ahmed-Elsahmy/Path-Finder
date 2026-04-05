using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CvDtos
{
    public class CvComparisonItem
    {
        public int CVId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int ScoreOutOf10 { get; set; }
        public string Summary { get; set; } = string.Empty;
        public List<string> Strengths { get; set; } = new();
        public List<string> Weaknesses { get; set; } = new();
        public List<string> Skills { get; set; } = new();
    }
}
