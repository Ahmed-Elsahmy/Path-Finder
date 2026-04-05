using BLL.Common;
using BLL.Dtos.EducationDtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace BLL.Services.EducationServices
{
    public interface IEducationService
    {
        Task<ServiceResult<List<UserEducationRS>>> GetUserEducationAsync(string userId);
        Task<ServiceResult<string>> AddEducationAsync(string userId, EducationRQ request);
        Task<ServiceResult<string>> UpdateEducationAsync(string userId, int educationId, JsonPatchDocument<UpdateEducationRQ> request);
        Task<ServiceResult<string>> DeleteEducationAsync(string userId, int educationId);
        Task<ServiceResult<string>> UploadCertificateAsync(string userId, int educationId, List<IFormFile> files);
        Task<ServiceResult<string>> DeleteCertificateAsync(string userId, int educationId, string certificateUrl);
    }
}