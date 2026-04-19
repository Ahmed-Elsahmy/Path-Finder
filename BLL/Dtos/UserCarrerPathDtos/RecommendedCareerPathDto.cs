using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.UserCarrerPathDtos
{
    public class RecommendedCareerPathDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }
        public string ?Reason { get; set; }
        public List<string> ?MissingSkills { get; set; }
    }
}
