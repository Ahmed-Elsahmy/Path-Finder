using AutoMapper;
using BLL.Common;
using BLL.Dtos.SkillDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.SkillService
{
    public class SkillService : ISkillService
    {
        private readonly IRepository<Skill> _skillRepository;
        private readonly IRepository<UserSkill> _userSkillRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SkillService> _logger;

        public SkillService(
            IRepository<Skill> skillRepository,
            IRepository<UserSkill> userSkillRepository,
            IMapper mapper,
            ILogger<SkillService> logger)
        {
            _skillRepository = skillRepository;
            _userSkillRepository = userSkillRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<Skill>>> GetAllGlobalSkillsAsync()
        {
            try
            {
                var skills = await _skillRepository.Query()
                    .OrderBy(s => s.SkillName)
                    .ToListAsync();

                return ServiceResult<List<Skill>>.Success(skills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving global skills");
                return ServiceResult<List<Skill>>.Failure("An error occurred while retrieving skills.");
            }
        }

        public async Task<ServiceResult<string>> CreateGlobalSkillAsync(CreateSkillRQ request)
        {
            try
            {
                var exists = await _skillRepository
                    .AnyAsync(s => s.SkillName.ToLower() == request.SkillName.ToLower());

                if (exists)
                    return ServiceResult<string>.Failure("Skill already exists in the system.");

                var newSkill = new Skill
                {
                    SkillName = request.SkillName,
                    Category = request.Category,
                    Description = request.Description,
                    IsTechnical = request.IsTechnical
                };

                await _skillRepository.AddAsync(newSkill);
                await _skillRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Skill created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating global skill {SkillName}", request.SkillName);
                return ServiceResult<string>.Failure($"An error occurred while creating the skill: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> AddSkillToUserAsync(string userId, AddUserSkillRQ request)
        {
            try
            {
                var skillExists = await _skillRepository.AnyAsync(s => s.SkillId == request.SkillId);
                if (!skillExists)
                    return ServiceResult<string>.Failure("Skill not found in the global repository.");

                var alreadyHasSkill = await _userSkillRepository
                    .AnyAsync(us => us.UserId == userId && us.SkillId == request.SkillId);
                if (alreadyHasSkill)
                    return ServiceResult<string>.Failure("You already have this skill in your profile.");

                var userSkill = new UserSkill
                {
                    UserId = userId,
                    SkillId = request.SkillId,
                    ProficiencyLevel = request.ProficiencyLevel,
                    Source = request.Source,
                    AcquiredDate = DateTime.UtcNow
                };

                await _userSkillRepository.AddAsync(userSkill);
                await _userSkillRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Skill added to your profile.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding skill {SkillId} to user {UserId}", request.SkillId, userId);
                return ServiceResult<string>.Failure($"An error occurred while adding the skill: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<UserSkillRS>>> GetUserSkillsAsync(string userId)
        {
            try
            {
                var userSkills = await _userSkillRepository.Query()
                    .Include(us => us.Skill)
                    .Where(us => us.UserId == userId)
                    .ToListAsync();

                var result = _mapper.Map<List<UserSkillRS>>(userSkills);
                return ServiceResult<List<UserSkillRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skills for user {UserId}", userId);
                return ServiceResult<List<UserSkillRS>>.Failure("An error occurred while retrieving your skills.");
            }
        }

        public async Task<ServiceResult<string>> RemoveUserSkillAsync(string userId, int userSkillId)
        {
            try
            {
                var userSkill = await _userSkillRepository
                    .FirstOrDefaultAsync(us => us.UserSkillId == userSkillId && us.UserId == userId);

                if (userSkill == null)
                    return ServiceResult<string>.Failure("Skill not found in your profile.");

                _userSkillRepository.Remove(userSkill);
                await _userSkillRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Skill removed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing skill {UserSkillId} for user {UserId}", userSkillId, userId);
                return ServiceResult<string>.Failure($"An error occurred while removing the skill: {ex.Message}");
            }
        }
    }
}