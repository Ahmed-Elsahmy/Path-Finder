using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class SavedCourse
    {
        [Key]
        public int SavedCourseId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course course { get; set; } = null!;

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        public string? Notes { get; set; }
    }
}
