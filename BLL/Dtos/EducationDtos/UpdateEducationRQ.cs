using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.EducationDtos
{
    public class UpdateEducationRQ
    {
        [StringLength(255)]
        public string? Institution { get; set; }

        [StringLength(100)]
        public string? Degree { get; set; }

        [StringLength(100)]
        public string? FieldOfStudy { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
