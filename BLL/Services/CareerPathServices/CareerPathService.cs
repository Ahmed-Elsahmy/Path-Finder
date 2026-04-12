using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Common;
using BLL.Dtos.CareerPathDtos;
using BLL.Dtos.UserProfileDtos;
using DAL.Helper.Enums;
using DAL.Models;
using DAL.Repository;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace BLL.Services.CareerPathServices
{
    public class CareerPathService : ICareerPathService
    {
        private readonly IRepository<CareerPath> _careerpathRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ILogger<CareerPathService> _logger;
        public CareerPathService(IRepository<CareerPath> careerpathRepository, IWebHostEnvironment env, IMapper mapper, ILogger<CareerPathService> logger)
        {
            _careerpathRepository = careerpathRepository;
            _env = env;
            _mapper = mapper;
            _logger = logger;
        }
        public async Task<ServiceResult<string>> CreateCareerPathAsync(CareerPathRQ request)
        {
            try
            {
                // 1. Validation
                if (string.IsNullOrWhiteSpace(request.CarrerPathName))
                    return ServiceResult<string>.Failure("Path name is required");
                var existing = await _careerpathRepository
                    .FirstOrDefaultAsync(x => x.PathName == request.CarrerPathName);

                if (existing != null)
                    return ServiceResult<string>.Failure("Career path already exists");


                var careerPath = _mapper.Map<CareerPath>(request);

                await _careerpathRepository.AddAsync(careerPath);
                await _careerpathRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Career path created successfully");
            }
            catch (Exception ex)
            {
                return ServiceResult<string>.Failure($"Error: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> DeleteCareerPathAsync(int id)
        {

            try
            {
                var careerpath = await _careerpathRepository.GetByIdAsync(id);
                if (careerpath == null)
                {
                    return ServiceResult<string>.Failure("CareerpPath not found.");
                }
                _careerpathRepository.Remove(careerpath);
                await _careerpathRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("CareerpPath deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CareerpPath {CareerpPath}", id);

                return ServiceResult<string>
                    .Failure("An error occurred while deleting the CareerpPath.");
            }
        }

        public async Task<ServiceResult<List<CareerPathRS>>> GetAllCareerPathsAsync()
        {
            try
            {
                var careerpaths = await _careerpathRepository.GetAllAsync();
                var result = _mapper.Map<List<CareerPathRS>>(careerpaths);
                return ServiceResult<List<CareerPathRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CarrerPaths");
                return ServiceResult<List<CareerPathRS>>.Failure("Error retrieving CarrerPaths.");
            }
        }

        public async Task<ServiceResult<CareerPathRS>> GetCareerPathByIdAsync(int id)
        {
            try
            {
                var careerpath = await _careerpathRepository.FirstOrDefaultAsync(c => c.CareerPathId == id);
                var result = _mapper.Map<CareerPathRS>(careerpath);
                return ServiceResult<CareerPathRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving CarrerPath with id {id}");
                return ServiceResult<CareerPathRS>.Failure($"Error retrieving CarrerPath with id {id}.");
            }
        }

        public async Task<ServiceResult<string>> UpdateCareerPathAsync(int id, UpdateCareerPathRQ request)
        {
            try
            {
                var experience = await _careerpathRepository
                    .GetByIdAsync(id);
                if (experience == null)
                {
                    return ServiceResult<string>.Failure("CareerPath not found.");
                }
                _mapper.Map(request, experience);

                await _careerpathRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("CareerPath updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating CareerPath {CareerPathID}", id);

                return ServiceResult<string>
                    .Failure("An error occurred while updating the CareerPath.");
            }

        }
    }
}
