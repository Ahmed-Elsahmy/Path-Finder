using BLL.Common;
using BLL.Dtos.CvDtos;

namespace BLL.Services.CvService
{
    public interface ICvService
    {
        Task<ServiceResult<string>> UploadCvAsync(string userId, UploadCvRQ request, string baseUrl);
        Task<ServiceResult<List<CvRS>>> GetUserCvsAsync(string userId);
        Task<ServiceResult<string>> SetPrimaryCvAsync(string userId, int cvId);
        Task<ServiceResult<string>> DeleteCvAsync(string userId, int cvId);
        Task<ServiceResult<CvComparisonRS>> CompareCvsAsync(string userId,  CvComparisonRQ request, CancellationToken cancellationToken = default);
    }
}
