using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.AuthDtos
{
    public class RegisterRQ
    {
        [Required(ErrorMessage ="Please write your first name."), StringLength(100)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please write your last name."), StringLength(100)]
        public string LastName { get; set; }

        [Required(ErrorMessage ="Please wirte a user name."), StringLength(50)]
        public string Username { get; set; }

        [Required(ErrorMessage ="Please add your vaild Email-address."), StringLength(128),EmailAddress]
        public string Email { get; set; }
        [StringLength(11)]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage ="Please write strong password."), StringLength(256)]
        public string Password { get; set; }
        [Required, StringLength(256), Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
