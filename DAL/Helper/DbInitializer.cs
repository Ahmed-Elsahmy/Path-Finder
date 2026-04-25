using DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class DbInitializer
{
    private const string CareerAssessmentType = "CareerDiscovery";

    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<AppDbContext>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        string adminEmail = "admin@example.com";
        string adminPassword = "Admin@123";
        string roleName = "Admin";

        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var user = await userManager.FindByEmailAsync(adminEmail);

        if (user == null)
        {
            user = new User
            {
                UserName = "Admin1",
                Email = adminEmail,
                EmailConfirmed = true,
                FirstName = "System",
                LastName = "Admin"
            };

            var result = await userManager.CreateAsync(user, adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }

        await SeedCareerAssessmentAsync(context);
    }

    private static async Task SeedCareerAssessmentAsync(AppDbContext context)
    {
        var alreadySeeded = await context.Questionnaires
            .AnyAsync(q => q.QuestionnaireType == CareerAssessmentType);

        if (alreadySeeded)
        {
            return;
        }

        var questionnaire = new Questionnaire
        {
            Title = "Career Compass Assessment",
            Description = "A guided test that uses your answers and the available career paths inside Path Finder to recommend the best next direction.",
            QuestionnaireType = CareerAssessmentType,
            IsActive = true,
            Questions = new List<Question>
            {
                new()
                {
                    QuestionText = "Which kind of work sounds most exciting to you?",
                    QuestionType = "single_choice",
                    OrderNumber = 1,
                    IsRequired = true,
                    Options = new List<string>
                    {
                        "Build web and mobile applications",
                        "Analyze data and find patterns",
                        "Design user experiences and interfaces",
                        "Plan projects and coordinate teams",
                        "Protect systems and solve security risks",
                        "Understand business needs and improve processes"
                    }
                },
                new()
                {
                    QuestionText = "When you face a new challenge, what is your instinct?",
                    QuestionType = "single_choice",
                    OrderNumber = 2,
                    IsRequired = true,
                    Options = new List<string>
                    {
                        "Prototype a practical solution",
                        "Research and compare evidence",
                        "Sketch the user journey",
                        "Organize people and priorities",
                        "Investigate root causes and weak points",
                        "Translate the problem into business goals"
                    }
                },
                new()
                {
                    QuestionText = "Which type of result makes you most proud?",
                    QuestionType = "single_choice",
                    OrderNumber = 3,
                    IsRequired = true,
                    Options = new List<string>
                    {
                        "A product people can use immediately",
                        "A report or dashboard with clear insights",
                        "An interface that feels simple and delightful",
                        "A team that delivered smoothly",
                        "A stable and secure system",
                        "A process that saves time and money"
                    }
                },
                new()
                {
                    QuestionText = "What kind of daily work environment fits you best?",
                    QuestionType = "single_choice",
                    OrderNumber = 4,
                    IsRequired = true,
                    Options = new List<string>
                    {
                        "Hands-on building with frequent experimentation",
                        "Deep focus with analysis and problem solving",
                        "Creative collaboration with users and feedback",
                        "Fast-moving teamwork and decision making",
                        "Structured systems and operational discipline",
                        "Cross-functional work between business and tech"
                    }
                },
                new()
                {
                    QuestionText = "Which skill do you most want to strengthen next?",
                    QuestionType = "single_choice",
                    OrderNumber = 5,
                    IsRequired = true,
                    Options = new List<string>
                    {
                        "Programming and software architecture",
                        "Statistics, SQL, or dashboards",
                        "Visual design and user research",
                        "Leadership and project execution",
                        "Networking, cloud, or security foundations",
                        "Requirements analysis and stakeholder communication"
                    }
                },
                new()
                {
                    QuestionText = "Which statement sounds most like you?",
                    QuestionType = "single_choice",
                    OrderNumber = 6,
                    IsRequired = true,
                    Options = new List<string>
                    {
                        "I like turning ideas into working software",
                        "I enjoy making decisions using evidence and data",
                        "I care deeply about how people experience products",
                        "I like aligning people around a plan",
                        "I notice risks before others do",
                        "I like connecting user problems to business value"
                    }
                },
                new()
                {
                    QuestionText = "What type of impact do you want your career to have?",
                    QuestionType = "single_choice",
                    OrderNumber = 7,
                    IsRequired = true,
                    Options = new List<string>
                    {
                        "Ship useful digital products",
                        "Help teams make smarter decisions",
                        "Make technology easier and more human",
                        "Lead delivery and create momentum",
                        "Keep platforms safe and reliable",
                        "Improve how organizations operate"
                    }
                },
                new()
                {
                    QuestionText = "Which learning style helps you grow fastest?",
                    QuestionType = "single_choice",
                    OrderNumber = 8,
                    IsRequired = true,
                    Options = new List<string>
                    {
                        "Building projects from scratch",
                        "Working through cases and datasets",
                        "Reviewing interfaces and user behavior",
                        "Leading group tasks and retrospectives",
                        "Studying systems, logs, and threat patterns",
                        "Mapping business processes end to end"
                    }
                },
                new()
                {
                    QuestionText = "Tell us about a problem you would love to spend years solving.",
                    QuestionType = "text",
                    OrderNumber = 9,
                    IsRequired = false,
                    Options = new List<string>()
                }
            }
        };

        await context.Questionnaires.AddAsync(questionnaire);
        await context.SaveChangesAsync();
    }
}
