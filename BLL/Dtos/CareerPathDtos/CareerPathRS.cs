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
    }
}
