using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.AiDtos
{
    /// <summary>Request for AI career roadmap generation</summary>
    public class CareerRoadmapRQ
    {
        [Required(ErrorMessage = "Target job title is required.")]
        [StringLength(200)]
        public string TargetJobTitle { get; set; }
    }
}
