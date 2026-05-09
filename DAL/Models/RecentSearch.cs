using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DAL.Helper.Enums;

namespace DAL.Models
{
    public class RecentSearch
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;
        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
        [Required]
        [StringLength(300)]
        public string SearchTerm { get; set; } = string.Empty;
        [Required]
        public RecentSearchType SearchType { get; set; }

        public DateTime SearchedAt { get; set; } = DateTime.UtcNow;
    }
}
