using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Dtos;

namespace BLL.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterRQ model);
        Task<AuthModel>LoginAsync(LoginRQ model);
    }
}
