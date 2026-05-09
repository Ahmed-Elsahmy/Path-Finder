using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Dashbord
{
    public class UserDashboardDto
    {
        public int ProfileCompletion { get; set; }
        public int ExperienceCompletion { get; set; }
        public int EducationCompletion { get; set; }
        public int TotalCompletion { get; set; }

        public List<MissingFieldDto> MissingFields { get; set; } = new List<MissingFieldDto>();
    }
}
