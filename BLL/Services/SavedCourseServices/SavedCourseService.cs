using AutoMapper;
using BLL.Common;
using BLL.Dtos.SavedCourseDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.SavedCourseServices
{
    public class SavedCourseService : ISavedCourseService
    {
        private readonly IRepository<SavedCourse> _savedCourseRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<SavedCourseService> _logger;

        public SavedCourseService(
            IRepository<SavedCourse> savedCourseRepository,
            IRepository<Course> courseRepository,
            IMapper mapper,
            ILogger<SavedCourseService> logger)
        {
            _savedCourseRepository = savedCourseRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> SaveCourseAsync(string userId, SavedCourseRQ request)
        {
            try
            {
                var course = await _courseRepository.GetByIdAsync(request.CourseId);
                if (course == null)
                    return ServiceResult<string>.Failure("Course not found.");

                var alreadySaved = await _savedCourseRepository.AnyAsync(
                    sc => sc.UserId == userId && sc.CourseId == request.CourseId);
                if (alreadySaved)
                    return ServiceResult<string>.Failure("Course is already saved.");

                var savedCourse = new SavedCourse
                {
                    UserId = userId,
                    CourseId = request.CourseId,
                    Notes = request.Notes,
                    course = course,
                    SavedAt = DateTime.UtcNow
                };

                await _savedCourseRepository.AddAsync(savedCourse);
                await _savedCourseRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Course saved successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving job {JobId} for user {UserId}", request.CourseId, userId);
                return ServiceResult<string>.Failure($"Error saving job: {ex.Message}");
            }
        }

        public async Task<ServiceResult<List<SavedCourseRS>>> GetSavedCoursesAsync(string userId)
        {
            try
            {
                var items = await _savedCourseRepository.Query()
                    .Where(sc => sc.UserId == userId)
                    .Include(sc => sc.course)
                    .OrderByDescending(sj => sj.SavedAt)
                    .ToListAsync();

                return ServiceResult<List<SavedCourseRS>>.Success(_mapper.Map<List<SavedCourseRS>>(items));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving saved courses for user {UserId}", userId);
                return ServiceResult<List<SavedCourseRS>>.Failure("Error retrieving saved courses.");
            }
        }

        public async Task<ServiceResult<string>> RemoveSavedCourseAsync(string userId, int savedCourseId)
        {
            try
            {
                var savedCourse = await _savedCourseRepository.FirstOrDefaultAsync(
                    sc => sc.SavedCourseId == savedCourseId && sc.UserId == userId);

                if (savedCourse == null)
                    return ServiceResult<string>.Failure("Saved Course not found.");

                _savedCourseRepository.Remove(savedCourse);
                await _savedCourseRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Saved course removed.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing saved course {savedCourseId}", savedCourseId);
                return ServiceResult<string>.Failure($"Error removing saved course: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> IsSavedAsync(string userId, int courseId)
        {
            try
            {
                var isSaved = await _savedCourseRepository.AnyAsync(
                    sc => sc.UserId == userId && sc.CourseId == courseId);
                return ServiceResult<bool>.Success(isSaved);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking saved status for course {courseId}", courseId);
                return ServiceResult<bool>.Failure("Error checking saved status.");
            }
        }
    }
}

