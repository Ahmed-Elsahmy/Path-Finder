using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.EducationDtos
{
    public class EducationRQ
    {
        public string Institution { get; set; }

        [StringLength(100)]
        public string? Degree { get; set; }

        [StringLength(100)]
        public string? FieldOfStudy { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public List<IFormFile>? Certificates { get; set; }

    }
}
