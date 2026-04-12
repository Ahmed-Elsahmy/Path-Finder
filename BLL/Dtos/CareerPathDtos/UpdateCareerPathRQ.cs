using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace BLL.Dtos.CareerPathDtos
{
    public class UpdateCareerPathRQ
    {
        public string ?CareerPathName {  get; set; }
        public string ?Description { get; set; }
        [EnumDataType(typeof(DifficultyLevel), ErrorMessage = "Invalid difficulty level.")]
        public DifficultyLevel ?DifficultyLevel { get; set; }
        [Range(0, 250, ErrorMessage = "Duration in months must be between 0 and 250 Hour.")]
        public int ?DurationInMonths { get; set; }
        [MaxLength(500, ErrorMessage = "Prerequisites cannot exceed 500 characters.")]
        public string ?Prerequisites { get; set; }
        [MaxLength(1000, ErrorMessage = "Expected Outcomes cannot exceed 1000 characters.")]
        public string ?ExpectedOutcomes { get; set; }
    }
}
