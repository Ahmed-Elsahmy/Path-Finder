using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Dtos.CategoryDtos
{
    public class CategoryRS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<SubCategoryRS> SubCategories { get; set; } = new List<SubCategoryRS>();
    }
}
