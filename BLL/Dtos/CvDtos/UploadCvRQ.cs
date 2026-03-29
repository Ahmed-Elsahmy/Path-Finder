using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CvDtos
{
    public class UploadCvRQ
    {
        [Required]
        public IFormFile File { get; set; }

        public bool IsPrimary { get; set; } = false;
    }
}
