using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CvDtos
{
    public class CvComparisonRQ
    {
        [Required]
        [MinLength(2, ErrorMessage = "You must provide at least 2 CV IDs to compare.")]
        [MaxLength(5, ErrorMessage = "You can compare a maximum of 5 CVs at once.")]
        public List<int> CvIds { get; set; } = new();
    }
}
