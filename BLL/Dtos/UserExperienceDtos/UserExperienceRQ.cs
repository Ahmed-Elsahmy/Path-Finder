using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace BLL.Dtos.UserExperienceDtos
{
    public class UserExperienceRQ
    {
        [Required(ErrorMessage ="You Must Add The CompnayName")]
        [StringLength(50)]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "You Must Add The Position")]
        [StringLength(50)]
        public string Position { get; set; }
        public string? Description { get; set; }
        [Required(ErrorMessage = "You Must Add The StartDate")]
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        [Required(ErrorMessage = "You Must Add The EmploymentType")]
        public EmploymentType ?EmploymentType { get; set; }
    }
}
