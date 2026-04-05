using BLL.Common;
using BLL.Dtos.AiDtos;

namespace BLL.Services.ChatbotService
{
    public interface IChatbotService
    {
        /// <summary>General career chatbot — now personalized with user context</summary>
        Task<ServiceResult<string>> AskQuestionAsync(
            ChatRQ request,
            string userId,
            string username,
            string? email = null,
            CancellationToken cancellationToken = default);

        /// <summary>Feature 6 — Generate a career roadmap from user's current skills to target job</summary>
        Task<ServiceResult<string>> GenerateCareerRoadmapAsync(
            string userId,
            CareerRoadmapRQ request,
            CancellationToken cancellationToken = default);

        /// <summary>Feature 8 — Generate mock interview questions for a given role</summary>
        Task<ServiceResult<string>> GenerateInterviewPrepAsync(
            string userId,
            InterviewPrepRQ request,
            CancellationToken cancellationToken = default);
    }
}