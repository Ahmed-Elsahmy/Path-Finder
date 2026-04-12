using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class Milestone
    {
        [Key]
        public int MilestoneId { get; set; }
        [Required]
        public int CareerPathId { get; set; }
        [ForeignKey("CareerPathId")]
        public virtual CareerPath CareerPath { get; set; }
        [Required]
        [MaxLength(200)]
        public string MilestoneName { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [Required]
        public int Order { get; set; }
        [MaxLength(500)]
        public string ?CompletionCriteria { get; set; }
        public int PointsAwarded { get; set; } = 0;
    }
}
