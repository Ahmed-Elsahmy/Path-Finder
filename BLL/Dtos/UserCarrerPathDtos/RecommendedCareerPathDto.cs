using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.UserCarrerPathDtos
{
    public class RecommendedCareerPathDto
    {
        public int CareerPathId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
    }
}
