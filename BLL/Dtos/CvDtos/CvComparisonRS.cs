using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CvDtos
{
    public class CvComparisonRS
    {
        // one entry per CV
        public List<CvComparisonItem> CVs { get; set; } = new();

        // shared skills across ALL compared CVs
        public List<string> CommonSkills { get; set; } = new();

        // skills unique to each CV
        public List<CvUniqueSkills> UniqueSkills { get; set; } = new();

        // AI full comparison summary
        public string ComparisonSummary { get; set; } = string.Empty;

        // which CV the AI recommends and why
        public string RecommendedCvFileName { get; set; } = string.Empty;
        public string RecommendationReason { get; set; } = string.Empty;
    }
}
