using AutoMapper;
using BLL.Common;
using BLL.Dtos.CareerPathCourseDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BLL.Services.CareerPathCourseServices
{
    public class CareerPathCourseService : ICareerPathCourseService
    {
        private readonly IRepository<CareerPathCourse> _careerPathCourseRepository;
        private readonly IRepository<CareerPath> _careerPathRepository;
        private readonly IRepository<Course> _courseRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CareerPathCourseService> _logger;

        public CareerPathCourseService(
            IRepository<CareerPathCourse> careerPathCourseRepository,
            IRepository<CareerPath> careerPathRepository,
            IRepository<Course> courseRepository,
            IMapper mapper,
            ILogger<CareerPathCourseService> logger)
        {
            _careerPathCourseRepository = careerPathCourseRepository;
            _careerPathRepository = careerPathRepository;
            _courseRepository = courseRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<CareerPathCourseRS>> CreateAsync(CareerPathCourseRQ request)
        {
            if (request == null)
                return ServiceResult<CareerPathCourseRS>.Failure("Invalid request.", ServiceErrorCode.ValidationError);

            if (request.CareerPathId <= 0 || request.CourseId <= 0 || request.OrderNumber <= 0)
                return ServiceResult<CareerPathCourseRS>.Failure("Invalid request data.", ServiceErrorCode.ValidationError);

            try
            {
                var careerPathExists = await _careerPathRepository.AnyAsync(cp => cp.CareerPathId == request.CareerPathId);
                if (!careerPathExists)
                    return ServiceResult<CareerPathCourseRS>.Failure("Career path not found.", ServiceErrorCode.NotFound);

                var courseExists = await _courseRepository.AnyAsync(c => c.Id == request.CourseId);
                if (!courseExists)
                    return ServiceResult<CareerPathCourseRS>.Failure("Course not found.", ServiceErrorCode.NotFound);

                var alreadyExists = await _careerPathCourseRepository.AnyAsync(x =>
                    x.CareerPathId == request.CareerPathId && x.CourseId == request.CourseId);

                if (alreadyExists)
                    return ServiceResult<CareerPathCourseRS>.Failure("Course already exists in this career path.", ServiceErrorCode.ValidationError);

                var orderAlreadyUsed = await _careerPathCourseRepository.AnyAsync(x =>
                    x.CareerPathId == request.CareerPathId && x.OrderNumber == request.OrderNumber);

                if (orderAlreadyUsed)
                    return ServiceResult<CareerPathCourseRS>.Failure("OrderNumber already used in this career path.", ServiceErrorCode.ValidationError);

                var entity = new CareerPathCourse
                {
                    CareerPathId = request.CareerPathId,
                    CourseId = request.CourseId,
                    OrderNumber = request.OrderNumber,
                    IsRequired = request.IsRequired,
                    CompletionCriteria = request.CompletionCriteria
                };

                await _careerPathCourseRepository.AddAsync(entity);
                await _careerPathCourseRepository.SaveChangesAsync();

                // Keep CareerPath.TotalCourses in sync
                var total = await _careerPathCourseRepository.Query()
                    .Where(x => x.CareerPathId == request.CareerPathId)
                    .CountAsync();

                var careerPath = await _careerPathRepository.GetByIdAsync(request.CareerPathId);
                if (careerPath != null)
                {
                    careerPath.TotalCourses = total;
                    await _careerPathRepository.SaveChangesAsync();
                }

                var created = await _careerPathCourseRepository.Query()
                    .Include(x => x.CareerPath)
                    .Include(x => x.Course)
                    .FirstOrDefaultAsync(x => x.CareerPathCourseId == entity.CareerPathCourseId);

                if (created == null)
                    return ServiceResult<CareerPathCourseRS>.Failure("Course added but could not be loaded.", ServiceErrorCode.UpstreamServiceError);

                return ServiceResult<CareerPathCourseRS>.Success(_mapper.Map<CareerPathCourseRS>(created));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating CareerPathCourse");
                return ServiceResult<CareerPathCourseRS>.Failure(
                    "An error occurred while adding the course to the career path.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<string>> DeleteAsync(int id)
        {
            if (id <= 0)
                return ServiceResult<string>.Failure("Invalid id.", ServiceErrorCode.ValidationError);

            try
            {
                var entity = await _careerPathCourseRepository.GetByIdAsync(id);

                if (entity == null)
                    return ServiceResult<string>.Failure("Career path course not found.", ServiceErrorCode.NotFound);

                var careerPathId = entity.CareerPathId;

                _careerPathCourseRepository.Remove(entity);
                await _careerPathCourseRepository.SaveChangesAsync();

                // Keep CareerPath.TotalCourses in sync
                var total = await _careerPathCourseRepository.Query()
                    .Where(x => x.CareerPathId == careerPathId)
                    .CountAsync();

                var careerPath = await _careerPathRepository.GetByIdAsync(careerPathId);
                if (careerPath != null)
                {
                    careerPath.TotalCourses = total;
                    await _careerPathRepository.SaveChangesAsync();
                }

                return ServiceResult<string>.Success("Deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CareerPathCourse {Id}", id);
                return ServiceResult<string>.Failure("Error while deleting", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<List<CareerPathCourseRS>>> GetByCareerPathIdAsync(int careerPathId)
        {
            try
            {
                var data = await _careerPathCourseRepository.Query()
                    .Where(x => x.CareerPathId == careerPathId)
                    .Include(x => x.CareerPath)
                    .Include(x => x.Course)
                    .ToListAsync();
                var result = data
                    .OrderBy(x => x.OrderNumber)
                    .Select(x => new CareerPathCourseRS
                    {
                        CareerPathCourseId = x.CareerPathCourseId,
                        CareerPathId = x.CareerPathId,
                        CareerPathName = x.CareerPath.PathName,
                        CourseId = x.CourseId,
                        CourseName = x.Course.Name,
                        OrderNumber = x.OrderNumber,
                        IsRequired = x.IsRequired,
                        CompletionCriteria = x.CompletionCriteria
                    }).ToList();

                return ServiceResult<List<CareerPathCourseRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CareerPathCourses for CareerPath {Id}", careerPathId);
                return ServiceResult<List<CareerPathCourseRS>>.Failure("Error retrieving data", ServiceErrorCode.UpstreamServiceError);
            }
        }
    }
}
