using System.ComponentModel.DataAnnotations;

namespace BLL.Dtos.AuthDtos
{
    public class ResetPasswordRQ
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Otp { get; set; } 

        [Required, StringLength(256)]
        public string NewPassword { get; set; }

        [Required, Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmNewPassword { get; set; }
    }
}