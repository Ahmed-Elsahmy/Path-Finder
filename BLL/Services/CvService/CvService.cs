using AutoMapper;
using BLL.Common;
using BLL.Dtos.CvDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace BLL.Services.CvService
{
    public class CvService : ICvService
    {
        private readonly IRepository<CV> _cvRepository;
        private readonly IRepository<Skill> _skillRepository;
        private readonly IRepository<UserSkill> _userSkillRepository;
        private readonly IWebHostEnvironment _env;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IMapper _mapper;
        private readonly ILogger<CvService> _logger;

        private const string GeminiBaseUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        public CvService(
            IRepository<CV> cvRepository,
            IRepository<Skill> skillRepository,
            IRepository<UserSkill> userSkillRepository,
            IWebHostEnvironment env,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            IMapper mapper,
            ILogger<CvService> logger)
        {
            _cvRepository = cvRepository;
            _skillRepository = skillRepository;
            _userSkillRepository = userSkillRepository;
            _env = env;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _mapper = mapper;
            _logger = logger;
        }
        private async Task<HttpResponseMessage> PostToGeminiAsync(
            object requestBody,
            CancellationToken ct = default)
        {
            var apiKey = _config["Gemini:ApiKey"];
            var client = _httpClientFactory.CreateClient("GeminiClient");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", apiKey);

            var httpContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json");

            HttpResponseMessage response = null!;
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                response = await client.PostAsync(GeminiBaseUrl, httpContent, ct);

                if (response.IsSuccessStatusCode) break;

                if ((int)response.StatusCode == 503 && attempt < 3)
                {
                    _logger.LogWarning("Gemini 503 on attempt {Attempt}, retrying in 3s...", attempt);
                    await Task.Delay(3000, ct);
                }
                else break;
            }

            return response;
        }
        public async Task<ServiceResult<string>> UploadCvAsync(string userId, UploadCvRQ request, string baseUrl)
        {
            try
            {
                var extension = Path.GetExtension(request.File.FileName).ToLower();
                if (extension != ".pdf" && extension != ".doc" && extension != ".docx")
                    return ServiceResult<string>.Failure("Invalid file format. Only PDF and Word documents are allowed.");

                if (request.File.Length > 10 * 1024 * 1024)
                    return ServiceResult<string>.Failure("File size exceeds 10 MB limit.");

                string uploadsFolder = Path.Combine(_env.WebRootPath, "Uploads", "CVs");
                if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + request.File.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await request.File.CopyToAsync(fileStream);

                if (request.IsPrimary)
                {
                    var existingCvs = await _cvRepository.FindAsync(c => c.UserId == userId);
                    foreach (var existingCv in existingCvs) existingCv.IsPrimary = false;
                    await _cvRepository.SaveChangesAsync();
                }

                string parsedContent = "AI Parsing Failed or not a PDF";
                List<string> extractedSkills = new List<string>();
                List<string> cvIssues = new List<string>();
                int cvScore = 0;
                List<string> suggestedJobTitles = new List<string>();
                List<string> recommendedSkills = new List<string>();

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
                            var aiResult = await ExtractDataWithGeminiAsync(cvText);
                            parsedContent = aiResult.ParsedContent;
                            extractedSkills = aiResult.ExtractedSkills;
                            cvIssues = aiResult.CVIssues;
                            cvScore = aiResult.CVScore;
                            suggestedJobTitles = aiResult.SuggestedJobTitles;
                            recommendedSkills = aiResult.RecommendedSkills;
                        }
                    }
                    else if (extension == ".docx")
                    {
                        string cvText = ExtractTextFromDocx(filePath);

                        if (!string.IsNullOrWhiteSpace(cvText))
                        {
                            var aiResult = await ExtractDataWithGeminiAsync(cvText);
                            parsedContent = aiResult.ParsedContent;
                            extractedSkills = aiResult.ExtractedSkills;
                            cvIssues = aiResult.CVIssues;
                            cvScore = aiResult.CVScore;
                            suggestedJobTitles = aiResult.SuggestedJobTitles;
                            recommendedSkills = aiResult.RecommendedSkills;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "AI extraction failed for CV uploaded by user {UserId}", userId);
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
                    ExtractedSkills = extractedSkills,
                    CVIssues = cvIssues,
                    CVScore = cvScore,
                    SuggestedJobTitles = suggestedJobTitles,
                    RecommendedSkills = recommendedSkills
                };

                await _cvRepository.AddAsync(cv);
                await _cvRepository.SaveChangesAsync();

                if (extractedSkills.Any())
                {
                    var allGlobalSkills = await _skillRepository.GetAllAsync();
                    var userSkillIds = (await _userSkillRepository.FindAsync(us => us.UserId == userId))
                        .Select(us => us.SkillId)
                        .ToHashSet();

                    foreach (var skill in extractedSkills)
                    {
                        var skillLower = skill.ToLower().Trim();

                        var globalskill = allGlobalSkills.FirstOrDefault(s =>
                        {
                            var globalLower = s.SkillName.ToLower().Trim();
                            // Exact match only — prevents "C" from matching "C++", "CSS", etc.
                            return globalLower == skillLower;
                        });

                        if (globalskill == null)
                        {
                            globalskill = new Skill { SkillName = skill, Category = "Ai Extracted", IsTechnical = true };
                            await _skillRepository.AddAsync(globalskill);
                            await _skillRepository.SaveChangesAsync();
                            allGlobalSkills.Add(globalskill);
                        }

                        if (!userSkillIds.Contains(globalskill.SkillId))
                        {
                            await _userSkillRepository.AddAsync(new UserSkill
                            {
                                UserId = userId,
                                SkillId = globalskill.SkillId,
                                ProficiencyLevel = "Not Specified",
                                Source = "AI Extracted from CV",
                                AcquiredDate = DateTime.UtcNow
                            });
                            userSkillIds.Add(globalskill.SkillId);
                        }
                    }
                    await _userSkillRepository.SaveChangesAsync();
                }

                return ServiceResult<string>.Success("CV Uploaded Successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading CV for user {UserId}", userId);
                return ServiceResult<string>.Failure($"Error uploading CV: {ex.Message}");
            }
        }
        private async Task<CvAiResult> ExtractDataWithGeminiAsync(string cvText)
        {
            var emptyResult = new CvAiResult();
            try
            {
                var apiKey = _config["Gemini:ApiKey"];
                if (string.IsNullOrEmpty(apiKey) || apiKey == "YOUR_GEMINI_API_KEY_HERE")
                {
                    emptyResult.ParsedContent = "API Error: Key Missing";
                    emptyResult.CVIssues.Add("Make sure Gemini:ApiKey is in appsettings.json or User Secrets");
                    return emptyResult;
                }

                var prompt = $@"
Analyze the following CV/Resume thoroughly. Return a JSON object with these exact keys:

""ParsedContent"": A 3-sentence professional summary of the candidate's experience and profession.

""ExtractedSkills"": A JSON array of short, canonical skill names found in the CV.
  - Use standard abbreviations (e.g., ""C++"" not ""C++ Programming"", ""React"" not ""React.js Development"").
  - Include both technical and soft skills.

""CVIssues"": A JSON array of specific, actionable problems found in the CV:
  - Missing sections (projects, experience, education, contact info).
  - Weak descriptions (vague responsibilities, no measurable achievements).
  - Formatting or clarity issues.
  - If no issues found, return an empty array [].

""CVScore"": An integer from 0 to 100 rating the overall quality of this CV. Consider:
  - Completeness of sections (20 points)
  - Quality of descriptions and achievements (25 points)
  - Skills relevance and depth (20 points)
  - Formatting and readability (15 points)
  - Overall professionalism (20 points)

""SuggestedJobTitles"": A JSON array of 3-5 job titles this candidate is best suited for based on their skills and experience.

""RecommendedSkills"": A JSON array of 3-7 skills the candidate should learn next to advance their career, based on their current skill set and industry trends. Be specific (e.g., ""Docker"" not ""DevOps tools"").

CV Text:
{cvText}";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                    generationConfig = new
                    {
                        temperature = 0.2,
                        responseMimeType = "application/json"
                    }
                };

                var response = await PostToGeminiAsync(requestBody);
                var responseString = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Gemini API returned error {StatusCode}: {Response}", response.StatusCode, responseString);
                    emptyResult.ParsedContent = $"API Error ({response.StatusCode})";
                    emptyResult.CVIssues.Add(responseString);
                    return emptyResult;
                }

                try
                {
                    using var document = JsonDocument.Parse(responseString);
                    var aiResponseText = document.RootElement
                        .GetProperty("candidates")[0]
                        .GetProperty("content")
                        .GetProperty("parts")[0]
                        .GetProperty("text").GetString();

                    aiResponseText = aiResponseText?.Replace("```json", "").Replace("```", "").Trim();

                    using var aiDoc = JsonDocument.Parse(aiResponseText!);
                    var root = aiDoc.RootElement;

                    var result = new CvAiResult
                    {
                        ParsedContent = root.TryGetProperty("ParsedContent", out var pc)
                            ? pc.GetString() ?? "Could not summarize" : "Could not summarize",

                        CVScore = root.TryGetProperty("CVScore", out var scoreEl)
                            ? scoreEl.TryGetInt32(out var score) ? score : 0 : 0,
                    };

                    if (root.TryGetProperty("ExtractedSkills", out var skillsEl) && skillsEl.ValueKind == JsonValueKind.Array)
                        result.ExtractedSkills = skillsEl.EnumerateArray()
                            .Select(s => s.GetString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();

                    if (root.TryGetProperty("CVIssues", out var issuesEl) && issuesEl.ValueKind == JsonValueKind.Array)
                        result.CVIssues = issuesEl.EnumerateArray()
                            .Select(s => s.GetString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();

                    if (root.TryGetProperty("SuggestedJobTitles", out var jobsEl) && jobsEl.ValueKind == JsonValueKind.Array)
                        result.SuggestedJobTitles = jobsEl.EnumerateArray()
                            .Select(s => s.GetString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();

                    if (root.TryGetProperty("RecommendedSkills", out var recEl) && recEl.ValueKind == JsonValueKind.Array)
                        result.RecommendedSkills = recEl.EnumerateArray()
                            .Select(s => s.GetString() ?? "").Where(s => !string.IsNullOrEmpty(s)).ToList();

                    return result;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to parse Gemini AI response for CV extraction");
                    emptyResult.ParsedContent = "AI Format Error";
                    emptyResult.CVIssues.Add($"Parsing failed: {ex.Message}");
                    return emptyResult;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini API request failed during CV extraction");
                emptyResult.ParsedContent = "AI Request Failed";
                emptyResult.CVIssues.Add($"Request failed: {ex.Message}");
                return emptyResult;
            }
        }

        private class CvAiResult
        {
            public string ParsedContent { get; set; } = "AI Parsing Failed";
            public List<string> ExtractedSkills { get; set; } = new();
            public List<string> CVIssues { get; set; } = new();
            public int CVScore { get; set; } = 0;
            public List<string> SuggestedJobTitles { get; set; } = new();
            public List<string> RecommendedSkills { get; set; } = new();
        }

        /// <summary>Extracts plain text from a .docx file using OpenXml SDK</summary>
        private string ExtractTextFromDocx(string filePath)
        {
            try
            {
                using var doc = WordprocessingDocument.Open(filePath, false);
                var body = doc.MainDocumentPart?.Document?.Body;
                if (body == null) return string.Empty;

                var sb = new StringBuilder();
                foreach (var paragraph in body.Elements<Paragraph>())
                {
                    sb.AppendLine(paragraph.InnerText);
                }
                return sb.ToString().Trim();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to extract text from .docx file: {FilePath}", filePath);
                return string.Empty;
            }
        }
        public async Task<ServiceResult<List<CvRS>>> GetUserCvsAsync(string userId)
        {
            try
            {
                var cvs = await _cvRepository.FindAsync(c => c.UserId == userId);
                var result = _mapper.Map<List<CvRS>>(cvs);
                return ServiceResult<List<CvRS>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving CVs for user {UserId}", userId);
                return ServiceResult<List<CvRS>>.Failure("An error occurred while retrieving your CVs.");
            }
        }
        public async Task<ServiceResult<string>> SetPrimaryCvAsync(string userId, int cvId)
        {
            try
            {
                var cvs = await _cvRepository.FindAsync(c => c.UserId == userId);
                var targetCv = cvs.FirstOrDefault(c => c.CVId == cvId);

                if (targetCv == null)
                    return ServiceResult<string>.Failure("CV not found.");

                foreach (var cv in cvs)
                    cv.IsPrimary = (cv.CVId == cvId);

                await _cvRepository.SaveChangesAsync();
                return ServiceResult<string>.Success("Primary CV updated.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting primary CV {CvId} for user {UserId}", cvId, userId);
                return ServiceResult<string>.Failure($"Error setting primary CV: {ex.Message}");
            }
        }
        public async Task<ServiceResult<string>> DeleteCvAsync(string userId, int cvId)
        {
            try
            {
                var cv = await _cvRepository.FirstOrDefaultAsync(c => c.CVId == cvId && c.UserId == userId);
                if (cv == null)
                    return ServiceResult<string>.Failure("CV not found.");

                var fileName = Path.GetFileName(cv.FileUrl);
                var filePath = Path.Combine(_env.WebRootPath, "Uploads", "CVs", fileName);
                if (File.Exists(filePath)) File.Delete(filePath);

                _cvRepository.Remove(cv);
                await _cvRepository.SaveChangesAsync();

                return ServiceResult<string>.Success("CV Deleted Successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting CV {CvId} for user {UserId}", cvId, userId);
                return ServiceResult<string>.Failure($"Error deleting CV: {ex.Message}");
            }
        }
        public async Task<ServiceResult<CvComparisonRS>> CompareCvsAsync(
            string userId,
            CvComparisonRQ request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                if (request.CvIds.Distinct().Count() != request.CvIds.Count)
                    return ServiceResult<CvComparisonRS>.Failure(
                        "Duplicate CV IDs found. Please provide distinct CV IDs.");

                var userCvs = await _cvRepository.FindAsync(c => c.UserId == userId);
                var selectedCvs = userCvs
                    .Where(c => request.CvIds.Contains(c.CVId))
                    .ToList();

                if (selectedCvs.Count != request.CvIds.Count)
                    return ServiceResult<CvComparisonRS>.Failure(
                        "One or more CVs not found or do not belong to you.");

                var unparsed = selectedCvs
                    .Where(c => string.IsNullOrWhiteSpace(c.ParsedContent)
                             || c.ParsedContent == "AI Parsing Failed or not a PDF")
                    .ToList();

                if (unparsed.Any())
                    return ServiceResult<CvComparisonRS>.Failure(
                        "The following CVs could not be parsed: " +
                        string.Join(", ", unparsed.Select(c => c.FileName)));

                var (aiResult, errorMessage) = await CompareWithGeminiAsync(selectedCvs, cancellationToken);

                if (aiResult == null)
                    return ServiceResult<CvComparisonRS>.Failure(
                        errorMessage ?? "AI comparison failed. Please try again.");

                var allSkillSets = selectedCvs
                    .Select(c => new
                    {
                        c.CVId,
                        c.FileName,
                        Skills = c.ExtractedSkills ?? new List<string>()
                    })
                    .ToList();

                List<string> commonSkills;
                if (allSkillSets.All(x => x.Skills.Any()))
                {
                    commonSkills = allSkillSets
                        .Select(x => x.Skills.Select(s => s.ToLower()))
                        .Aggregate((a, b) => a.Intersect(b))
                        .ToList();
                }
                else
                {
                    commonSkills = new List<string>();
                }

                var uniqueSkills = allSkillSets
                    .Select(x => new CvUniqueSkills
                    {
                        CVId = x.CVId,
                        FileName = x.FileName,
                        Skills = x.Skills
                            .Where(s => !commonSkills.Contains(s.ToLower()))
                            .ToList()
                    })
                    .ToList();

                aiResult.CommonSkills = commonSkills;
                aiResult.UniqueSkills = uniqueSkills;

                return ServiceResult<CvComparisonRS>.Success(aiResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error comparing CVs for user {UserId}", userId);
                return ServiceResult<CvComparisonRS>.Failure($"Unexpected error: {ex.Message}");
            }
        }
        private async Task<(CvComparisonRS? Result, string? Error)> CompareWithGeminiAsync(
            List<CV> cvs,
            CancellationToken ct)
        {
            try
            {
                var apiKey = _config["Gemini:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                    return (null, "Gemini API key is missing from configuration.");

                var cvBlocks = string.Join("\n\n", cvs.Select((cv, i) =>
                    $"--- CV {i + 1} (ID: {cv.CVId}, File: {cv.FileName}) ---\n" +
                    $"Parsed Summary: {cv.ParsedContent}\n" +
                    $"Skills: {string.Join(", ", cv.ExtractedSkills ?? new List<string>())}\n" +
                    $"Known Issues: {string.Join(", ", cv.CVIssues ?? new List<string>())}"));

                var prompt = $@"
You are an expert career advisor. Compare the following {cvs.Count} CVs.
Return ONLY a valid JSON object — no markdown, no code fences, no explanation.

The JSON must have exactly these keys:
""CVs"": array of objects, one per CV in the input, each with:
  - ""CVId"": integer (copy exactly from the CV block header)
  - ""FileName"": string (copy exactly from the CV block header)
  - ""ScoreOutOf10"": integer 1-10
  - ""Summary"": string, 2 sentences
  - ""Strengths"": array of strings, minimum 2 items
  - ""Weaknesses"": array of strings, minimum 2 items
  - ""Skills"": array of strings

""ComparisonSummary"": string paragraph comparing all CVs.
""RecommendedCvFileName"": string, the FileName of the best CV.
""RecommendationReason"": string, 2-3 sentences why.

{cvBlocks}";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                    generationConfig = new
                    {
                        temperature = 0.2,
                        topP = 0.8,
                        maxOutputTokens = 3000,
                        candidateCount = 1,
                        responseMimeType = "application/json"
                    }
                };

                var response = await PostToGeminiAsync(requestBody, ct);
                var raw = await response.Content.ReadAsStringAsync(ct);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini error {Status}: {Body}", response.StatusCode, raw);
                    return (null, $"Gemini API error ({response.StatusCode}): {raw}");
                }

                using var doc = JsonDocument.Parse(raw);
                var aiText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrWhiteSpace(aiText))
                    return (null, "Gemini returned an empty response.");

                aiText = aiText
                    .Replace("```json", "")
                    .Replace("```JSON", "")
                    .Replace("```", "")
                    .Trim();

                try
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<CvComparisonRS>(aiText, options);

                    if (result == null)
                        return (null, "Gemini response deserialized to null.");

                    if (result.CVs == null || result.CVs.Count == 0)
                        return (null, $"Gemini returned no CV items. Raw AI text: {aiText}");

                    return (result, null);
                }
                catch (JsonException ex)
                {
                    _logger.LogError(ex, "Failed to parse Gemini JSON. Raw text: {Text}", aiText);
                    return (null, $"Failed to parse Gemini response as JSON. Details: {ex.Message}. Raw: {aiText[..Math.Min(300, aiText.Length)]}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Gemini comparison request failed");
                return (null, $"Unexpected error calling Gemini: {ex.Message}");
            }
        }
    }
}