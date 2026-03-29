using BLL.Dtos.SkillDtos;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL.Services.SkillService
{
    public interface ISkillService
    {
        Task<List<Skill>> GetAllGlobalSkillsAsync();
        Task<string> CreateGlobalSkillAsync(CreateSkillRQ request);
        Task<string> AddSkillToUserAsync(string userId, AddUserSkillRQ request);
        Task<List<UserSkillRS>> GetUserSkillsAsync(string userId);
        Task<string> RemoveUserSkillAsync(string userId, int userSkillId);
    }
}
