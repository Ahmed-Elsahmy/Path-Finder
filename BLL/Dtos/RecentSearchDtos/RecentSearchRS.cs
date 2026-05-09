using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Helper.Enums;

namespace BLL.Dtos.RecentSearchDtos
{
    public class RecentSearchRS
    {
        public int Id { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public RecentSearchType SearchType { get; set; }
        public DateTime SearchedAt { get; set; }
    }
}
