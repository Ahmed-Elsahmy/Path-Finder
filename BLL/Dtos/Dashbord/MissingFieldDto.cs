using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.Dashbord
{
    public class MissingFieldDto
    {
        public string Message { get; set; }
        public string Section { get; set; } // Profile, Experience, Education
        public string ActionUrl { get; set; }
    }
}
