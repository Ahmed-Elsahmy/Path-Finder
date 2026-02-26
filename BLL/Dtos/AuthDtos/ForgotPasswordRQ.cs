using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.AuthDtos
{
    public class ForgotPasswordRQ
    {
        [Required(ErrorMessage ="Please provide your email address."), EmailAddress(ErrorMessage = "Please provide a valid email address.")]
        public string Email { get; set; }
    }
}
