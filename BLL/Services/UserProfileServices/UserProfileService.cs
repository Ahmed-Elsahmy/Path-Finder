using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using BLL.Common;
using BLL.Dtos.EducationDtos;
using BLL.Dtos.UserProfileDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.Extensions.Logging;

namespace BLL.Services.UserProfileServices
{
    public class UserProfileService:IUserProfileService
    {
        private readonly IRepository<UserProfile> _userprofileRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IMapper _mapper;
        private readonly ILogger<UserProfileService> _logger;

        private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB
        public UserProfileService(
           IRepository<UserProfile> userprofileRepository,
           IWebHostEnvironment env,
           IMapper mapper,
           ILogger<UserProfileService> logger)
        {
            _userprofileRepository = userprofileRepository;
            _env = env;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<UserProfileRS>> GetUserProfileAsync(string userId)
        {
            try
            {
                var profile = await _userprofileRepository
                 .FirstOrDefaultAsync(x => x.UserId == userId);

                if (profile == null)
                    return ServiceResult<UserProfileRS>.Failure("Profile not found");
                var result = _mapper.Map<UserProfileRS>(profile);
                return ServiceResult<UserProfileRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile for user {UserId}", userId);
                return ServiceResult<UserProfileRS>.Failure("Error retrieving profile.");
            }
        }

        public async Task<ServiceResult<string>> AddUserProfileAsync(string userId, UserProfileRQ request)
        {
            try
            {
                var existing = await _userprofileRepository.FirstOrDefaultAsync(x => x.UserId == userId);

                if (existing != null)
                    return ServiceResult<string>.Failure("User already has a profile.");

                var profile = _mapper.Map<UserProfile>(request);
                profile.UserId = userId;
                profile.UpdatedAt = DateTime.UtcNow;

                await _userprofileRepository.AddAsync(profile);
                await _userprofileRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Profile created successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating profile for user {UserId}", userId);
                return ServiceResult<string>.Failure($"Error creating profile: {ex.Message}");
            }
        }

        public async Task<ServiceResult<string>> UpdateUserProfileAsync(
     string userId,
     UpdateUserProfileRQ request,
     IFormCollection form)
        {
            try
            {
                var profile = await _userprofileRepository
                    .FirstOrDefaultAsync(x => x.UserId == userId);

                if (profile == null)
                    return ServiceResult<string>.Failure("Profile not found.");
                if (request.ProfilePictureUrl != null)
                {
                    var validationError = ValidateFile(request.ProfilePictureUrl);
                    if (validationError != null)
                        return ServiceResult<string>.Failure(validationError);
                    DeleteFileFromDisk(profile.ProfilePictureUrl);

                    var filePath = await UploadFileAsync(request.ProfilePictureUrl, userId);

                    profile.ProfilePictureUrl = filePath;
                }

                // UserName
                if (form.ContainsKey("UserName"))
                {
                    if (string.IsNullOrWhiteSpace(request.UserName))
                        return ServiceResult<string>.Failure("UserName cannot be empty.");
                    profile.UserName = request.UserName;
                }

                // FirstName
                if (form.ContainsKey("FirstName"))
                {
                    profile.FirstName = string.IsNullOrWhiteSpace(request.FirstName)
                        ? null
                        : request.FirstName;
                }

                // LastName
                if (form.ContainsKey("LastName"))
                {
                    profile.LastName = string.IsNullOrWhiteSpace(request.LastName)
                        ? null
                        : request.LastName;
                }

                // PhoneNumber
                if (form.ContainsKey("PhoneNumber"))
                {
                    profile.PhoneNumber = string.IsNullOrWhiteSpace(request.PhoneNumber)
                        ? null
                        : request.PhoneNumber;
                }

                // Bio
                if (form.ContainsKey("Bio"))
                {
                    profile.Bio = string.IsNullOrWhiteSpace(request.Bio)
                        ? null
                        : request.Bio;
                }

                // Location
                if (form.ContainsKey("Location"))
                {
                    profile.Location = string.IsNullOrWhiteSpace(request.Location)
                        ? null
                        : request.Location;
                }
                // DateOfBirth
                if (form.ContainsKey("DateOfBirth"))
                {
                    profile.DateOfBirth = request.DateOfBirth;
                }

                profile.UpdatedAt = DateTime.UtcNow;

                await _userprofileRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("Profile updated successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return ServiceResult<string>.Failure($"Error updating profile: {ex.Message}");
            }
        }

        private static string? ValidateFile(IFormFile file)
        {
            if (file.Length > MaxFileSizeBytes)
                return "File exceeds 5 MB.";

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(ext))
                return $"Invalid format. Allowed: {string.Join(", ", AllowedExtensions)}";

            return null;
        }
        private async Task<string> UploadFileAsync(IFormFile file, string userId)
        {
            var folder = Path.Combine(_env.WebRootPath, "Uploads", "profiles", userId);
            Directory.CreateDirectory(folder);

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            var fileName = $"{Guid.NewGuid()}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/Uploads/profiles/{userId}/{fileName}";
        }
        private void DeleteFileFromDisk(string? path)
        {
            if (string.IsNullOrEmpty(path)) return;

            var fullPath = Path.Combine(_env.WebRootPath, path.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
