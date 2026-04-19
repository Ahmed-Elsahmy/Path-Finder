using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BLL.Common;
using BLL.Dtos.CareerPathCourseDtos;

namespace BLL.Services.CareerPathCourseServices
{
    public interface ICareerPathCourseService
    {
        Task<ServiceResult<CareerPathCourseRS>> CreateAsync(CareerPathCourseRQ request);
        Task<ServiceResult<string>> DeleteAsync(int id);
        Task<ServiceResult<List<CareerPathCourseRS>>> GetByCareerPathIdAsync(int careerPathId);
    }
}
