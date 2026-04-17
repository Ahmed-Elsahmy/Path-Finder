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

        //    public async Task<ServiceResult<string>> CreateAsync(CareerPathCourseRQ request)
        //    {
        //        try
        //        {
        //            // ✅ Validation
        //            var careerPath = await _careerPathRepository.GetByIdAsync(request.CareerPathId);
        //            if (careerPath == null)
        //                return ServiceResult<string>.Failure("CareerPath not found");

        //            var course = await _courseRepository.GetByIdAsync(request.CourseId);
        //            if (course == null)
        //                return ServiceResult<string>.Failure("Course not found");

        //            var exists = await _careerPathCourseRepository.FirstOrDefaultAsync(x =>
        //                x.CareerPathId == request.CareerPathId &&
        //                x.CourseId == request.CourseId);

        //            if (exists != null)
        //                return ServiceResult<string>.Failure("Course already exists in this CareerPath");

        //            var duplicateOrder = await _careerPathCourseRepository.FirstOrDefaultAsync(x =>
        //x.CareerPathId == request.CareerPathId &&
        //x.OrderNumber == request.OrderNumber);

        //            if (duplicateOrder != null)
        //                return ServiceResult<string>.Failure("OrderNumber already used in this CareerPath");

        //            // ✅ Mapping
        //            var entity = _mapper.Map<CareerPathCourse>(request);

        //            await _careerPathCourseRepository.AddAsync(entity);
        //            await _careerPathCourseRepository.SaveChangesAsync();

        //            return ServiceResult<string>.Success("Course added to CareerPath successfully");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error creating CareerPathCourse");
        //            return ServiceResult<string>.Failure($"Error: {ex.Message}");
        //        }
        //    }

        //    public async Task<ServiceResult<string>> DeleteAsync(int id)
        //    {
        //        try
        //        {
        //            var entity = await _careerPathCourseRepository.GetByIdAsync(id);

        //            if (entity == null)
        //                return ServiceResult<string>.Failure("CareerPathCourse not found");

        //            _careerPathCourseRepository.Remove(entity);
        //            await _careerPathCourseRepository.SaveChangesAsync();

        //            return ServiceResult<string>.Success("Deleted successfully");
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger.LogError(ex, "Error deleting CareerPathCourse {Id}", id);
        //            return ServiceResult<string>.Failure("Error while deleting");
        //        }
        //    }

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
                return ServiceResult<List<CareerPathCourseRS>>.Failure("Error retrieving data");
            }
        }
    }
}
