using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.AuthDtos
{
    public class LoginRQ
    {
        [Required(ErrorMessage ="Please write your Email-address.")]
       public string Email { get; set; }
        [Required(ErrorMessage ="Please write your password.")]
        public string Password { get; set; }
    }
}
