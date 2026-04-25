using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using BLL.Common;
using BLL.Dtos.QuestionnaireDtos;
using DAL.Helper.Enums;
using DAL.Models;
using DAL.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BLL.Services.QuestionnaireServices
{
    public class QuestionnaireService : IQuestionnaireService
    {
        private readonly IRepository<Questionnaire> _questionnaireRepository;
        private readonly IRepository<QuestionnaireResponse> _responseRepository;
        private readonly IRepository<CareerPath> _careerPathRepository;
        private readonly IRepository<UserCareerPath> _userCareerPathRepository;
        private readonly IRepository<UserSkill> _userSkillRepository;
        private readonly IRepository<UserEducation> _userEducationRepository;
        private readonly IRepository<UserExperience> _userExperienceRepository;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<QuestionnaireService> _logger;

        private const string CareerAssessmentType = "CareerDiscovery";
        private const string GeminiBaseUrl =
            "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly Dictionary<string, ThemeDefinition> ThemeDefinitions =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["builder"] = new(
                    "Hands-on builder",
                    new[]
                    {
                        "build", "builder", "software", "application", "applications", "app", "apps",
                        "programming", "code", "prototype", "product", "web", "mobile", "architecture"
                    }),
                ["analyst"] = new(
                    "Analytical thinker",
                    new[]
                    {
                        "analyze", "analysis", "data", "dataset", "datasets", "sql", "statistics",
                        "dashboard", "dashboards", "evidence", "insights", "patterns", "report"
                    }),
                ["designer"] = new(
                    "User-centered creator",
                    new[]
                    {
                        "design", "designer", "user", "users", "ux", "ui", "interface", "interfaces",
                        "journey", "visual", "research", "creative", "experience", "delightful"
                    }),
                ["leader"] = new(
                    "Organized coordinator",
                    new[]
                    {
                        "lead", "leadership", "project", "projects", "coordinate", "team", "teams",
                        "plan", "planning", "priorities", "delivery", "retrospectives", "momentum"
                    }),
                ["security"] = new(
                    "Systems and security focus",
                    new[]
                    {
                        "security", "secure", "systems", "system", "network", "networking", "cloud",
                        "logs", "threat", "threats", "reliable", "reliability", "risk", "risks"
                    }),
                ["business"] = new(
                    "Business problem solver",
                    new[]
                    {
                        "business", "stakeholder", "stakeholders", "requirements", "process",
                        "processes", "value", "operations", "market", "markets", "organization",
                        "organizations", "cross-functional"
                    })
            };

        public QuestionnaireService(
            IRepository<Questionnaire> questionnaireRepository,
            IRepository<QuestionnaireResponse> responseRepository,
            IRepository<CareerPath> careerPathRepository,
            IRepository<UserCareerPath> userCareerPathRepository,
            IRepository<UserSkill> userSkillRepository,
            IRepository<UserEducation> userEducationRepository,
            IRepository<UserExperience> userExperienceRepository,
            IConfiguration config,
            IHttpClientFactory httpClientFactory,
            ILogger<QuestionnaireService> logger)
        {
            _questionnaireRepository = questionnaireRepository;
            _responseRepository = responseRepository;
            _careerPathRepository = careerPathRepository;
            _userCareerPathRepository = userCareerPathRepository;
            _userSkillRepository = userSkillRepository;
            _userEducationRepository = userEducationRepository;
            _userExperienceRepository = userExperienceRepository;
            _config = config;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<ServiceResult<CareerAssessmentQuestionnaireRS>> GetCareerAssessmentAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<CareerAssessmentQuestionnaireRS>.Failure(
                    "Invalid user ID.",
                    ServiceErrorCode.Unauthorized);
            }

            var questionnaire = await _questionnaireRepository.Query()
                .AsNoTracking()
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(
                    q => q.IsActive && q.QuestionnaireType == CareerAssessmentType,
                    cancellationToken);

            if (questionnaire == null)
            {
                return ServiceResult<CareerAssessmentQuestionnaireRS>.Failure(
                    "No active career assessment is configured yet.",
                    ServiceErrorCode.NotFound);
            }

            var savedResponses = await _responseRepository.Query()
                .AsNoTracking()
                .Where(r => r.UserId == userId && r.Question.QuestionnaireId == questionnaire.QuestionnaireId)
                .ToListAsync(cancellationToken);

            var responseLookup = savedResponses
                .GroupBy(r => r.QuestionId)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(x => x.AnsweredAt).First());

            var orderedQuestions = questionnaire.Questions
                .OrderBy(q => q.OrderNumber)
                .Select(q =>
                {
                    responseLookup.TryGetValue(q.QuestionId, out var savedResponse);

                    return new CareerAssessmentQuestionRS
                    {
                        QuestionId = q.QuestionId,
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        Options = q.Options ?? new List<string>(),
                        OrderNumber = q.OrderNumber,
                        IsRequired = q.IsRequired,
                        SavedAnswer = savedResponse?.Answer,
                        SavedInsight = savedResponse?.AIAnalysis
                    };
                })
                .ToList();

            var answeredQuestions = orderedQuestions.Count(q => !string.IsNullOrWhiteSpace(q.SavedAnswer));
            var totalQuestions = orderedQuestions.Count;

            var response = new CareerAssessmentQuestionnaireRS
            {
                QuestionnaireId = questionnaire.QuestionnaireId,
                Title = questionnaire.Title,
                Description = questionnaire.Description,
                QuestionnaireType = questionnaire.QuestionnaireType ?? string.Empty,
                TotalQuestions = totalQuestions,
                AnsweredQuestions = answeredQuestions,
                CompletionPercentage = totalQuestions == 0
                    ? 0
                    : (int)Math.Round(answeredQuestions * 100d / totalQuestions),
                LastUpdatedAt = savedResponses
                    .OrderByDescending(r => r.AnsweredAt)
                    .Select(r => (DateTime?)r.AnsweredAt)
                    .FirstOrDefault(),
                Questions = orderedQuestions
            };

            return ServiceResult<CareerAssessmentQuestionnaireRS>.Success(response);
        }

        public async Task<ServiceResult<CareerAssessmentResultRS>> SubmitCareerAssessmentAsync(
            string userId,
            SubmitCareerAssessmentRQ request,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                return ServiceResult<CareerAssessmentResultRS>.Failure(
                    "Invalid user ID.",
                    ServiceErrorCode.Unauthorized);
            }

            if (request == null || request.QuestionnaireId <= 0 || request.Answers == null || request.Answers.Count == 0)
            {
                return ServiceResult<CareerAssessmentResultRS>.Failure(
                    "Please answer the questionnaire before submitting.",
                    ServiceErrorCode.ValidationError);
            }

            var questionnaire = await _questionnaireRepository.Query()
                .Include(q => q.Questions)
                .FirstOrDefaultAsync(
                    q => q.QuestionnaireId == request.QuestionnaireId
                      && q.IsActive
                      && q.QuestionnaireType == CareerAssessmentType,
                    cancellationToken);

            if (questionnaire == null)
            {
                return ServiceResult<CareerAssessmentResultRS>.Failure(
                    "The requested questionnaire could not be found.",
                    ServiceErrorCode.NotFound);
            }

            var orderedQuestions = questionnaire.Questions
                .OrderBy(q => q.OrderNumber)
                .ToList();

            var questionLookup = orderedQuestions.ToDictionary(q => q.QuestionId);
            var normalizedAnswers = request.Answers
                .GroupBy(a => a.QuestionId)
                .ToDictionary(g => g.Key, g => NormalizeAnswer(g.Last().Answer));

            var invalidQuestionIds = normalizedAnswers.Keys
                .Where(questionId => !questionLookup.ContainsKey(questionId))
                .ToList();

            if (invalidQuestionIds.Count > 0)
            {
                return ServiceResult<CareerAssessmentResultRS>.Failure(
                    "Some submitted answers do not belong to this questionnaire.",
                    ServiceErrorCode.ValidationError);
            }

            foreach (var question in orderedQuestions)
            {
                if (!normalizedAnswers.TryGetValue(question.QuestionId, out var answer) || string.IsNullOrWhiteSpace(answer))
                {
                    continue;
                }

                if (IsChoiceQuestion(question))
                {
                    var matchedOption = (question.Options ?? new List<string>())
                        .FirstOrDefault(option => string.Equals(
                            option.Trim(),
                            answer,
                            StringComparison.OrdinalIgnoreCase));

                    if (matchedOption == null)
                    {
                        return ServiceResult<CareerAssessmentResultRS>.Failure(
                            $"Answer for question {question.OrderNumber} is not one of the allowed options.",
                            ServiceErrorCode.ValidationError);
                    }

                    normalizedAnswers[question.QuestionId] = matchedOption;
                }
            }

            var missingRequiredQuestions = orderedQuestions
                .Where(q =>
                    q.IsRequired &&
                    (!normalizedAnswers.TryGetValue(q.QuestionId, out var answer)
                     || string.IsNullOrWhiteSpace(answer)))
                .Select(q => q.OrderNumber)
                .ToList();

            if (missingRequiredQuestions.Count > 0)
            {
                return ServiceResult<CareerAssessmentResultRS>.Failure(
                    $"Please answer all required questions before submitting. Missing question numbers: {string.Join(", ", missingRequiredQuestions)}.",
                    ServiceErrorCode.ValidationError);
            }

            var questionIds = orderedQuestions.Select(q => q.QuestionId).ToList();
            var existingResponses = await _responseRepository.Query()
                .Where(r => r.UserId == userId && questionIds.Contains(r.QuestionId))
                .ToListAsync(cancellationToken);

            var responseLookup = existingResponses.ToDictionary(r => r.QuestionId);

            foreach (var question in orderedQuestions)
            {
                var hasAnswer = normalizedAnswers.TryGetValue(question.QuestionId, out var answer);

                if (!hasAnswer)
                {
                    if (!responseLookup.TryGetValue(question.QuestionId, out var existingOptionalResponse))
                    {
                        continue;
                    }

                    existingOptionalResponse.Answer = null;
                    existingOptionalResponse.AIAnalysis = null;
                    existingOptionalResponse.AnsweredAt = DateTime.UtcNow;
                    continue;
                }

                if (!responseLookup.TryGetValue(question.QuestionId, out var response))
                {
                    response = new QuestionnaireResponse
                    {
                        UserId = userId,
                        QuestionId = question.QuestionId
                    };

                    await _responseRepository.AddAsync(response);
                    responseLookup[question.QuestionId] = response;
                    existingResponses.Add(response);
                }

                response.Answer = answer;
                response.AIAnalysis = null;
                response.AnsweredAt = DateTime.UtcNow;
            }

            await _responseRepository.SaveChangesAsync();

            var result = await EvaluateCareerAssessmentAsync(
                userId,
                questionnaire,
                orderedQuestions,
                existingResponses,
                cancellationToken);

            if (!result.IsSuccess || result.Data == null)
            {
                return result;
            }

            var insightLookup = result.Data.ResponseInsights
                .GroupBy(i => i.QuestionId)
                .ToDictionary(g => g.Key, g => g.First().AIAnalysis);

            foreach (var response in existingResponses)
            {
                if (string.IsNullOrWhiteSpace(response.Answer))
                {
                    response.AIAnalysis = null;
                    continue;
                }

                response.AIAnalysis = insightLookup.TryGetValue(response.QuestionId, out var insight)
                    ? insight
                    : BuildDefaultInsight(questionLookup[response.QuestionId], response.Answer);
            }

            await _responseRepository.SaveChangesAsync();

            return result;
        }

        private async Task<ServiceResult<CareerAssessmentResultRS>> EvaluateCareerAssessmentAsync(
            string userId,
            Questionnaire questionnaire,
            List<Question> orderedQuestions,
            List<QuestionnaireResponse> savedResponses,
            CancellationToken cancellationToken)
        {
            var answeredResponses = savedResponses
                .Where(r => !string.IsNullOrWhiteSpace(r.Answer))
                .GroupBy(r => r.QuestionId)
                .Select(g => g.OrderByDescending(x => x.AnsweredAt).First())
                .ToList();

            if (answeredResponses.Count == 0)
            {
                return ServiceResult<CareerAssessmentResultRS>.Failure(
                    "The questionnaire has no usable answers to analyze.",
                    ServiceErrorCode.ValidationError);
            }

            var userContext = await BuildUserContextAsync(userId, cancellationToken);

            var careerPaths = await _careerPathRepository.Query()
                .AsNoTracking()
                .Include(cp => cp.Category)
                .Include(cp => cp.SubCategory)
                .Include(cp => cp.CareerPathCourses)
                .ToListAsync(cancellationToken);

            if (careerPaths.Count == 0)
            {
                return ServiceResult<CareerAssessmentResultRS>.Failure(
                    "No career paths are available in the database yet.",
                    ServiceErrorCode.NotFound);
            }

            var rankedCandidates = RankCareerPathCandidates(careerPaths, answeredResponses, userContext);
            if (rankedCandidates.Count == 0)
            {
                return ServiceResult<CareerAssessmentResultRS>.Failure(
                    "We could not match your answers to any career path yet.",
                    ServiceErrorCode.NotFound);
            }

            var aiResult = await TryGenerateAiAssessmentAsync(
                questionnaire,
                orderedQuestions,
                answeredResponses,
                userContext,
                rankedCandidates,
                cancellationToken);

            var enrolledIds = await _userCareerPathRepository.Query()
                .AsNoTracking()
                .Where(x => x.UserId == userId && x.Status != CareerPathStatus.Cancelled)
                .Select(x => x.CareerPathId)
                .Distinct()
                .ToListAsync(cancellationToken);

            var enrolledIdSet = enrolledIds.ToHashSet();

            var response = aiResult == null
                ? BuildHeuristicResult(questionnaire, orderedQuestions, answeredResponses, userContext, rankedCandidates, enrolledIdSet)
                : BuildAiResult(questionnaire, orderedQuestions, answeredResponses, userContext, rankedCandidates, enrolledIdSet, aiResult);

            return ServiceResult<CareerAssessmentResultRS>.Success(response);
        }

        private async Task<UserAssessmentContext> BuildUserContextAsync(string userId, CancellationToken cancellationToken)
        {
            var skillRows = await _userSkillRepository.Query()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.Skill)
                .OrderByDescending(x => x.AcquiredDate)
                .Select(x => new
                {
                    SkillName = x.Skill != null ? x.Skill.SkillName : null
                })
                .ToListAsync(cancellationToken);

            var skills = skillRows
                .Select(x => x.SkillName)
                .Where(skillName => !string.IsNullOrWhiteSpace(skillName))
                .Cast<string>()
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(10)
                .ToList();

            var educationRows = await _userEducationRepository.Query()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsCurrent)
                .ThenByDescending(x => x.EndDate ?? x.StartDate)
                .Select(x => new
                {
                    x.Degree,
                    x.FieldOfStudy,
                    x.Institution
                })
                .ToListAsync(cancellationToken);

            var education = educationRows
                .Select(x => string.Join(
                    " ",
                    new[]
                    {
                        x.Degree,
                        string.IsNullOrWhiteSpace(x.FieldOfStudy) ? null : $"in {x.FieldOfStudy}",
                        string.IsNullOrWhiteSpace(x.Institution) ? null : $"at {x.Institution}"
                    }.Where(part => !string.IsNullOrWhiteSpace(part))))
                .Where(summary => !string.IsNullOrWhiteSpace(summary))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();

            var experienceRows = await _userExperienceRepository.Query()
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.IsCurrent)
                .ThenByDescending(x => x.StartDate)
                .Select(x => new
                {
                    x.Position,
                    x.CompanyName
                })
                .ToListAsync(cancellationToken);

            var experiences = experienceRows
                .Select(x => string.Join(
                    " ",
                    new[]
                    {
                        x.Position,
                        string.IsNullOrWhiteSpace(x.CompanyName) ? null : $"at {x.CompanyName}"
                    }.Where(part => !string.IsNullOrWhiteSpace(part))))
                .Where(summary => !string.IsNullOrWhiteSpace(summary))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(3)
                .ToList();

            return new UserAssessmentContext(skills, education, experiences);
        }

        private List<CareerPathCandidate> RankCareerPathCandidates(
            List<CareerPath> careerPaths,
            List<QuestionnaireResponse> answeredResponses,
            UserAssessmentContext userContext)
        {
            var answerTexts = answeredResponses
                .Select(r => r.Answer)
                .Where(answer => !string.IsNullOrWhiteSpace(answer))
                .Cast<string>()
                .ToList();

            var themeScores = ExtractThemeScores(answerTexts);
            var answerTokens = Tokenize(string.Join(" ", answerTexts));
            var skillTokens = Tokenize(string.Join(" ", userContext.Skills));

            var ranked = careerPaths
                .Select(path =>
                {
                    var pathText = BuildCareerPathText(path);
                    var pathTokens = Tokenize(pathText);
                    var signals = new List<string>();

                    var score = answerTokens.Intersect(pathTokens).Count() * 5;
                    score += skillTokens.Intersect(pathTokens).Count() * 4;

                    foreach (var theme in themeScores.OrderByDescending(x => x.Value))
                    {
                        if (!ThemeDefinitions.TryGetValue(theme.Key, out var definition))
                        {
                            continue;
                        }

                        if (!ContainsAnyKeyword(pathText, definition.Keywords))
                        {
                            continue;
                        }

                        score += theme.Value * 8;
                        signals.Add(definition.Label);
                    }

                    if (!string.IsNullOrWhiteSpace(path.Category?.Name))
                    {
                        score += Tokenize(path.Category.Name).Intersect(answerTokens).Count() * 3;
                    }

                    if (!string.IsNullOrWhiteSpace(path.SubCategory?.Name))
                    {
                        score += Tokenize(path.SubCategory.Name).Intersect(answerTokens).Count() * 4;
                    }

                    if (path.TotalCourses > 0)
                    {
                        score += 4;
                    }

                    signals = signals
                        .Distinct(StringComparer.OrdinalIgnoreCase)
                        .Take(3)
                        .ToList();

                    return new CareerPathCandidate
                    {
                        Path = path,
                        HeuristicScore = Math.Max(score, signals.Count * 6 + 2),
                        Signals = signals,
                        GrowthAreas = ExtractGrowthAreas(path)
                    };
                })
                .OrderByDescending(x => x.HeuristicScore)
                .ThenBy(x => x.Path.PathName)
                .Take(12)
                .ToList();

            if (ranked.Count == 0)
            {
                return ranked;
            }

            var maxScore = ranked.Max(x => x.HeuristicScore);
            var minScore = ranked.Min(x => x.HeuristicScore);

            for (var index = 0; index < ranked.Count; index++)
            {
                ranked[index].DisplayScore = CalculateDisplayScore(ranked[index].HeuristicScore, minScore, maxScore, index);
            }

            return ranked;
        }

        private async Task<AssessmentAiResult?> TryGenerateAiAssessmentAsync(
            Questionnaire questionnaire,
            List<Question> orderedQuestions,
            List<QuestionnaireResponse> answeredResponses,
            UserAssessmentContext userContext,
            List<CareerPathCandidate> candidates,
            CancellationToken cancellationToken)
        {
            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }

            try
            {
                var questionLookup = orderedQuestions.ToDictionary(q => q.QuestionId);
                var answersPayload = answeredResponses
                    .OrderBy(r => questionLookup[r.QuestionId].OrderNumber)
                    .Select(r => new
                    {
                        r.QuestionId,
                        QuestionText = questionLookup[r.QuestionId].QuestionText,
                        Answer = r.Answer
                    })
                    .ToList();

                var candidatePayload = candidates
                    .Select(candidate => new
                    {
                        CareerPathId = candidate.Path.CareerPathId,
                        PathName = candidate.Path.PathName,
                        candidate.Path.Description,
                        Category = candidate.Path.Category?.Name,
                        SubCategory = candidate.Path.SubCategory?.Name,
                        DifficultyLevel = candidate.Path.DifficultyLevel?.ToString(),
                        EstimatedDurationMonths = candidate.Path.EstimatedDurationMonths,
                        candidate.Path.Prerequisites,
                        candidate.Path.ExpectedOutcomes,
                        candidate.Path.TotalCourses
                    })
                    .ToList();

                var prompt = $@"
You are Path Finder AI.
Analyze the user's career-assessment answers and rank ONLY the career paths provided below.

Return ONLY valid JSON with this exact schema:
{{
  ""ProfileSummary"": ""string"",
  ""RecommendationStrategy"": ""string"",
  ""TopTraits"": [""string""],
  ""ResponseInsights"": [
    {{
      ""QuestionId"": 0,
      ""AIAnalysis"": ""string""
    }}
  ],
  ""Recommendations"": [
    {{
      ""CareerPathId"": 0,
      ""SuitabilityScore"": 0,
      ""MatchReason"": ""string"",
      ""WhyItFits"": ""string"",
      ""StrengthSignals"": [""string""],
      ""GrowthAreas"": [""string""],
      ""SuggestedNextStep"": ""string""
    }}
  ]
}}

Rules:
- Recommend between 3 and 5 career paths.
- CareerPathId must come from the provided list only.
- Sort recommendations by SuitabilityScore descending.
- SuitabilityScore must be an integer from 0 to 100.
- MatchReason should be short and direct.
- StrengthSignals should contain 2 or 3 short bullet-style strings.
- GrowthAreas should contain 1 to 3 short bullet-style strings.
- SuggestedNextStep should be a single actionable sentence.
- Include one ResponseInsight for every answered question.
- No markdown, no prose outside JSON.

Questionnaire:
{JsonSerializer.Serialize(new
{
    questionnaire.QuestionnaireId,
    questionnaire.Title,
    questionnaire.Description,
    questionnaire.QuestionnaireType
}, new JsonSerializerOptions { WriteIndented = true })}

UserProfile:
{JsonSerializer.Serialize(new
{
    Skills = userContext.Skills,
    Education = userContext.Education,
    Experience = userContext.Experiences
}, new JsonSerializerOptions { WriteIndented = true })}

Answers:
{JsonSerializer.Serialize(answersPayload, new JsonSerializerOptions { WriteIndented = true })}

CareerPaths:
{JsonSerializer.Serialize(candidatePayload, new JsonSerializerOptions { WriteIndented = true })}
";

                var requestBody = new
                {
                    contents = new[] { new { parts = new[] { new { text = prompt } } } },
                    generationConfig = new
                    {
                        temperature = 0.3,
                        responseMimeType = "application/json"
                    }
                };

                var client = _httpClientFactory.CreateClient("GeminiClient");
                client.DefaultRequestHeaders.Remove("x-goog-api-key");
                client.DefaultRequestHeaders.TryAddWithoutValidation("x-goog-api-key", apiKey);

                HttpResponseMessage? response = null;
                for (var attempt = 1; attempt <= 3; attempt++)
                {
                    using var content = new StringContent(
                        JsonSerializer.Serialize(requestBody),
                        Encoding.UTF8,
                        "application/json");

                    response = await client.PostAsync(GeminiBaseUrl, content, cancellationToken);
                    if (response.IsSuccessStatusCode)
                    {
                        break;
                    }

                    if ((int)response.StatusCode == 503 && attempt < 3)
                    {
                        _logger.LogWarning("Gemini returned 503 for career assessment attempt {Attempt}. Retrying.", attempt);
                        await Task.Delay(2500, cancellationToken);
                        continue;
                    }

                    break;
                }

                if (response == null)
                {
                    return null;
                }

                var raw = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Gemini career assessment failed with status {Status}. Body: {Body}", response.StatusCode, raw);
                    return null;
                }

                using var document = JsonDocument.Parse(raw);
                var generatedText = document.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                if (string.IsNullOrWhiteSpace(generatedText))
                {
                    return null;
                }

                var cleanedJson = generatedText
                    .Replace("```json", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Replace("```", string.Empty, StringComparison.OrdinalIgnoreCase)
                    .Trim();

                var parsedResult = JsonSerializer.Deserialize<AssessmentAiResult>(cleanedJson, JsonOptions);
                return SanitizeAiResult(parsedResult, candidates, answeredResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Career assessment AI generation failed.");
                return null;
            }
        }

        private AssessmentAiResult? SanitizeAiResult(
            AssessmentAiResult? aiResult,
            List<CareerPathCandidate> candidates,
            List<QuestionnaireResponse> answeredResponses)
        {
            if (aiResult == null)
            {
                return null;
            }

            var allowedCareerPathIds = candidates
                .Select(c => c.Path.CareerPathId)
                .ToHashSet();

            var answeredQuestionIds = answeredResponses
                .Select(r => r.QuestionId)
                .ToHashSet();

            aiResult.TopTraits = CleanList(aiResult.TopTraits, 4);

            aiResult.ResponseInsights = (aiResult.ResponseInsights ?? new List<CareerAssessmentResponseInsightRS>())
                .Where(i => answeredQuestionIds.Contains(i.QuestionId) && !string.IsNullOrWhiteSpace(i.AIAnalysis))
                .GroupBy(i => i.QuestionId)
                .Select(g => new CareerAssessmentResponseInsightRS
                {
                    QuestionId = g.Key,
                    AIAnalysis = g.First().AIAnalysis.Trim()
                })
                .ToList();

            aiResult.Recommendations = (aiResult.Recommendations ?? new List<AssessmentAiRecommendation>())
                .Where(r => allowedCareerPathIds.Contains(r.CareerPathId))
                .GroupBy(r => r.CareerPathId)
                .Select(g =>
                {
                    var recommendation = g.First();
                    recommendation.SuitabilityScore = Math.Clamp(recommendation.SuitabilityScore, 55, 99);
                    recommendation.MatchReason = recommendation.MatchReason?.Trim() ?? string.Empty;
                    recommendation.WhyItFits = recommendation.WhyItFits?.Trim() ?? string.Empty;
                    recommendation.SuggestedNextStep = recommendation.SuggestedNextStep?.Trim() ?? string.Empty;
                    recommendation.StrengthSignals = CleanList(recommendation.StrengthSignals, 3);
                    recommendation.GrowthAreas = CleanList(recommendation.GrowthAreas, 3);
                    return recommendation;
                })
                .OrderByDescending(r => r.SuitabilityScore)
                .Take(5)
                .ToList();

            if (aiResult.Recommendations.Count == 0)
            {
                return null;
            }

            aiResult.ProfileSummary = aiResult.ProfileSummary?.Trim() ?? string.Empty;
            aiResult.RecommendationStrategy = aiResult.RecommendationStrategy?.Trim() ?? string.Empty;

            return aiResult;
        }

        private CareerAssessmentResultRS BuildAiResult(
            Questionnaire questionnaire,
            List<Question> orderedQuestions,
            List<QuestionnaireResponse> answeredResponses,
            UserAssessmentContext userContext,
            List<CareerPathCandidate> rankedCandidates,
            HashSet<int> enrolledIds,
            AssessmentAiResult aiResult)
        {
            var candidateLookup = rankedCandidates.ToDictionary(c => c.Path.CareerPathId);
            var topTraits = aiResult.TopTraits.Count > 0
                ? aiResult.TopTraits
                : GetTopThemeLabels(ExtractThemeScores(answeredResponses.Select(r => r.Answer!)), 3);

            var recommendations = new List<CareerAssessmentRecommendationRS>();

            foreach (var aiRecommendation in aiResult.Recommendations)
            {
                if (!candidateLookup.TryGetValue(aiRecommendation.CareerPathId, out var candidate))
                {
                    continue;
                }

                recommendations.Add(MapAiRecommendation(candidate, aiRecommendation, enrolledIds));
            }

            foreach (var candidate in rankedCandidates)
            {
                if (recommendations.Count >= 5)
                {
                    break;
                }

                if (recommendations.Any(r => r.CareerPathId == candidate.Path.CareerPathId))
                {
                    continue;
                }

                recommendations.Add(MapHeuristicRecommendation(candidate, enrolledIds));
            }

            return new CareerAssessmentResultRS
            {
                QuestionnaireId = questionnaire.QuestionnaireId,
                QuestionnaireTitle = questionnaire.Title,
                EvaluatedAt = DateTime.UtcNow,
                ProfileSummary = !string.IsNullOrWhiteSpace(aiResult.ProfileSummary)
                    ? aiResult.ProfileSummary
                    : BuildProfileSummary(topTraits, userContext),
                RecommendationStrategy = !string.IsNullOrWhiteSpace(aiResult.RecommendationStrategy)
                    ? aiResult.RecommendationStrategy
                    : BuildDefaultRecommendationStrategy(),
                TopTraits = topTraits,
                ResponseInsights = BuildResponseInsights(orderedQuestions, answeredResponses, aiResult.ResponseInsights),
                Recommendations = recommendations
                    .OrderByDescending(r => r.SuitabilityScore)
                    .ThenBy(r => r.CareerPathName)
                    .Take(5)
                    .ToList()
            };
        }

        private CareerAssessmentResultRS BuildHeuristicResult(
            Questionnaire questionnaire,
            List<Question> orderedQuestions,
            List<QuestionnaireResponse> answeredResponses,
            UserAssessmentContext userContext,
            List<CareerPathCandidate> rankedCandidates,
            HashSet<int> enrolledIds)
        {
            var topTraits = GetTopThemeLabels(ExtractThemeScores(answeredResponses.Select(r => r.Answer!)), 3);

            return new CareerAssessmentResultRS
            {
                QuestionnaireId = questionnaire.QuestionnaireId,
                QuestionnaireTitle = questionnaire.Title,
                EvaluatedAt = DateTime.UtcNow,
                ProfileSummary = BuildProfileSummary(topTraits, userContext),
                RecommendationStrategy = BuildDefaultRecommendationStrategy(),
                TopTraits = topTraits,
                ResponseInsights = BuildResponseInsights(orderedQuestions, answeredResponses),
                Recommendations = rankedCandidates
                    .Take(5)
                    .Select(candidate => MapHeuristicRecommendation(candidate, enrolledIds))
                    .ToList()
            };
        }

        private CareerAssessmentRecommendationRS MapAiRecommendation(
            CareerPathCandidate candidate,
            AssessmentAiRecommendation aiRecommendation,
            HashSet<int> enrolledIds)
        {
            var fallback = MapHeuristicRecommendation(candidate, enrolledIds);

            fallback.SuitabilityScore = aiRecommendation.SuitabilityScore;
            fallback.MatchReason = !string.IsNullOrWhiteSpace(aiRecommendation.MatchReason)
                ? aiRecommendation.MatchReason
                : fallback.MatchReason;
            fallback.WhyItFits = !string.IsNullOrWhiteSpace(aiRecommendation.WhyItFits)
                ? aiRecommendation.WhyItFits
                : fallback.WhyItFits;
            fallback.StrengthSignals = aiRecommendation.StrengthSignals.Count > 0
                ? aiRecommendation.StrengthSignals
                : fallback.StrengthSignals;
            fallback.GrowthAreas = aiRecommendation.GrowthAreas.Count > 0
                ? aiRecommendation.GrowthAreas
                : fallback.GrowthAreas;
            fallback.SuggestedNextStep = !string.IsNullOrWhiteSpace(aiRecommendation.SuggestedNextStep)
                ? aiRecommendation.SuggestedNextStep
                : fallback.SuggestedNextStep;

            return fallback;
        }

        private CareerAssessmentRecommendationRS MapHeuristicRecommendation(
            CareerPathCandidate candidate,
            HashSet<int> enrolledIds)
        {
            var focusArea = candidate.Path.SubCategory?.Name
                ?? candidate.Path.Category?.Name
                ?? candidate.Path.PathName;

            var signalText = candidate.Signals.Count > 0
                ? string.Join(" and ", candidate.Signals.Take(2).Select(signal => signal.ToLowerInvariant()))
                : "problem solving and growth preferences";

            return new CareerAssessmentRecommendationRS
            {
                CareerPathId = candidate.Path.CareerPathId,
                CareerPathName = candidate.Path.PathName,
                Description = candidate.Path.Description,
                CategoryName = candidate.Path.Category?.Name,
                SubCategoryName = candidate.Path.SubCategory?.Name,
                DifficultyLevel = candidate.Path.DifficultyLevel,
                DurationInMonths = candidate.Path.EstimatedDurationMonths,
                TotalCourses = candidate.Path.TotalCourses,
                SuitabilityScore = candidate.DisplayScore,
                MatchReason = candidate.Signals.Count > 0
                    ? $"Strong alignment with your {signalText} profile."
                    : "Strong alignment with the way you prefer to learn and solve problems.",
                WhyItFits = $"Your answers repeatedly pointed toward {signalText}. {candidate.Path.PathName} is a strong option because it centers on {focusArea} and already exists as a structured path inside Path Finder.",
                StrengthSignals = candidate.Signals.Count > 0
                    ? candidate.Signals
                    : new List<string> { "Clear interest alignment", "Strong platform fit" },
                GrowthAreas = candidate.GrowthAreas.Count > 0
                    ? candidate.GrowthAreas
                    : new List<string> { "Validate the day-to-day work with a starter project" },
                SuggestedNextStep = BuildSuggestedNextStep(candidate.Path),
                IsAlreadyEnrolled = enrolledIds.Contains(candidate.Path.CareerPathId)
            };
        }

        private List<CareerAssessmentResponseInsightRS> BuildResponseInsights(
            List<Question> orderedQuestions,
            List<QuestionnaireResponse> answeredResponses,
            List<CareerAssessmentResponseInsightRS>? aiInsights = null)
        {
            var questionLookup = orderedQuestions.ToDictionary(q => q.QuestionId);
            var aiInsightLookup = (aiInsights ?? new List<CareerAssessmentResponseInsightRS>())
                .GroupBy(i => i.QuestionId)
                .ToDictionary(g => g.Key, g => g.First().AIAnalysis);

            return answeredResponses
                .Where(r => !string.IsNullOrWhiteSpace(r.Answer))
                .OrderBy(r => questionLookup[r.QuestionId].OrderNumber)
                .Select(r => new CareerAssessmentResponseInsightRS
                {
                    QuestionId = r.QuestionId,
                    AIAnalysis = aiInsightLookup.TryGetValue(r.QuestionId, out var insight)
                        ? insight
                        : BuildDefaultInsight(questionLookup[r.QuestionId], r.Answer)
                })
                .ToList();
        }

        private static string BuildDefaultRecommendationStrategy()
        {
            return "Recommendations are ordered by how strongly your interests, preferred work style, and current background align with the career paths stored in Path Finder.";
        }

        private static string BuildSuggestedNextStep(CareerPath careerPath)
        {
            var focusArea = careerPath.SubCategory?.Name
                ?? careerPath.Category?.Name
                ?? careerPath.PathName;

            return careerPath.TotalCourses > 0
                ? $"Start with the first course in this path and test your fit with a small {focusArea} project."
                : $"Review the prerequisites and try a small {focusArea} project before committing fully.";
        }

        private static string BuildDefaultInsight(Question question, string? answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                return "No answer was provided for this question.";
            }

            var themes = GetTopThemeLabels(ExtractThemeScores(new[] { answer }), 1);
            if (themes.Count > 0)
            {
                return $"This answer highlights a strong {themes[0].ToLowerInvariant()} preference.";
            }

            return question.QuestionType.Equals("text", StringComparison.OrdinalIgnoreCase)
                ? "This answer adds extra context about the kind of problems that motivate you."
                : "This answer gives another signal about the work style and problems that fit you best.";
        }

        private static string BuildProfileSummary(List<string> topTraits, UserAssessmentContext userContext)
        {
            var leadTraits = topTraits.Count > 0
                ? topTraits
                : new List<string> { "Curious learner" };

            var builder = new StringBuilder();
            builder.Append($"Your answers suggest a {leadTraits[0].ToLowerInvariant()} profile");

            if (leadTraits.Count > 1)
            {
                builder.Append(" with strong ");
                builder.Append(string.Join(" and ", leadTraits.Skip(1).Select(x => x.ToLowerInvariant())));
                builder.Append(" tendencies");
            }

            builder.Append('.');

            if (userContext.Skills.Count > 0)
            {
                builder.Append(" You already bring experience in ");
                builder.Append(string.Join(", ", userContext.Skills.Take(3)));
                builder.Append('.');
            }

            return builder.ToString();
        }

        private static List<string> GetTopThemeLabels(Dictionary<string, int> themeScores, int maxCount)
        {
            var labels = themeScores
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .Take(maxCount)
                .Select(x => ThemeDefinitions.TryGetValue(x.Key, out var definition)
                    ? definition.Label
                    : "Curious learner")
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (labels.Count == 0)
            {
                labels.Add("Curious learner");
            }

            return labels;
        }

        private static Dictionary<string, int> ExtractThemeScores(IEnumerable<string> answers)
        {
            var scores = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var answer in answers.Where(a => !string.IsNullOrWhiteSpace(a)))
            {
                var answerText = answer.Trim();
                var answerTokens = Tokenize(answerText);

                foreach (var entry in ThemeDefinitions)
                {
                    var matches = entry.Value.Keywords.Count(keyword =>
                        KeywordMatches(answerText, answerTokens, keyword));

                    if (matches == 0)
                    {
                        continue;
                    }

                    scores[entry.Key] = scores.TryGetValue(entry.Key, out var existingScore)
                        ? existingScore + matches
                        : matches;
                }
            }

            return scores;
        }

        private static List<string> ExtractGrowthAreas(CareerPath careerPath)
        {
            var items = SplitTextIntoBullets(careerPath.Prerequisites, 2);
            if (items.Count > 0)
            {
                return items;
            }

            if (careerPath.DifficultyLevel == DifficultyLevel.Advanced)
            {
                return new List<string>
                {
                    "Strengthen your fundamentals before advanced topics"
                };
            }

            if (careerPath.DifficultyLevel == DifficultyLevel.Intermediate)
            {
                return new List<string>
                {
                    "Build confidence with a few guided practice projects"
                };
            }

            return new List<string>
            {
                "Validate the fit with a small real-world project"
            };
        }

        private static int CalculateDisplayScore(int score, int minScore, int maxScore, int index)
        {
            if (maxScore == minScore)
            {
                return Math.Clamp(88 - (index * 3), 70, 95);
            }

            var normalized = 72 + (int)Math.Round((score - minScore) * 23d / (maxScore - minScore));
            return Math.Clamp(normalized - (index * 2), 65, 97);
        }

        private static List<string> CleanList(IEnumerable<string>? values, int maxCount)
        {
            return (values ?? Enumerable.Empty<string>())
                .Where(value => !string.IsNullOrWhiteSpace(value))
                .Select(value => Regex.Replace(value.Trim(), @"\s+", " "))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(maxCount)
                .ToList();
        }

        private static List<string> SplitTextIntoBullets(string? text, int maxCount)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return new List<string>();
            }

            return text
                .Split(new[] { '.', ',', ';', '|', '\n' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(part => part.Length > 3)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Take(maxCount)
                .ToList();
        }

        private static string BuildCareerPathText(CareerPath careerPath)
        {
            return string.Join(
                " ",
                new[]
                {
                    careerPath.PathName,
                    careerPath.Description,
                    careerPath.Category?.Name,
                    careerPath.SubCategory?.Name,
                    careerPath.Prerequisites,
                    careerPath.ExpectedOutcomes
                }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static HashSet<string> Tokenize(string? text)
        {
            return Regex.Matches(text ?? string.Empty, "[a-z0-9]{3,}", RegexOptions.IgnoreCase)
                .Select(match => match.Value.ToLowerInvariant())
                .ToHashSet();
        }

        private static bool ContainsAnyKeyword(string? text, IEnumerable<string> keywords)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            var tokens = Tokenize(text);
            return keywords.Any(keyword => KeywordMatches(text, tokens, keyword));
        }

        private static string? NormalizeAnswer(string? answer)
        {
            if (string.IsNullOrWhiteSpace(answer))
            {
                return null;
            }

            return Regex.Replace(answer.Trim(), @"\s+", " ");
        }

        private static bool IsChoiceQuestion(Question question)
        {
            return !question.QuestionType.Equals("text", StringComparison.OrdinalIgnoreCase);
        }

        private static bool KeywordMatches(string text, HashSet<string> tokens, string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return false;
            }

            return keyword.Contains(' ') || keyword.Contains('-')
                ? text.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                : tokens.Contains(keyword.ToLowerInvariant());
        }

        private sealed class CareerPathCandidate
        {
            public CareerPath Path { get; init; } = null!;
            public int HeuristicScore { get; init; }
            public int DisplayScore { get; set; }
            public List<string> Signals { get; init; } = new();
            public List<string> GrowthAreas { get; init; } = new();
        }

        private sealed record ThemeDefinition(string Label, string[] Keywords);

        private sealed record UserAssessmentContext(
            List<string> Skills,
            List<string> Education,
            List<string> Experiences);
    }
}
