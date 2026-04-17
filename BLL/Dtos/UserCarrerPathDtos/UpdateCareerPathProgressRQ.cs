using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.UserCarrerPathDtos
{
    public class UpdateCareerPathProgressRQ
    {
        [Required]
        public int NewlyAchievedMilestones { get; set; }
    }
}
