using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.EducationDtos
{
    public class UserEducationRS
    {
        public int EducationId { get; set; }
        public string Institution { get; set; }
        public string? Degree { get; set; }
        public string? FieldOfStudy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        public List<string> CertificateUrls { get; set; } = new List<string>();

    }
}
