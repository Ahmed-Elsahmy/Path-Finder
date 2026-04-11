using BLL.Common;
using BLL.Dtos.AiDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace BLL.Services.ChatbotService
{
    public class ChatbotService : IChatbotService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _config;
        private readonly ILogger<ChatbotService> _logger;
        private readonly IRepository<UserSkill> _userSkillRepository;
        private readonly IRepository<UserEducation> _educationRepository;

        private const long MaxFileSizeBytes = 10 * 1024 * 1024;
        private const string GeminiBaseUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        private const string SystemPrompt = @"
You are 'Path Finder AI' — an elite, bilingual career advisor and project consultant
built exclusively for the Path Finder platform.

═══════════════════════════════════════════
LANGUAGE RULES
═══════════════════════════════════════════
- Detect the user's language automatically from their message.
- If the user writes in Arabic → respond ENTIRELY in Arabic.
- If the user writes in English → respond ENTIRELY in English.
- If the user mixes both → respond in the language that dominates the message.
- NEVER mix languages in the same response unless the user does first.

═══════════════════════════════════════════
IDENTITY RULES
═══════════════════════════════════════════
- Your name is 'Path Finder AI'. Never say otherwise.
- NEVER mention Google, Gemini, OpenAI, or any underlying model.
- If asked what AI you are: 'I am Path Finder AI, your career and project advisor.'

═══════════════════════════════════════════
YOUR EXPERTISE — CAREER GUIDANCE
═══════════════════════════════════════════
1. Analyze the user's skills, experience, and goals deeply.
2. Suggest specific job titles, career paths, and industries that fit them.
3. Identify skill gaps and recommend exact courses, certifications, or resources.
4. Give salary expectations based on market context.
5. Help write or improve CVs, cover letters, and LinkedIn summaries.
6. Prepare users for interviews with mock questions and model answers.
7. Advise on career pivots, promotions, freelancing, and remote work strategies.

═══════════════════════════════════════════
YOUR EXPERTISE — PROJECT IDEAS
═══════════════════════════════════════════
When a user asks for project ideas, ALWAYS give structured output:

For EACH idea provide:
- Project Name
- Problem it solves (real-world pain point)
- Target users
- Core features (minimum 4)
- Suggested tech stack (frontend, backend, database, extras)
- Monetization or impact potential
- Difficulty level: Beginner / Intermediate / Advanced

Ideas must be:
- Relevant to the user's field and skill level
- Practical and buildable within 1-3 months
- Useful for a portfolio or real business

═══════════════════════════════════════════
YOUR EXPERTISE — FILE & CV ANALYSIS
═══════════════════════════════════════════
- When a CV/resume is attached: analyze deeply, score out of 10,
  list strengths and weaknesses, give specific improvements.
- When code is attached: review it, find bugs, suggest improvements.
- When an image is attached: describe and analyze in context of the question.
- When audio is attached: transcribe and respond to its contents.

═══════════════════════════════════════════
RESPONSE STYLE
═══════════════════════════════════════════
- Be warm, encouraging, and professional — like a trusted mentor.
- Use clear formatting: headers, bullet points, numbered lists when helpful.
- Be concise but complete — never give vague or generic answers.
- Always end with a relevant follow-up question to continue the conversation.
- Never say 'I cannot help with that' — always find a way to add value.
";

        public ChatbotService(
            IHttpClientFactory httpClientFactory,
            IConfiguration config,
            ILogger<ChatbotService> logger,
            IRepository<UserSkill> userSkillRepository,
            IRepository<UserEducation> educationRepository)
        {
            _httpClientFactory = httpClientFactory;
            _config = config;
            _logger = logger;
            _userSkillRepository = userSkillRepository;
            _educationRepository = educationRepository;
        }

        public async Task<ServiceResult<string>> AskQuestionAsync(
            ChatRQ request,
            string userId,
            string username,
            string? email = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var apiKey = _config["Gemini:ApiKey"];
                if (string.IsNullOrWhiteSpace(apiKey))
                    return ServiceResult<string>.Failure("Gemini API key is not configured.");

                if (string.IsNullOrWhiteSpace(request.Message) && request.Attachment == null)
                    return ServiceResult<string>.Failure(
                        "You must provide either a message or a file/voice note.");

                // Feature 4: Inject user profile (skills + education) into system prompt
                var userContext = await BuildUserContextAsync(userId, username, email);
                var personalizedPrompt = SystemPrompt + userContext;

                var historyTask = BuildHistoryAsync(request, cancellationToken);
                var attachTask = BuildAttachmentPartAsync(request, cancellationToken);
                await Task.WhenAll(historyTask, attachTask);

                var contentsList = new List<object>();
                var historyItems = historyTask.Result;
                var attachmentPart = attachTask.Result;

                if (historyItems != null)
                    contentsList.AddRange(historyItems);

                var userText = request.Message ?? string.Empty;
                if (request.Attachment != null && string.IsNullOrWhiteSpace(userText))
                    userText = "[System: The user has sent a file. Please respond to its contents.]";

                var currentParts = new List<object> { new { text = userText } };
                if (attachmentPart != null)
                    currentParts.Add(attachmentPart);

                contentsList.Add(new { role = "user", parts = currentParts });

                return await CallGeminiAsync(personalizedPrompt, contentsList, 0.7, cancellationToken);
            }
            catch (TaskCanceledException)
            {
                return ServiceResult<string>.Failure("The request timed out. Please try again.",
                    ServiceErrorCode.UpstreamServiceError);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AskQuestionAsync");
                return ServiceResult<string>.Failure("An unexpected error occurred. Please try again.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<string>> GenerateCareerRoadmapAsync(
            string userId,
            CareerRoadmapRQ request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userSkills = await GetUserSkillNamesAsync(userId);

                var skillsContext = userSkills.Any()
                    ? $"Current Skills: {string.Join(", ", userSkills)}"
                    : "Current Skills: None — the user is starting from scratch.";

                var prompt = $@"
You are a career roadmap expert. Based on the user's CURRENT skills and their TARGET job title,
create a detailed, step-by-step learning roadmap.

═══════════════ USER PROFILE ═══════════════
{skillsContext}
Target Job: {request.TargetJobTitle}

═══════════════ INSTRUCTIONS ═══════════════
1. Analyze the gap between current skills and what's needed for the target job.
2. Create a month-by-month roadmap (3-6 months).
3. For each month, provide:
   - Skill/Topic to learn
   - WHY it's needed for the target role
   - Specific FREE resources (course name + platform, e.g., 'CS50 — Harvard/edX')
   - A mini-project to practice the skill
4. At the end, provide:
   - Estimated total time to be job-ready
   - Portfolio projects they should have completed
   - Tips for standing out in applications

═══════════════ FORMAT ═══════════════
Use clear headers, bullet points, and numbered steps.
Respond in the same language the target job title is written in.
Be specific — no generic advice like 'learn programming basics'.
";

                var contents = new List<object>
                {
                    new { role = "user", parts = new[] { new { text = $"Generate a career roadmap for: {request.TargetJobTitle}" } } }
                };

                return await CallGeminiAsync(prompt, contents, 0.5, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating career roadmap for user {UserId}", userId);
                return ServiceResult<string>.Failure("Failed to generate career roadmap. Please try again.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }


        public async Task<ServiceResult<string>> GenerateInterviewPrepAsync(
            string userId,
            InterviewPrepRQ request,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var userSkills = await GetUserSkillNamesAsync(userId);
                var skillsContext = userSkills.Any()
                    ? $"The candidate's skills: {string.Join(", ", userSkills)}"
                    : "No specific skills listed.";

                var prompt = $@"
You are a senior technical interviewer and career coach.
Generate a complete mock interview preparation session.

═══════════════ CONTEXT ═══════════════
Job Title: {request.JobTitle}
Difficulty Level: {request.Difficulty ?? "Intermediate"}
{skillsContext}

═══════════════ INSTRUCTIONS ═══════════════
Generate exactly:
1. **5 Technical Questions** — specific to the job title and skills.
   For each question provide:
   - The question
   - What the interviewer is really testing
   - A model answer (concise but complete)
   - Common mistakes to avoid

2. **3 Behavioral Questions** (STAR format)
   For each provide:
   - The question
   - What quality it tests
   - A model STAR answer template

3. **2 System Design / Problem-Solving Questions** (if applicable to the role)
   For each provide:
   - The question
   - Key points the interviewer expects
   - Approach outline

4. **Interview Tips** — 5 specific tips for this exact role.

═══════════════ FORMAT ═══════════════
Use clear headers and numbered lists.
Match the difficulty level specified.
Respond in the same language the job title is written in.
";

                var contents = new List<object>
                {
                    new { role = "user", parts = new[] { new { text = $"Prepare me for a {request.JobTitle} interview ({request.Difficulty} level)" } } }
                };

                return await CallGeminiAsync(prompt, contents, 0.6, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating interview prep for user {UserId}", userId);
                return ServiceResult<string>.Failure("Failed to generate interview prep. Please try again.",
                    ServiceErrorCode.UpstreamServiceError);
            }
        }
        private async Task<ServiceResult<string>> CallGeminiAsync(
            string systemPrompt,
            List<object> contentsList,
            double temperature,
            CancellationToken cancellationToken)
        {
            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
                return ServiceResult<string>.Failure("Gemini API key is not configured.");

            var body = new
            {
                systemInstruction = new
                {
                    parts = new[] { new { text = systemPrompt } }
                },
                contents = contentsList,
                generationConfig = new
                {
                    temperature,
                    topP = 0.9,
                    topK = 40,
                    maxOutputTokens = 4096,
                    candidateCount = 1
                }
            };

            var client = _httpClientFactory.CreateClient("GeminiClient");
            client.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", apiKey);

            var httpContent = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json");

            var response = await client.PostAsync(GeminiBaseUrl, httpContent, cancellationToken);
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini error {Status}: {Body}", response.StatusCode, responseString);
                return ServiceResult<string>.Failure(
                    $"AI service error ({response.StatusCode}). Please try again.",
                    ServiceErrorCode.UpstreamServiceError);
            }

            using var doc = JsonDocument.Parse(responseString);
            var aiText = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            return ServiceResult<string>.Success(
                aiText?.Trim() ?? "I'm sorry, I could not generate a response.");
        }

        /// <summary>Feature 4: Builds personalized context from user's skills and education</summary>
        private async Task<string> BuildUserContextAsync(string userId, string username, string? email)
        {
            var skills = await GetUserSkillNamesAsync(userId);
            var educations = await _educationRepository.FindAsync(e => e.UserId == userId);

            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendLine("═══════════════════════════════════════════");
            sb.AppendLine("CURRENT USER PROFILE");
            sb.AppendLine("═══════════════════════════════════════════");
            sb.AppendLine($"- Username : {username}");
            sb.AppendLine($"- Email    : {(string.IsNullOrWhiteSpace(email) ? "not provided" : email)}");

            if (skills.Any())
            {
                sb.AppendLine($"- Skills   : {string.Join(", ", skills)}");
                sb.AppendLine("  (Use this knowledge to give personalized, contextual advice.)");
            }
            else
            {
                sb.AppendLine("- Skills   : Not added yet. Encourage user to add skills or upload a CV.");
            }

            if (educations.Any())
            {
                sb.AppendLine("- Education:");
                foreach (var edu in educations)
                {
                    var degree = edu.Degree ?? "N/A";
                    var field = edu.FieldOfStudy ?? "";
                    sb.AppendLine($"    • {degree} {field} at {edu.Institution}");
                }
            }

            sb.AppendLine();
            sb.AppendLine("IMPORTANT:");
            sb.AppendLine($"- You already know the user's name is '{username}'.");
            sb.AppendLine("- Address them by name naturally (not every message).");
            sb.AppendLine("- NEVER ask 'what is your name?' — you already know it.");
            sb.AppendLine($"- If the user asks 'what is my name?' → reply with '{username}'.");
            sb.AppendLine("- Use their skills and education to personalize your advice.");
            sb.AppendLine("- If they ask 'what are my skills?', list the skills above.");

            return sb.ToString();
        }

        /// <summary>Gets the user's skill names from DB</summary>
        private async Task<List<string>> GetUserSkillNamesAsync(string userId)
        {
            var userSkills = await _userSkillRepository.Query()
                .Where(us => us.UserId == userId)
                .Select(us => us.Skill.SkillName)
                .Distinct()
                .ToListAsync();

            return userSkills;
        }

        private async Task<List<object>?> BuildHistoryAsync(ChatRQ request, CancellationToken ct)
        {
            if (request.HistoryFile == null || request.HistoryFile.Length == 0)
                return null;

            try
            {
                using var reader = new StreamReader(request.HistoryFile.OpenReadStream());
                var json = await reader.ReadToEndAsync(ct);

                if (string.IsNullOrWhiteSpace(json)
                    || json.Trim() == "null"
                    || json.Trim() == "[]")
                    return null;

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var history = JsonSerializer.Deserialize<List<ChatMessageHistory>>(json, options);

                return history?
                    .Select(msg => (object)new
                    {
                        role = msg.Role.ToLower() == "model" ? "model" : "user",
                        parts = new[] { new { text = msg.Text } }
                    })
                    .ToList();
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to parse HistoryFile. Proceeding without history.");
                return null;
            }
        }

        private async Task<object?> BuildAttachmentPartAsync(ChatRQ request, CancellationToken ct)
        {
            if (request.Attachment == null) return null;

            if (request.Attachment.Length > MaxFileSizeBytes)
                return null;

            using var ms = new MemoryStream();
            await request.Attachment.CopyToAsync(ms, ct);
            var base64 = Convert.ToBase64String(ms.ToArray());
            var mimeType = ResolveMimeType(
                request.Attachment.ContentType,
                request.Attachment.FileName);

            return new { inlineData = new { mimeType, data = base64 } };
        }

        private static string ResolveMimeType(string contentType, string fileName)
        {
            if (!string.IsNullOrWhiteSpace(contentType)
                && contentType != "application/octet-stream")
                return contentType;

            return Path.GetExtension(fileName ?? "").ToLower() switch
            {
                ".mp3" => "audio/mp3",
                ".wav" => "audio/wav",
                ".m4a" => "audio/mp4",
                ".ogg" => "audio/ogg",
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".cs" => "text/plain",
                ".json" => "application/json",
                _ => "application/octet-stream"
            };
        }
    }
}