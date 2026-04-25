using BLL.Common;
using BLL.Dtos.QuestionnaireDtos;

namespace BLL.Services.QuestionnaireServices
{
    public interface IQuestionnaireService
    {
        Task<ServiceResult<CareerAssessmentQuestionnaireRS>> GetCareerAssessmentAsync(
            string userId,
            CancellationToken cancellationToken = default);

        Task<ServiceResult<CareerAssessmentResultRS>> SubmitCareerAssessmentAsync(
            string userId,
            SubmitCareerAssessmentRQ request,
            CancellationToken cancellationToken = default);
    }
}
