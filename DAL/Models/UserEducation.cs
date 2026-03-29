using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class UserEducation
    {
        [Key]
        public int EducationId { get; set; }
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(255)]
        public string Institution { get; set; }

        [StringLength(100)]
        public string? Degree { get; set; } // Nullable

        [StringLength(100)]
        public string? FieldOfStudy { get; set; } // Nullable

        [Column(TypeName = "date")]
        public DateTime? StartDate { get; set; } // Nullable

        [Column(TypeName = "date")]
        public DateTime? EndDate { get; set; } // Nullable

        public bool IsCurrent { get; set; } = false;
        [StringLength(500)]
        public List<string>? CertificatePaths { get; set; }

    }
}
