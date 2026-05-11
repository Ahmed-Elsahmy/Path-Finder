using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace BLL.Dtos.UserCarrerPathDtos
{
    public class CareerPathRecommendationRS
    {
        public int CareerPathId { get; set; }

        public string? CareerPathName { get; set; }

        public string? Description { get; set; }

        public DifficultyLevel? DifficultyLevel { get; set; }

        public int ?DurationInMonths { get; set; }

        public int MatchScore { get; set; }

        public string? AIRecommendationReason { get; set; }

        public List<string> SkillsYouNeedToLearn { get; set; } = new();
    }

    public class CareerPathRecommendationListRS
    {
        public List<CareerPathRecommendationRS> Recommendations { get; set; } = new();

        public string? OverallAdvice { get; set; }
    }
}
