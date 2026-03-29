using BLL.Dtos.CvDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.CvService
{
    public interface ICvService
    {
        Task<string> UploadCvAsync(string userId, UploadCvRQ request, string baseUrl);
        Task<List<CvRS>> GetUserCvsAsync(string userId);
        Task<string> SetPrimaryCvAsync(string userId, int cvId);
        Task<string> DeleteCvAsync(string userId, int cvId);
    }
}
