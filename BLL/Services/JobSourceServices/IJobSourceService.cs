using BLL.Common;
using BLL.Dtos.JobDtos;

namespace BLL.Services.JobSourceServices
{
    public interface IJobSourceService
    {
        Task<ServiceResult<List<JobSourceRS>>> GetAllSourcesAsync(bool onlyActive = true);
        Task<ServiceResult<JobSourceRS>> GetSourceByIdAsync(int id);
        Task<ServiceResult<string>> CreateSourceAsync(JobSourceRQ request);
        Task<ServiceResult<string>> UpdateSourceAsync(int id, UpdateJobSourceRQ request);
        Task<ServiceResult<string>> DeleteSourceAsync(int id);
    }
}
