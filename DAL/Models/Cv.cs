using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class CV
    {
        [Key]
        public int CVId { get; set; }

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [Required]
        [StringLength(255)]
        public string FileName { get; set; }
        [Required]
        [StringLength(500)]
        public string FileUrl { get; set; }

        public string? ParsedContent { get; set; } 

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public bool IsPrimary { get; set; } = false;

        public List<string>? ExtractedSkills { get; set; }
        public List<string>? CVIssues { get; set; } 
    }
}
