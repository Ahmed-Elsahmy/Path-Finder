using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Common;
using BLL.Dtos.CareerPathDtos;

namespace BLL.Services.CareerPathServices
{
   public interface ICareerPathService
    {
         Task<ServiceResult<List<CareerPathRS>>> GetAllCareerPathsAsync();
        Task<ServiceResult<CareerPathRS>> GetCareerPathByIdAsync(int id);
        Task<ServiceResult<string>> CreateCareerPathAsync(CareerPathRQ request);
        Task<ServiceResult<string>> UpdateCareerPathAsync(int id, UpdateCareerPathRQ request);
        Task<ServiceResult<string>> DeleteCareerPathAsync(int id);

    }
}
