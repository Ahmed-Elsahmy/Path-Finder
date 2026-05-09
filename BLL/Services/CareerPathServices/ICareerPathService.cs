using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Common;
using BLL.Dtos.CareerPathDtos;
using BLL.Dtos.JobDtos;

namespace BLL.Services.CareerPathServices
{
   public interface ICareerPathService
    {
        Task<ServiceResult<List<CareerPathRS>>> GetAllCareerPathsAsync();
        Task<ServiceResult<CareerPathRS>> GetCareerPathByIdAsync(int id);
        Task<ServiceResult<List<CareerPathRS>>> SearchCareerPathsAsync(string name, string? userId = null);
        Task<ServiceResult<CareerPathRS>> CreateCareerPathAsync(CareerPathRQ request);
        Task<ServiceResult<CareerPathRS>> UpdateCareerPathAsync(int id, UpdateCareerPathRQ request);
        Task<ServiceResult<string>> DeleteCareerPathAsync(int id);

    }
}
