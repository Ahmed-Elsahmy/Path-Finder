using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Dtos.AuthDtos;

namespace BLL.Services.AuthService
{
    public interface IAuthService
    {
        Task<AuthModel> RegisterAsync(RegisterRQ model);
        Task<AuthModel>LoginAsync(LoginRQ model);
        Task<AuthModel> GoogleSignInAsync(GoogleAuthRQ model);
        Task<AuthModel> ForgotPasswordAsync(ForgotPasswordRQ model);
        Task<AuthModel> ResetPasswordAsync(ResetPasswordRQ model);
        Task<AuthModel> ConfirmEmailAsync(ConfirmEmailRQ model);


    }
}
