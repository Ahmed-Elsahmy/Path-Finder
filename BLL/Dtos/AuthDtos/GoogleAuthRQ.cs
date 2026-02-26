using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.AuthDtos
{
    public class GoogleAuthRQ
    {
        [Required]
        public string IdToken { get; set; }
    }
}
