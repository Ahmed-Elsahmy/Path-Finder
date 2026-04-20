using BLL.Common;
using BLL.Dtos.AiDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BLL.Services.ResumeBuilderService
{
    public class ResumeBuilderService : IResumeBuilderService
    {
        private readonly IRepository<UserProfile> _profileRepo;
        private readonly IRepository<UserSkill> _userSkillRepo;
        private readonly IRepository<UserEducation> _educationRepo;
        private readonly IRepository<UserExperience> _experienceRepo;
        private readonly IRepository<CV> _cvRepo;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ResumeBuilderService> _logger;

        private const string GeminiBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        public ResumeBuilderService(
            IRepository<UserProfile> profileRepo,
            IRepository<UserSkill> userSkillRepo,
            IRepository<UserEducation> educationRepo,
            IRepository<UserExperience> experienceRepo,
            IRepository<CV> cvRepo,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            ILogger<ResumeBuilderService> logger)
        {
            _profileRepo = profileRepo;
            _userSkillRepo = userSkillRepo;
            _educationRepo = educationRepo;
            _experienceRepo = experienceRepo;
            _cvRepo = cvRepo;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        // ==========================================
        // 1. توليد الـ CV كنص JSON (للعرض المسبق في الفرونت إند)
        // ==========================================
        public async Task<ServiceResult<ResumeBuilderRS>> GenerateResumeAsync(
            string userId, ResumeBuilderRQ request, CancellationToken cancellationToken = default)
        {
            try
            {
                var profile = await _profileRepo.FirstOrDefaultAsync(p => p.UserId == userId);
                if (profile == null)
                    return ServiceResult<ResumeBuilderRS>.Failure("Please complete your profile before generating a resume.", ServiceErrorCode.ValidationError);

                var skills = await _userSkillRepo.Query().Where(s => s.UserId == userId).Include(s => s.Skill).ToListAsync(cancellationToken);
                var education = await _educationRepo.FindAsync(e => e.UserId == userId);
                var experiences = await _experienceRepo.FindAsync(e => e.UserId == userId);
                var latestCv = await _cvRepo.Query().Where(c => c.UserId == userId).OrderByDescending(c => c.UploadedAt).FirstOrDefaultAsync(cancellationToken);

                var userData = BuildUserDataContext(profile, skills, education, experiences, latestCv);
                var result = await GenerateWithGeminiAsync(userData, request, cancellationToken);

                if (result == null)
                    return ServiceResult<ResumeBuilderRS>.Failure("AI resume generation failed. Please try again.", ServiceErrorCode.UpstreamServiceError);

                return ServiceResult<ResumeBuilderRS>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating resume for user {UserId}", userId);
                return ServiceResult<ResumeBuilderRS>.Failure("An unexpected error occurred while generating your resume.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<(byte[] PdfBytes, string FullName)>> GenerateResumePdfAsync(
            string userId, ResumeBuilderRQ request, CancellationToken cancellationToken = default)
        {
            try
            {
                var aiResult = await GenerateResumeAsync(userId, request, cancellationToken);

                if (!aiResult.IsSuccess || aiResult.Data == null)
                    return ServiceResult<(byte[], string)>.Failure(aiResult.ErrorMessage ?? "Failed to generate resume data.", aiResult.ErrorCode);

                var realFullName = aiResult.Data.FullName ?? "My";

                byte[] pdfBytes = ResumePdfGenerator.Generate(aiResult.Data, request.Style);

                return ServiceResult<(byte[], string)>.Success((pdfBytes, realFullName));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF resume for user {UserId}", userId);
                return ServiceResult<(byte[], string)>.Failure("An unexpected error occurred while mapping PDF.", ServiceErrorCode.UpstreamServiceError);
            }
        }


        private string BuildUserDataContext(UserProfile profile, List<UserSkill> skills, List<UserEducation> education, List<UserExperience> experiences, CV? latestCv)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Name: {profile.FirstName} {profile.LastName}");
            sb.AppendLine($"Email: {profile.PhoneNumber} | Location: {profile.Location} | Bio: {profile.Bio}\n");

            sb.AppendLine("SKILLS:");
            foreach (var group in skills.GroupBy(s => s.Skill?.Category ?? "Other"))
            {
                sb.AppendLine($"{group.Key}: {string.Join(", ", group.Select(s => s.Skill?.SkillName))}");
            }

            sb.AppendLine("\nEXPERIENCE:");
            foreach (var exp in experiences.OrderByDescending(e => e.StartDate))
            {
                var endStr = exp.IsCurrent ? "Present" : (exp.EndDate?.ToString("MMM yyyy") ?? "N/A");
                sb.AppendLine($"{exp.Position} at {exp.CompanyName} ({exp.StartDate?.ToString("MMM yyyy")} - {endStr})");
                sb.AppendLine($"Desc: {exp.Description}");
            }

            sb.AppendLine("\nEDUCATION:");
            foreach (var edu in education.OrderByDescending(e => e.StartDate))
            {
                var endStr = edu.IsCurrent ? "Present" : (edu.EndDate?.ToString("yyyy") ?? "N/A");
                sb.AppendLine($"{edu.Degree} in {edu.FieldOfStudy} - {edu.Institution} ({endStr})");
            }

            if (latestCv != null)
                sb.AppendLine($"\nOLD CV SUMMARY: {latestCv.ParsedContent}");

            return sb.ToString();
        }

        private async Task<ResumeBuilderRS?> GenerateWithGeminiAsync(string userData, ResumeBuilderRQ request, CancellationToken ct)
        {
            try
            {
                var apiKey = _config["Gemini:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey)) return null;

                var prompt = $@"
You are an expert resume writer. Generate a professional resume based purely on the user's data provided below.
Refine bullet points using action verbs. Correct grammar. 

USER DATA:
{userData}

Preferences: Target Job: {request.TargetJobTitle ?? "Not specific"}, Style: {request.Style}, Language: {request.Language}.
Return ONLY a valid JSON object tracking the schema defined. Do NOT use markdown code blocks like ```json.

Schema:
{{
  ""FullName"": """", ""Email"": """", ""Phone"": """", ""Location"": """",
  ""ProfessionalSummary"": """",
  ""SkillSections"":[{{ ""Category"": """", ""Skills"": [] }}],
  ""Experience"":[{{ ""Position"": """", ""Company"": """", ""Duration"": """", ""BulletPoints"": [] }}],
  ""Education"":[{{ ""Degree"": """", ""Institution"": """", ""Duration"": """", ""FieldOfStudy"": """" }}],
  ""Certifications"":[], ""AdditionalSections"": """",
  ""FullResumeText"": """", ""AITips"": """"
}}";
                var body = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                    generationConfig = new { temperature = 0.4, responseMimeType = "application/json" }
                };

                var client = _httpClientFactory.CreateClient("GeminiClient");
                client.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", apiKey);

                var httpContent = new StringContent(JsonSerializer.Serialize(body), Encoding.UTF8, "application/json");
                var response = await client.PostAsync(GeminiBaseUrl, httpContent, ct);

                if (!response.IsSuccessStatusCode) return null;

                var raw = await response.Content.ReadAsStringAsync(ct);
                using var doc = JsonDocument.Parse(raw);
                var aiText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

                aiText = aiText?.Replace("```json", "").Replace("```", "").Trim();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                return JsonSerializer.Deserialize<ResumeBuilderRS>(aiText ?? "{}", options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini API failed");
                return null;
            }
        }
    }
}