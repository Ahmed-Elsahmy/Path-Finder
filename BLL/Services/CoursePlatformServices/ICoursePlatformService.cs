using BLL.Common;
using BLL.Dtos.CoursePlatformDtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.CoursePlatformServices
{
    public interface ICoursePlatformService
    {
        Task<ServiceResult<List<CoursePlatformRS>>> GetAllPlatformsAsync(bool onlyActive = false);

        Task<ServiceResult<CoursePlatformRS>> GetPlatformByIdAsync(int id);

        Task<ServiceResult<string>> CreatePlatformAsync(CoursePlatformRQ request);

        Task<ServiceResult<string>> UpdatePlatformAsync(int id, UpdateCoursePlatformRQ request, IFormCollection form);

        Task<ServiceResult<string>> DeletePlatformAsync(int id);
    }
}
