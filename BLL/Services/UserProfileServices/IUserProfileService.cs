using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Common;
using BLL.Dtos.EducationDtos;
using BLL.Dtos.UserProfileDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace BLL.Services.UserProfileServices
{
    public interface IUserProfileService
    {
        Task<ServiceResult<UserProfileRS>> GetUserProfileAsync(string userId);
        Task<ServiceResult<string>> AddUserProfileAsync(string userId, UserProfileRQ request);
        Task<ServiceResult<string>> UpdateUserProfileAsync(string userId, UpdateUserProfileRQ request, IFormCollection form);
    }
}
