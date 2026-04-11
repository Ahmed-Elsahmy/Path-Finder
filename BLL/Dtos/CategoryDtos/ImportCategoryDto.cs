using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CategoryDtos
{
    public class ImportCategoryDto
    {
        public string Name { get; set; }
        public List<string> SubCategories { get; set; } = new List<string>();
    }
}
