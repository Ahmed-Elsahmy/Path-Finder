using BLL.Common;
using BLL.Dtos.JobDtos;

namespace BLL.Services.SavedJobServices
{
    public interface ISavedJobService
    {
        Task<ServiceResult<string>> SaveJobAsync(string userId, SaveJobRQ request);
        Task<ServiceResult<List<SavedJobRS>>> GetSavedJobsAsync(string userId);
        Task<ServiceResult<string>> RemoveSavedJobAsync(string userId, int savedJobId);
        Task<ServiceResult<bool>> IsSavedAsync(string userId, int jobId);
    }
}
