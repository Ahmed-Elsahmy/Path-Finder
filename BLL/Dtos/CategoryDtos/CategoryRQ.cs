using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CategoryDtos
{
    public class CategoryRQ
    {
        [Required(ErrorMessage ="Please Add Category Name")]
        public string Name { get; set; }
    }
}
