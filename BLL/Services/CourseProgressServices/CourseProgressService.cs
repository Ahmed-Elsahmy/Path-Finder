using AutoMapper;
using BLL.Common;
using BLL.Dtos.CourseProgressDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.CourseProgressService
{
    public class CourseProgressService : ICourseProgressService
    {
        private readonly IRepository<CourseProgress> _progressRepo;
        private readonly IRepository<Course> _courseRepo;
        // 🟢 مستودعات المهارات لنظام المكافآت عند إتمام الكورس
        private readonly IRepository<CourseSkill> _courseSkillRepo;
        private readonly IRepository<UserSkill> _userSkillRepo;

        private readonly IMapper _mapper;
        private readonly ILogger<CourseProgressService> _logger;

        public CourseProgressService(
            IRepository<CourseProgress> progressRepo,
            IRepository<Course> courseRepo,
            IRepository<CourseSkill> courseSkillRepo,
            IRepository<UserSkill> userSkillRepo,
            IMapper mapper,
            ILogger<CourseProgressService> logger)
        {
            _progressRepo = progressRepo;
            _courseRepo = courseRepo;
            _courseSkillRepo = courseSkillRepo;
            _userSkillRepo = userSkillRepo;
            _mapper = mapper;
            _logger = logger;
        }

        // ====================================================
        // 1. جلب كورسات المستخدم الحالية
        // ====================================================
        public async Task<ServiceResult<List<CourseProgressRS>>> GetUserProgressAsync(string userId)
        {
            try
            {
                var progressList = await _progressRepo.Query()
                    .Include(p => p.Course)
                    .Where(p => p.UserId == userId)
                    .OrderByDescending(p => p.StartedAt)
                    .ToListAsync();

                return ServiceResult<List<CourseProgressRS>>.Success(_mapper.Map<List<CourseProgressRS>>(progressList));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching progress for user {UserId}", userId);
                return ServiceResult<List<CourseProgressRS>>.Failure("An error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<CourseProgressRS>> GetProgressByIdAsync(string userId, int progressId)
        {
            var progress = await _progressRepo.Query()
                .Include(p => p.Course)
                .FirstOrDefaultAsync(p => p.ProgressId == progressId && p.UserId == userId);

            if (progress == null)
                return ServiceResult<CourseProgressRS>.Failure("Progress record not found.", ServiceErrorCode.NotFound);

            return ServiceResult<CourseProgressRS>.Success(_mapper.Map<CourseProgressRS>(progress));
        }

        // ====================================================
        // 2. التسجيل في الكورس (Enroll)
        // ====================================================
        public async Task<ServiceResult<string>> EnrollInCourseAsync(string userId, EnrollCourseRQ request)
        {
            try
            {
                var courseExists = await _courseRepo.AnyAsync(c => c.Id == request.CourseId);
                if (!courseExists)
                    return ServiceResult<string>.Failure("Course not found.", ServiceErrorCode.NotFound);

                var alreadyEnrolled = await _progressRepo.AnyAsync(p => p.CourseId == request.CourseId && p.UserId == userId);
                if (alreadyEnrolled)
                    return ServiceResult<string>.Failure("You are already enrolled in this course.", ServiceErrorCode.ValidationError);

                var newProgress = new CourseProgress
                {
                    UserId = userId,
                    CourseId = request.CourseId,
                    StartedAt = DateTime.UtcNow,
                    CompletedLessons = 0,
                    ProgressPercentage = 0,
                    Status = "Not Started"
                };

                await _progressRepo.AddAsync(newProgress);
                await _progressRepo.SaveChangesAsync();

                return ServiceResult<string>.Success("Successfully enrolled in the course.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enrolling user {UserId} in course {CourseId}", userId, request.CourseId);
                return ServiceResult<string>.Failure("An error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        // ====================================================
        // 3. التحديث التراكمي (Incremental Update) والمكافآت
        // ====================================================
        public async Task<ServiceResult<string>> UpdateProgressAsync(string userId, int progressId, UpdateProgressRQ request)
        {
            try
            {
                var progress = await _progressRepo.Query()
                    .Include(p => p.Course)
                    .FirstOrDefaultAsync(p => p.ProgressId == progressId && p.UserId == userId);

                if (progress == null)
                    return ServiceResult<string>.Failure("Progress record not found.", ServiceErrorCode.NotFound);

                int totalLessons = progress.Course.TotalLessons > 0 ? progress.Course.TotalLessons : 1;

                // 🟢 1. السيناريو التراكمي (نجمع الدروس المنجزة حالياً مع الرصيد القديم)
                if (request.NewlyCompletedLessons > 0)
                {
                    progress.CompletedLessons += request.NewlyCompletedLessons;
                }

                // حماية: التأكد أن المجموع لا يتخطى إجمالي دروس الكورس
                if (progress.CompletedLessons > totalLessons)
                    progress.CompletedLessons = totalLessons;

                if (request.Notes != null)
                    progress.Notes = request.Notes;

                // 🟢 2. حساب النسبة المئوية أوتوماتيكياً
                double percentage = ((double)progress.CompletedLessons / totalLessons) * 100;
                progress.ProgressPercentage = (int)Math.Round(percentage);

                // متغير لكي نُعطي المهارات للمستخدم مرة واحدة فقط عند التخرج
                bool justCompletedNow = false;

                // 🟢 3. تغيير الحالة والتاريخ بناءً على الإنجاز
                if (progress.CompletedLessons == 0)
                {
                    progress.Status = "Not Started";
                    progress.CompletedAt = null;
                }
                else if (progress.CompletedLessons > 0 && progress.CompletedLessons < totalLessons)
                {
                    progress.Status = "In Progress";
                    progress.CompletedAt = null;
                }
                else if (progress.CompletedLessons == totalLessons)
                {
                    if (progress.Status != "Completed")
                        justCompletedNow = true; // اكتشفنا أنه أنهى الكورس للتو في هذه اللحظة!

                    progress.Status = "Completed";
                    if (!progress.CompletedAt.HasValue)
                        progress.CompletedAt = DateTime.UtcNow;
                }

                // حفظ التقدم
                _progressRepo.Update(progress);
                await _progressRepo.SaveChangesAsync();

                // 🟢 4. السحر: نقل المهارات للمستخدم إذا اكتمل الكورس الآن
                if (justCompletedNow)
                {
                    await AssignCourseSkillsToUserAsync(userId, progress.CourseId, progress.Course.Name);
                }

                return ServiceResult<string>.Success($"Progress updated. ({progress.CompletedLessons}/{totalLessons} lessons completed - {progress.ProgressPercentage}%).");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating progress {ProgressId} for user {UserId}", progressId, userId);
                return ServiceResult<string>.Failure("An error occurred while computing progress.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        // ====================================================
        // دالة مساعدة: نقل مهارات الكورس لبروفايل المستخدم
        // ====================================================
        private async Task AssignCourseSkillsToUserAsync(string userId, int courseId, string courseName)
        {
            try
            {
                // جلب المهارات المربوطة بهذا الكورس
                var courseSkills = await _courseSkillRepo.Query()
                    .Where(cs => cs.CourseId == courseId)
                    .ToListAsync();

                if (!courseSkills.Any()) return;

                foreach (var cs in courseSkills)
                {
                    // التأكد أن المستخدم لا يمتلكها في بروفايله مسبقاً لمنع التكرار
                    bool hasSkill = await _userSkillRepo.AnyAsync(us => us.UserId == userId && us.SkillId == cs.SkillId);

                    if (!hasSkill)
                    {
                        var newUserSkill = new UserSkill
                        {
                            UserId = userId,
                            SkillId = cs.SkillId,
                            ProficiencyLevel = cs.SkillLevel ?? "Beginner", // نأخذ المستوى الذي حدده الذكاء الاصطناعي
                            Source = $"Earned from Course: {courseName}",
                            AcquiredDate = DateTime.UtcNow
                        };
                        await _userSkillRepo.AddAsync(newUserSkill);
                    }
                }

                await _userSkillRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning skills to user {UserId} after completing course {CourseId}", userId, courseId);
            }
        }

        // ====================================================
        // 5. إلغاء التسجيل (Drop Course)
        // ====================================================
        public async Task<ServiceResult<string>> DropCourseAsync(string userId, int progressId)
        {
            var progress = await _progressRepo.FirstOrDefaultAsync(p => p.ProgressId == progressId && p.UserId == userId);
            if (progress == null)
                return ServiceResult<string>.Failure("Progress record not found.", ServiceErrorCode.NotFound);

            _progressRepo.Remove(progress);
            await _progressRepo.SaveChangesAsync();

            return ServiceResult<string>.Success("Course dropped successfully.");
        }
    }
}