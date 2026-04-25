using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using DAL.Helper.Enums;

namespace BLL.Dtos.CareerPathDtos
{
    public class CareerPathRQ
    {
        [Required]
        public string CareerPathName { get; set; }
        public string? Description { get; set; }
        [EnumDataType(typeof(DifficultyLevel), ErrorMessage = "Invalid difficulty level.")]
        public DifficultyLevel? DifficultyLevel { get; set; }
        [Range(0, 250, ErrorMessage = "Duration in months must be between 0 and 250 Hour.")]
        public int? DurationInMonths { get; set; }
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
    }
}
