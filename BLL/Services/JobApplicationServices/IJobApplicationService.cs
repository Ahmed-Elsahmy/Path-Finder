using BLL.Common;
using BLL.Dtos.JobDtos;

namespace BLL.Services.JobApplicationServices
{
    public interface IJobApplicationService
    {
        Task<ServiceResult<JobApplicationRS>> ApplyToJobAsync(string userId, ApplyJobRQ request);
        Task<ServiceResult<List<JobApplicationRS>>> GetUserApplicationsAsync(string userId);
        Task<ServiceResult<string>> UpdateApplicationAsync(string userId, int applicationId, UpdateApplicationRQ request);
        Task<ServiceResult<string>> WithdrawApplicationAsync(string userId, int applicationId);
    }
}
