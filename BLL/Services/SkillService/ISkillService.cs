using BLL.Common;
using BLL.Dtos.SkillDtos;
using DAL.Models;

namespace BLL.Services.SkillService
{
    public interface ISkillService
    {
        Task<ServiceResult<List<Skill>>> GetAllGlobalSkillsAsync();
        Task<ServiceResult<string>> CreateGlobalSkillAsync(CreateSkillRQ request);
        Task<ServiceResult<string>> AddSkillToUserAsync(string userId, AddUserSkillRQ request);
        Task<ServiceResult<List<UserSkillRS>>> GetUserSkillsAsync(string userId);
        Task<ServiceResult<string>> RemoveUserSkillAsync(string userId, int userSkillId);
    }
}
