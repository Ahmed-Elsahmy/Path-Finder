using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CategoryDtos
{
    public class SubCategoryRQ
    {
        [Required(ErrorMessage ="Please Add Name")]
        public string Name { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
