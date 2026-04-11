using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CoursePlatformDtos
{
    public class UpdateCoursePlatformRQ
    {
        [StringLength(100)]
        public string? Name { get; set; }       

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(255)]
        public string? BaseUrl { get; set; }    

        [StringLength(500)]
        public string? LogoUrl { get; set; }

        [StringLength(255)]
        public string? ApiEndPoint { get; set; }

        public bool? IsActive { get; set; }
    }

}
