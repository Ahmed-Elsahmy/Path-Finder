using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; }
        [Required]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [Required]
        public int MilestoneId {  get; set; }
        [ForeignKey("MilestoneId")]
        public virtual Milestone Milestone { get; set; }
        public DateTime AchievedAt { get; set; } = DateTime.UtcNow;
        public string ?BadgeUrl { get; set; }
    }
}
