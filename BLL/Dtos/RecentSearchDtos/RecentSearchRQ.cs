using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.RecentSearchDtos
{
    public class RecentSearchRQ
    {
        [Required]
        [StringLength(300)]
        public string SearchTerm { get; set; } = string.Empty;
    }
}
