using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CoursePlatformDtos
{
    public class CoursePlatformRS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public string BaseUrl { get; set; }
        public string? LogoUrl { get; set; }
        public string? ApiEndPoint { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
