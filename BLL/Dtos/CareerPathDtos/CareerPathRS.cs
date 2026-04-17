using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace BLL.Dtos.CareerPathDtos
{
    public class CareerPathRS
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DifficultyLevel DifficultyLevel { get; set; }
        public int DurationInMonths { get; set; }
        public string Prerequisites {  get; set; }
        public string ExpectedOutcomes { get; set; }
        public DateTime CreatedAt { get; set; }
        public int TotalCourses { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
        public int CareerPathId { get; internal set; }
        public string PathName { get; internal set; }
        public object Courses { get; internal set; }
    }
}
