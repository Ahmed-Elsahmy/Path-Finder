using AutoMapper;
using BLL.Common;
using BLL.Dtos.EducationDtos;
using BLL.Services.EducationServices;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace BLL.Services.EducationService
{
    public class EducationService : IEducationService
    {
        private readonly IRepository<UserEducation> _educationRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ILogger<EducationService> _logger;

        private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public EducationService(
            IRepository<UserEducation> educationRepository,
            IWebHostEnvironment env,
            IMapper mapper,
            ILogger<EducationService> logger)
        {
            _educationRepository = educationRepository;
            _env = env;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<UserEducationRS>>> GetUserEducationAsync(string userId)
        {
            try
            {
                var educations = await _educationRepository.FindAsync(e => e.UserId == userId);
                var ordered = educations.OrderByDescending(e => e.StartDate).ToList();
                var result = _mapper.Map<List<UserEducationRS>>(ordered);
                return ServiceResult<List<UserEducationRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving education for user {UserId}", userId);
                return ServiceResult<List<UserEducationRS>>.Failure("An error occurred while retrieving your education records.");
            }
        }

        public async Task<ServiceResult<string>> AddEducationAsync(string userId, EducationRQ request)
        {
            try
            {
                List<string> certificatePaths = new List<string>();

                if (request.Certificates != null && request.Certificates.Any())
                {
                    var validationError = ValidateFiles(request.Certificates);
                    if (validationError != null)
                        return ServiceResult<string>.Failure(validationError);

                    certificatePaths = await UploadFilesAsync(request.Certificates, userId);
                }

                var education = new UserEducation
                {
                    UserId = userId,
                    Institution = request.Institution,
                    Degree = request.Degree,
                    FieldOfStudy = request.FieldOfStudy,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate ?? DateTime.Now,
                    CertificatePaths = certificatePaths
                };

                if (education.EndDate < DateTime.Now)
                    education.IsCurrent = false;
                else
                    education.IsCurrent = true;

                await _educationRepository.AddAsync(education);
                await _educationRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Education added successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding education for user {UserId}", userId);
                return ServiceResult<string>.Failure($"An error occurred while adding education: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> UpdateEducationAsync(
      string userId,
      int educationId,
      UpdateEducationRQ request,
      IFormCollection form)
        {
            try
            {
                var education = await _educationRepository
                    .FirstOrDefaultAsync(e => e.EducationId == educationId && e.UserId == userId);

                if (education == null)
                    return ServiceResult<string>.Failure("Education record not found.");

                // Institution
                if (form.ContainsKey("Institution"))
                {
                    education.Institution = string.IsNullOrWhiteSpace(request.Institution)
                        ? null
                        : request.Institution;
                }

                // Degree
                if (form.ContainsKey("Degree"))
                {
                    education.Degree = string.IsNullOrWhiteSpace(request.Degree)
                        ? null
                        : request.Degree;
                }

                // FieldOfStudy
                if (form.ContainsKey("FieldOfStudy"))
                {
                    education.FieldOfStudy = string.IsNullOrWhiteSpace(request.FieldOfStudy)
                        ? null
                        : request.FieldOfStudy;
                }

                // StartDate
                if (form.ContainsKey("StartDate"))
                {
                    education.StartDate = request.StartDate;
                }

                // EndDate (allow null = delete)
                if (form.ContainsKey("EndDate"))
                {
                    education.EndDate = request.EndDate;
                }

                // Validation
                if (education.EndDate.HasValue && education.EndDate < education.StartDate)
                    return ServiceResult<string>.Failure("EndDate cannot be before StartDate");

                // IsCurrent logic
                education.IsCurrent = !education.EndDate.HasValue || education.EndDate >= DateTime.UtcNow;

                await _educationRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Education updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating education {EducationId} for user {UserId}", educationId, userId);
                return ServiceResult<string>.Failure($"An error occurred while updating education: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> DeleteEducationAsync(string userId, int educationId)
        {
            try
            {
                var education = await _educationRepository
                    .FirstOrDefaultAsync(e => e.EducationId == educationId && e.UserId == userId);

                if (education == null)
                    return ServiceResult<string>.Failure("Education record not found.");

                if (education.CertificatePaths != null)
                {
                    foreach (var path in education.CertificatePaths)
                        DeleteFileFromDisk(path);
                }

                _educationRepository.Remove(education);
                await _educationRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Education deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting education {EducationId} for user {UserId}", educationId, userId);
                return ServiceResult<string>.Failure($"An error occurred while deleting education: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> UploadCertificateAsync(string userId, int educationId, List<IFormFile> files)
        {
            try
            {
                var education = await _educationRepository
                    .FirstOrDefaultAsync(e => e.EducationId == educationId && e.UserId == userId);

                if (education == null)
                    return ServiceResult<string>.Failure("Education record not found.");

                if (files == null || !files.Any())
                    return ServiceResult<string>.Failure("No files provided.");

                if (education.CertificatePaths == null)
                    education.CertificatePaths = new List<string>();

                var validationError = ValidateFiles(files);
                if (validationError != null)
                    return ServiceResult<string>.Failure(validationError);

                var newPaths = await UploadFilesAsync(files, userId);
                education.CertificatePaths.AddRange(newPaths);

                await _educationRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Certificates uploaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading certificates for education {EducationId}, user {UserId}", educationId, userId);
                return ServiceResult<string>.Failure($"An error occurred while uploading certificates: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> DeleteCertificateAsync(string userId, int educationId, string certificateUrl)
        {
            try
            {
                var education = await _educationRepository
                    .FirstOrDefaultAsync(e => e.EducationId == educationId && e.UserId == userId);

                if (education == null)
                    return ServiceResult<string>.Failure("Education record not found.");

                if (education.CertificatePaths == null || !education.CertificatePaths.Contains(certificateUrl))
                    return ServiceResult<string>.Failure("Certificate not found on this record.");

                DeleteFileFromDisk(certificateUrl);
                education.CertificatePaths.Remove(certificateUrl);

                await _educationRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Certificate deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting certificate from education {EducationId}, user {UserId}", educationId, userId);
                return ServiceResult<string>.Failure($"An error occurred while deleting the certificate: {ex.Message}");
            }
        }
        // helper methods
        private static string? ValidateFiles(List<IFormFile> files)
        {
            foreach (var file in files)
            {
                if (file.Length > MaxFileSizeBytes)
                    return $"File {file.FileName} exceeds 5 MB.";

                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!AllowedExtensions.Contains(extension))
                    return $"File {file.FileName} is not a valid format. Allowed: {string.Join(", ", AllowedExtensions)}";
            }
            return null;
        }

        private async Task<List<string>> UploadFilesAsync(List<IFormFile> files, string userId)
        {
            var uploadFolder = Path.Combine(_env.WebRootPath, "Uploads", "certificates", userId);
            Directory.CreateDirectory(uploadFolder);

            var paths = new List<string>();
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var fullPath = Path.Combine(uploadFolder, uniqueFileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                paths.Add($"/Uploads/certificates/{userId}/{uniqueFileName}");
            }
            return paths;
        }
        private void DeleteFileFromDisk(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.TrimStart('/'));
            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}