using BLL.Common;
using BLL.Dtos.AiDtos;

namespace BLL.Services.ResumeBuilderService
{
    public interface IResumeBuilderService
    {
        Task<ServiceResult<(byte[] PdfBytes, string FullName)>> GenerateResumePdfAsync(
            string userId,
            ResumeBuilderRQ request,
            CancellationToken cancellationToken = default);
    }
}