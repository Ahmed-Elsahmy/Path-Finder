using BLL.Dtos.Dashbord;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;

namespace BLL.Services.DashbordServices
{
    public class UserDashboardService : IUserDashboardService
    {
        private readonly IRepository<User> _userRepo;
        private readonly IRepository<UserExperience> _expRepo;
        private readonly IRepository<UserEducation> _eduRepo;
        private readonly IRepository<UserProfile> _profileRepo;

        public UserDashboardService(
            IRepository<User> userRepo,
            IRepository<UserExperience> expRepo,
           IRepository<UserEducation> eduRepo,
           IRepository<UserProfile> profileRepo)
        {
            _userRepo = userRepo;
            _expRepo = expRepo;
            _eduRepo = eduRepo;
            _profileRepo = profileRepo;
        }

        public async Task<UserDashboardDto> GetUserDashboardAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found");

            var experiences = await _expRepo.Query()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var educations = await _eduRepo.Query()
                .Where(x => x.UserId == userId)
                .ToListAsync();

            var (profileMissing, profile) = CalculateProfile(user);
            var (expMissing, exp) = CalculateExperience(experiences);
            var (eduMissing, edu) = CalculateEducation(educations);

            var allMissing = profileMissing
                .Concat(expMissing)
                .Concat(eduMissing)
                .GroupBy(x => x.Message)
                .Select(g => g.First())
                .ToList();

            var total = CalculateTotal(profile, exp, edu);

            return new UserDashboardDto
            {
                ProfileCompletion = profile,
                ExperienceCompletion = exp,
                EducationCompletion = edu,
                TotalCompletion = total,
                MissingFields = allMissing
            };
        }

        // ---------------- Profile ----------------
        private (List<MissingFieldDto>, int) CalculateProfile(User user)
        {
            var missing = new List<MissingFieldDto>();
            int total = 5;
            int completed = 0;

            if (!string.IsNullOrEmpty(user.FirstName)) completed++;
            else missing.Add(Create("Add First Name", "Profile", "/profile/edit"));

            if (!string.IsNullOrEmpty(user.LastName)) completed++;
            else missing.Add(Create("Add Last Name", "Profile", "/profile/edit"));

            if (!string.IsNullOrEmpty(user.Email)) completed++;
            else missing.Add(Create("Add Email", "Profile", "/profile/edit"));

            if (!string.IsNullOrEmpty(user.PhoneNumber)) completed++;
            else missing.Add(Create("Add Phone Number", "Profile", "/profile/edit"));
            var profile = _profileRepo.FirstOrDefaultAsync(p => p.UserId == user.Id);
            if (!string.IsNullOrEmpty(profile.Result.ProfilePictureUrl)) completed++;
            else missing.Add(Create("Add Profile Image", "Profile", "/profile/edit"));

            return (missing, (completed * 100) / total);
        }

        // ---------------- Experience ----------------
        private (List<MissingFieldDto>, int) CalculateExperience(List<UserExperience> experiences)
        {
            var missing = new List<MissingFieldDto>();

            if (!experiences.Any())
            {
                missing.Add(Create("Add Work Experience", "Experience", "/experience/add"));
                return (missing, 0);
            }

            int totalFields = 4;
            int totalScore = 0;

            foreach (var exp in experiences)
            {
                int completed = 0;

                if (!string.IsNullOrEmpty(exp.Position)) completed++;
                else missing.Add(Create("Add Job Title", "Experience", "/experience/edit"));

                if (!string.IsNullOrEmpty(exp.CompanyName)) completed++;
                else missing.Add(Create("Add Company Name", "Experience", "/experience/edit"));

                if (exp.StartDate != null) completed++;
                else missing.Add(Create("Add Start Date", "Experience", "/experience/edit"));

                if (exp.EndDate != null) completed++;
                else missing.Add(Create("Add End Date", "Experience", "/experience/edit"));

                totalScore += (completed * 100) / totalFields;
            }

            return (missing, totalScore / experiences.Count);
        }

        // ---------------- Education ----------------
        private (List<MissingFieldDto>, int) CalculateEducation(List<UserEducation> educations)
        {
            var missing = new List<MissingFieldDto>();

            if (!educations.Any())
            {
                missing.Add(Create("Add Education", "Education", "/education/add"));
                return (missing, 0);
            }

            int totalFields = 4;
            int totalScore = 0;

            foreach (var edu in educations)
            {
                int completed = 0;

                if (!string.IsNullOrEmpty(edu.Institution)) completed++;
                else missing.Add(Create("Add School Name", "Education", "/education/edit"));

                if (!string.IsNullOrEmpty(edu.Degree)) completed++;
                else missing.Add(Create("Add Degree", "Education", "/education/edit"));

                if (edu.StartDate != null) completed++;
                else missing.Add(Create("Add Start Date", "Education", "/education/edit"));

                if (edu.EndDate != null) completed++;
                else missing.Add(Create("Add End Date", "Education", "/education/edit"));

                totalScore += (completed * 100) / totalFields;
            }

            return (missing, totalScore / educations.Count);
        }

        // ---------------- Helper ----------------
        private MissingFieldDto Create(string msg, string section, string url)
        {
            return new MissingFieldDto
            {
                Message = msg,
                Section = section,
                ActionUrl = url
            };
        }

        private int CalculateTotal(int profile, int exp, int edu)
        {
            return (int)(profile * 0.4 + exp * 0.3 + edu * 0.3);
        }
    }
}
