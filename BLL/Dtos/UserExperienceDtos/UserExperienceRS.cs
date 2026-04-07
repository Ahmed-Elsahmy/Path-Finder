using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace BLL.Dtos.UserExperienceDtos
{
    public class UserExperienceRS
    {
        public string CompanyName { get; set; }
        public string Position { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCurrent { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EmploymentType? EmploymentType { get; set; }

    }
}
