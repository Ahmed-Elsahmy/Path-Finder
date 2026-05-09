using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Dtos.Dashbord;

namespace BLL.Services.DashbordServices
{
    public interface IUserDashboardService
    {
        Task<UserDashboardDto> GetUserDashboardAsync(string userId);
    }
}
