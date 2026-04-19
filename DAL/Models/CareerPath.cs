using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace DAL.Models
{
    public class CareerPath
    {
        [Key]
        public int CareerPathId { get; set; }
        [Required]
        [MaxLength(200)]
        public string PathName { get; set; }
        [MaxLength(1000)]
        public string ?Description { get; set; }
        public DifficultyLevel ?DifficultyLevel { get; set; }
        [Range(0,250)]
       public int ?EstimatedDurationMonths { get; set; }
        public string ?Prerequisites { get; set; }
        public string ?ExpectedOutcomes { get; set; }
        public int TotalCourses { get; set; } = 1;
        public int? CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        public int? SubCategoryId { get; set; }
        [ForeignKey("SubCategoryId")]
        public virtual SubCategory SubCategory { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<CareerPathCourse> CareerPathCourses { get; set; } = new List<CareerPathCourse>();

    }
}
