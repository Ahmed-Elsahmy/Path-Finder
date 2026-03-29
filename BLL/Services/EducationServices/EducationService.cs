using BLL.Dtos.EducationDtos;
using BLL.Services.EducationServices;
using DAL.Helper;
using DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BLL.Services.EducationService
{
    public class EducationService : IEducationService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        private static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        private const long MaxFileSizeBytes = 5 * 1024 * 1024; // 5 MB

        public EducationService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<List<UserEducationRS>> GetUserEducationAsync(string userId)
        {
            try
            {
                return await _context.UserEducations
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.StartDate)
                    .Select(e => new UserEducationRS
                    {
                        EducationId = e.EducationId,
                        Institution = e.Institution,
                        Degree = e.Degree,
                        FieldOfStudy = e.FieldOfStudy,
                        StartDate = e.StartDate,
                        EndDate = e.EndDate,
                        IsCurrent = e.IsCurrent,
                        // Make sure your model uses List<string> for CertificatePaths
                        CertificateUrls = e.CertificatePaths ?? new List<string>()
                    })
                    .ToListAsync();
            }
            catch
            {
                return new List<UserEducationRS>();
            }
        }

        public async Task<string> AddEducationAsync(string userId, EducationRQ request)
        {
            try
            {
                List<string> certificatePaths = new List<string>();

                // Process Multiple Certificates
                if (request.Certificates != null && request.Certificates.Any())
                {
                    foreach (var file in request.Certificates)
                    {
                        if (file.Length > MaxFileSizeBytes) return $"File {file.FileName} exceeds 5 MB.";
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!AllowedExtensions.Contains(extension)) return $"File {file.FileName} is not a valid format.";

                        var uploadFolder = Path.Combine(_env.WebRootPath, "Uploads", "certificates", userId);
                        Directory.CreateDirectory(uploadFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                        var fullPath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        certificatePaths.Add($"/Uploads/certificates/{userId}/{uniqueFileName}");
                    }
                }

                var education = new UserEducation
                {
                    UserId = userId,
                    Institution = request.Institution,
                    Degree = request.Degree,
                    FieldOfStudy = request.FieldOfStudy,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate ?? DateTime.Now,
                    CertificatePaths = certificatePaths // Save the List
                };

                if (education.EndDate < DateTime.Now)
                    education.IsCurrent = false;
                else
                    education.IsCurrent = true;

                _context.UserEducations.Add(education);
                await _context.SaveChangesAsync();
                return "Education added successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> UpdateEducationAsync(string userId, int educationId, EducationRQ request)
        {
            try
            {
                var education = await _context.UserEducations
                    .FirstOrDefaultAsync(e => e.EducationId == educationId && e.UserId == userId);

                if (education == null) return "Education record not found.";

                education.Institution = request.Institution ?? education.Institution;
                education.Degree = request.Degree ?? education.Degree;
                education.FieldOfStudy = request.FieldOfStudy ?? education.FieldOfStudy;
                education.StartDate = request.StartDate ?? education.StartDate;
                education.EndDate = request.EndDate ?? education.EndDate;

                if (education.EndDate < DateTime.Now) education.IsCurrent = false;
                else education.IsCurrent = true;

                // If user uploads NEW certificates during update, we REPLACE the old ones
                if (request.Certificates != null && request.Certificates.Any())
                {
                    // 1. Delete old certificates from disk
                    if (education.CertificatePaths != null)
                    {
                        foreach (var oldPath in education.CertificatePaths)
                            DeleteFileFromDisk(oldPath);
                    }
                    education.CertificatePaths = new List<string>();

                    // 2. Upload new certificates
                    foreach (var file in request.Certificates)
                    {
                        if (file.Length > MaxFileSizeBytes) return $"File {file.FileName} exceeds 5 MB.";
                        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                        if (!AllowedExtensions.Contains(extension)) return $"File {file.FileName} is not a valid format.";

                        var uploadFolder = Path.Combine(_env.WebRootPath, "Uploads", "certificates", userId);
                        Directory.CreateDirectory(uploadFolder);

                        var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                        var fullPath = Path.Combine(uploadFolder, uniqueFileName);

                        using (var stream = new FileStream(fullPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        education.CertificatePaths.Add($"/Uploads/certificates/{userId}/{uniqueFileName}");
                    }
                }

                await _context.SaveChangesAsync();
                return "Education updated successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public async Task<string> DeleteEducationAsync(string userId, int educationId)
        {
            try
            {
                var education = await _context.UserEducations
                    .FirstOrDefaultAsync(e => e.EducationId == educationId && e.UserId == userId);

                if (education == null) return "Education record not found.";

                // Delete ALL certificates associated with this education from disk
                if (education.CertificatePaths != null)
                {
                    foreach (var path in education.CertificatePaths)
                        DeleteFileFromDisk(path);
                }

                _context.UserEducations.Remove(education);
                await _context.SaveChangesAsync();
                return "Education deleted successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Changed from single file to List<IFormFile>
        public async Task<string> UploadCertificateAsync(string userId, int educationId, List<IFormFile> files)
        {
            try
            {
                var education = await _context.UserEducations
                    .FirstOrDefaultAsync(e => e.EducationId == educationId && e.UserId == userId);

                if (education == null) return "Education record not found.";
                if (files == null || !files.Any()) return "No files provided.";

                if (education.CertificatePaths == null)
                    education.CertificatePaths = new List<string>();

                foreach (var file in files)
                {
                    if (file.Length > MaxFileSizeBytes) return $"File {file.FileName} exceeds 5 MB.";
                    var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!AllowedExtensions.Contains(extension)) return $"File {file.FileName} is not allowed.";

                    var uploadFolder = Path.Combine(_env.WebRootPath, "Uploads", "certificates", userId);
                    Directory.CreateDirectory(uploadFolder);

                    var uniqueFileName = $"{educationId}_{Guid.NewGuid()}{extension}";
                    var fullPath = Path.Combine(uploadFolder, uniqueFileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // Add to existing list
                    education.CertificatePaths.Add($"/Uploads/certificates/{userId}/{uniqueFileName}");
                }

                await _context.SaveChangesAsync();
                return "Certificates uploaded successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        // Updated to require the specific URL of the certificate you want to delete
        public async Task<string> DeleteCertificateAsync(string userId, int educationId, string certificateUrl)
        {
            try
            {
                var education = await _context.UserEducations
                    .FirstOrDefaultAsync(e => e.EducationId == educationId && e.UserId == userId);

                if (education == null) return "Education record not found.";
                if (education.CertificatePaths == null || !education.CertificatePaths.Contains(certificateUrl))
                    return "Certificate not found on this record.";

                // 1. Delete physical file
                DeleteFileFromDisk(certificateUrl);

                // 2. Remove from Database list
                education.CertificatePaths.Remove(certificateUrl);

                await _context.SaveChangesAsync();
                return "Certificate deleted successfully.";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
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