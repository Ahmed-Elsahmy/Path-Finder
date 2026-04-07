using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Models
{
    public class UserProfile
    {
        [Key]
        public int ProfileId { get; set; }
        [ForeignKey("User")]
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [StringLength(100)]
        [Required]
        public string UserName { get; set; }
        [StringLength(50)]
        [Required]
        public string FirstName { get; set; }
        [StringLength(50)]
        [Required]
        public string LastName { get; set; }
        [StringLength(20)]
        public string ?PhoneNumber { get; set; }
        [StringLength(100)]
        public string? Bio { get; set; }
        [StringLength(50)]
        public string? Location { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? ProfilePictureUrl { get; set; }
    }
}
