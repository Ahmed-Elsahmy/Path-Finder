using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace BLL.Dtos.UserCarrerPathDtos
{
    public class UserCareerPathRS
    {
        public int UserCareerPathId { get; set; }
        public int CareerPathId { get; set; }
        public DateTime EnrolledAt { get; set; }
        public string UserId { get; set; }
        public CareerPathStatus CareerPathStatus { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? AIRecommendationReason { get; set; }
    }
}
