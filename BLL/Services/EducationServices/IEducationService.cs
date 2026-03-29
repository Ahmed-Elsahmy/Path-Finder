using BLL.Dtos.EducationDtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.EducationServices
{
    public interface IEducationService
    {
        Task<List<UserEducationRS>> GetUserEducationAsync(string userId);
        Task<string> AddEducationAsync(string userId, EducationRQ request);
        Task<string> UpdateEducationAsync(string userId, int educationId, EducationRQ request);
        Task<string> DeleteEducationAsync(string userId, int educationId);
        Task<string> UploadCertificateAsync(string userId, int educationId, List<IFormFile> file);
        Task<string> DeleteCertificateAsync(string userId, int educationId, string certificateUrl);
    }
}