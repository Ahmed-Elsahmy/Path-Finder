using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class CoursePlatform
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string BaseUrl { get; set; }
        public string LogoUrl { get; set; }
        public string ApiEndPoint { get; set; }
        public bool IsActive { get; set; } = true; // Added: Soft delete or disable
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public List<Course> Courses { get; set; } = new List<Course>(); // Navigation property
    }
}
