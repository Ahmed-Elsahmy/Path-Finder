using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CareerPathDtos
{
    public class CourseRecommendation
    {
        public int CourseId { get; set; }
        public int Order { get; set; }
        public bool IsRequired { get; set; }
        public string Reason { get; set; }
    }
}
