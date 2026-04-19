using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace BLL.Dtos.UserCarrerPathDtos
{
    public class UserCareerPathRQ
    {
        public int CareerPathId { get; set; }
        public CareerPathStatus careerPathStatus { get; set; } = CareerPathStatus.InProgress;
    }
}
