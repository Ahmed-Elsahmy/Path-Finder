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
        [Required(ErrorMessage = "Please write your Email-address.")]
        [EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        [StringLength(128)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please write your password.")]
        [StringLength(256)]
        public string Password { get; set; }
    }
}
