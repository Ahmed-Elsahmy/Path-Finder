using BLL.Common;
using BLL.Dtos.JobDtos;

namespace BLL.Services.JobServices
{
    public interface IJobService
    {
        Task<ServiceResult<List<JobRS>>> GetJobsAsync(JobFilterRQ filter);
        Task<ServiceResult<JobRS>> GetJobByIdAsync(int id);
        Task<ServiceResult<List<JobRS>>> SearchJobsAsync(string name, string? userId = null);
        Task<ServiceResult<string>> CreateJobAsync(JobRQ request);
        Task<ServiceResult<string>> UpdateJobAsync(int id, UpdateJobRQ request);
        Task<ServiceResult<string>> DeleteJobAsync(int id);
        Task<ServiceResult<List<JobRS>>> GetRecommendedJobsAsync(string userId);
    }
}
