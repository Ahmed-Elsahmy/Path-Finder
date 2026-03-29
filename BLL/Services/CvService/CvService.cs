using BLL.Dtos;
using BLL.Dtos.CvDtos;
using DAL.Helper;
using DAL.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace BLL.Services.CvService
{
    public class CvService : ICvService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        public CvService(AppDbContext context, IWebHostEnvironment env,IConfiguration config)
        {
            _context = context;
            _env = env;
            _config= config;
        }
        public async Task<string> UploadCvAsync(string userId, UploadCvRQ request, string baseUrl)
        {
            try
            {
                var extension = Path.GetExtension(request.File.FileName).ToLower();
                if (extension != ".pdf" && extension != ".doc" && extension != ".docx")
                    return "Invalid file format. Only PDF and Word documents are allowed.";

                string uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "CVs");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await request.File.CopyToAsync(fileStream);
                }

                if (request.IsPrimary)
                {
                    var existingCvs = await _context.CVs.Where(c => c.UserId == userId).ToListAsync();
                    foreach (var existingCv in existingCvs) existingCv.IsPrimary = false;
                }

                // Default values as lists
                string parsedContent = "AI Parsing Failed or not a PDF";
                List<string> extractedSkills = new List<string>();
                List<string> cvIssues = new List<string>();

                try
                {
                    if (extension == ".pdf")
                    {
                        string cvText = "";
                        using (var pdf = PdfDocument.Open(filePath))
                        {
                            foreach (var page in pdf.GetPages())
                                cvText += ContentOrderTextExtractor.GetText(page) + " ";
                        }

                        if (!string.IsNullOrWhiteSpace(cvText))
                        {
                            // Now receives 3 values including CVIssues
                            var aiResult = await ExtractDataWithGeminiAsync(cvText);
                            parsedContent = aiResult.ParsedContent;
                            extractedSkills = aiResult.ExtractedSkills;
                            cvIssues = aiResult.CVIssues;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("AI Extraction Error: " + ex.Message);
                    cvIssues = new List<string> { $"AI Extraction Error: {ex.Message}" };
                }

                var cv = new CV
                {
                    UserId = userId,
                    FileName = request.File.FileName,
                    FileUrl = $"{baseUrl}/Uploads/CVs/{uniqueFileName}",
                    IsPrimary = request.IsPrimary,
                    UploadedAt = DateTime.UtcNow,
                    ParsedContent = parsedContent,
                    ExtractedSkills = extractedSkills,  // Now List<string>
                    CVIssues = cvIssues                 // Now List<string>
                };

                _context.CVs.Add(cv);
                await _context.SaveChangesAsync();
                if (extractedSkills.Any())
                {
                    foreach (var skill in extractedSkills)
                    {
                        var globalskill = await _context.Skills.FirstOrDefaultAsync(s => s.SkillName.ToLower() == skill.ToLower());
                        if(globalskill == null)
                        {
                            globalskill = new Skill { SkillName = skill , Category = "Ai Extracted" , IsTechnical = true };
                            _context.Skills.Add(globalskill);
                            await _context.SaveChangesAsync();
                        }
                        var userAlreadyHasSkill = await _context.UserSkills
                           .AnyAsync(us => us.UserId == userId && us.SkillId == globalskill.SkillId);
                        if (!userAlreadyHasSkill)
                        {
                            var newUserSkill = new UserSkill
                            {
                                UserId = userId,
                                SkillId = globalskill.SkillId,
                                ProficiencyLevel = "Not Specified",
                                Source = "AI Extracted from CV",
                                AcquiredDate = DateTime.UtcNow
                            };
                            _context.UserSkills.Add(newUserSkill);
                        }
                    }
                    await _context.SaveChangesAsync();
                }
                return "CV Uploaded Successfully";

            }
            catch (Exception ex)
            {
                return $"Error uploading CV: {ex.Message}";
            }
        }
        // private method to call Gemini API and extract summary and skills from CV text
        private async Task<(string ParsedContent, List<string> ExtractedSkills, List<string> CVIssues)> ExtractDataWithGeminiAsync(string cvText)
        {
            try
            {
                var apiKey = _config["Gemini:ApiKey"];

                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
                    return ("API Error: Key Missing", new List<string>(), new List<string> { "Make sure Gemini:ApiKey is in appsettings.json" });

                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={apiKey}";

                var prompt = $@"
Analyze the following CV. Return ONLY a valid JSON object without any markdown codes.

The JSON must have exactly three keys:
""ParsedContent"": A 3-sentence summary of the candidate's experience and profession.
""ExtractedSkills"": A JSON array of strings, each being a technical or soft skill found in the CV.
""CVIssues"": A JSON array of strings, each describing a specific problem, weakness, or missing element in the CV.

Instructions:
- Identify missing sections (e.g., no projects, no experience, no education details).
- Highlight weak descriptions (e.g., vague responsibilities, no measurable achievements).
- Detect lack of required skills for the role if inferable.
- Mention formatting or clarity issues if present.
- Be specific and actionable.
- If no issues are found, return an empty array [].

CV Text: {cvText}";
                 
                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } }
                };

                using var client = new HttpClient();
                var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        using var document = JsonDocument.Parse(responseString);
                        var aiResponseText = document.RootElement
                            .GetProperty("candidates")[0]
                            .GetProperty("content")
                            .GetProperty("parts")[0]
                            .GetProperty("text").GetString();

                        aiResponseText = aiResponseText.Replace("```json", "").Replace("```", "").Trim();

                        using var aiDoc = JsonDocument.Parse(aiResponseText);
                        var root = aiDoc.RootElement;

                        // Parse ParsedContent
                        var parsedContent = root.TryGetProperty("ParsedContent", out var pc)
                            ? pc.GetString() ?? "Could not summarize"
                            : "Could not summarize";

                        // Parse ExtractedSkills as List<string>
                        var extractedSkills = new List<string>();
                        if (root.TryGetProperty("ExtractedSkills", out var skillsEl) && skillsEl.ValueKind == JsonValueKind.Array)
                            extractedSkills = skillsEl.EnumerateArray()
                                .Select(s => s.GetString() ?? "")
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToList();

                        // Parse CVIssues as List<string>
                        var cvIssues = new List<string>();
                        if (root.TryGetProperty("CVIssues", out var issuesEl) && issuesEl.ValueKind == JsonValueKind.Array)
                            cvIssues = issuesEl.EnumerateArray()
                                .Select(s => s.GetString() ?? "")
                                .Where(s => !string.IsNullOrEmpty(s))
                                .ToList();

                        return (parsedContent, extractedSkills, cvIssues);
                    }
                    catch (Exception ex)
                    {
                        return (
                            "AI Format Error",
                            new List<string>(),
                            new List<string> { $"Parsing failed: {ex.Message}", $"Gemini returned: {responseString}" }
                        );
                    }
                }

                return (
                    $"API Error ({response.StatusCode})",
                    new List<string>(),
                    new List<string> { responseString }
                );
            }
            catch (Exception ex)
            {
                return (
                    "AI Request Failed",
                    new List<string>(),
                    new List<string> { $"Request failed: {ex.Message}" }
                );

            }
        }

        public async Task<List<CvRS>> GetUserCvsAsync(string userId)
        {
            try
            {
                return await _context.CVs
                .Where(c => c.UserId == userId)
                .Select(c => new CvRS
                {
                    CVId = c.CVId,
                    FileName = c.FileName,
                    FileUrl = c.FileUrl,
                    IsPrimary = c.IsPrimary,
                    UploadedAt = c.UploadedAt,
                    ParsedContent = c.ParsedContent,
                    ExtractedSkills = c.ExtractedSkills ?? new List<string>(),
                    CVIssues = c.CVIssues ?? new List<string>()               
                })
                   .ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<CvRS>();
            }
        }

        public async Task<string> SetPrimaryCvAsync(string userId, int cvId)
        {
            try
            {
                var cvs = await _context.CVs.Where(c => c.UserId == userId).ToListAsync();
                var targetCv = cvs.FirstOrDefault(c => c.CVId == cvId);

                if (targetCv == null) return "CV not found.";

                foreach (var cv in cvs)
                {
                    cv.IsPrimary = (cv.CVId == cvId);
                }

                await _context.SaveChangesAsync();
                return "Primary CV updated.";
            }
            catch (Exception ex)
            {
                return $"Error setting primary CV: {ex.Message}";
            }
        }

        public async Task<string> DeleteCvAsync(string userId, int cvId)
        {
            try
            {
                var cv = await _context.CVs.FirstOrDefaultAsync(c => c.CVId == cvId && c.UserId == userId);
                if (cv == null) return "CV not found.";

                // Delete physical file
                var fileName = Path.GetFileName(cv.FileUrl);
                var filePath = Path.Combine(_env.WebRootPath, "Uploads", "CVs", fileName);
                if (File.Exists(filePath)) File.Delete(filePath);

                // Delete from DB
                _context.CVs.Remove(cv);
                await _context.SaveChangesAsync();

                return "CV Deleted Successfully.";
            }
            catch (Exception ex)
            {
                return $"Error deleting CV: {ex.Message}";
            }
        }
    }
}