using BLL.Dtos;
using BLL.Dtos.SkillDtos;
using DAL.Helper;
using DAL.Models;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.SkillService
{
    public class SkillService : ISkillService
    {
        private readonly AppDbContext _context;

        public SkillService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Skill>> GetAllGlobalSkillsAsync()
        {
            return await _context.Skills.OrderBy(s => s.SkillName).ToListAsync();
        }

        public async Task<string> CreateGlobalSkillAsync(CreateSkillRQ request)
        {
            try
            {
                // Check if skill already exists (case-insensitive)
                var exists = await _context.Skills
                    .AnyAsync(s => s.SkillName.ToLower() == request.SkillName.ToLower());

                if (exists) return "Skill already exists in the system.";

                var newSkill = new Skill
                {
                    SkillName = request.SkillName,
                    Category = request.Category,
                    Description = request.Description,
                    IsTechnical = request.IsTechnical
                };

                _context.Skills.Add(newSkill);
                await _context.SaveChangesAsync();
                return "Skill created successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return $"An error occurred while creating the skill: {ex.Message}";
            }
        }

        public async Task<string> AddSkillToUserAsync(string userId, AddUserSkillRQ request)
        {
            try
            {
                // 1. Check if the skill actually exists in the global DB
                var skillExists = await _context.Skills.AnyAsync(s => s.SkillId == request.SkillId);
                if (!skillExists) return "Skill not found in the global repository.";

                // 2. Prevent adding the same skill twice for a user
                var alreadyHasSkill = await _context.UserSkills
                    .AnyAsync(us => us.UserId == userId && us.SkillId == request.SkillId);
                if (alreadyHasSkill) return "You already have this skill in your profile.";

                // 3. Add the skill
                var userSkill = new UserSkill
                {
                    UserId = userId,
                    SkillId = request.SkillId,
                    ProficiencyLevel = request.ProficiencyLevel,
                    Source = request.Source,
                    AcquiredDate = DateTime.UtcNow
                };

                _context.UserSkills.Add(userSkill);
                await _context.SaveChangesAsync();
                return "Skill added to your profile.";
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return $"An error occurred while adding the skill: {ex.Message}";
            }

        }

        public async Task<List<UserSkillRS>> GetUserSkillsAsync(string userId)
        {
            try
            {
                return await _context.UserSkills
         .Include(us => us.Skill) // Join with Skill table to get the Name
         .Where(us => us.UserId == userId)
         .Select(us => new UserSkillRS
         {
             UserSkillId = us.UserSkillId,
             SkillId = us.SkillId,
             SkillName = us.Skill.SkillName,
             IsTechnical = us.Skill.IsTechnical,
             Category = us.Skill.Category ?? "",
             ProficiencyLevel = us.ProficiencyLevel ?? "Not Specified",
             AcquiredDate = us.AcquiredDate,
             Source = us.Source ?? "Manual"
         })
         .ToListAsync();
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return new List<UserSkillRS>(); // Return empty list on error
            }

        }

        public async Task<string> RemoveUserSkillAsync(string userId, int userSkillId)
        {
            try
            {
                var userSkill = await _context.UserSkills
    .FirstOrDefaultAsync(us => us.UserSkillId == userSkillId && us.UserId == userId);

                if (userSkill == null) return "Skill not found in your profile.";

                _context.UserSkills.Remove(userSkill);
                await _context.SaveChangesAsync();
                return "Skill removed successfully.";
            }
            catch (Exception ex)
            {
                // Log the exception (not implemented here)
                return $"An error occurred while removing the skill: {ex.Message}";

            }
        }
    }
}