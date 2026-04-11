using BLL.Mapping;
using BLL.Services.AuthService;
using BLL.Services.ChatbotService;
using BLL.Services.CourseCategoryService;
using BLL.Services.CoursePlatformService;
using BLL.Services.CoursePlatformServices;
using BLL.Services.CourseProgressService;
using BLL.Services.CourseService;
using BLL.Services.CvService;
using BLL.Services.EducationService;
using BLL.Services.EducationServices;
using BLL.Services.EmailService;
using BLL.Services.SkillService;
using BLL.Services.UserExperienceServices;
using BLL.Services.UserProfileServices;
using DAL.Helper;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Path_Finder.Middleware;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

namespace Path_Finder
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Configure Serilog from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
                .AddUserSecrets<Program>(optional: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .CreateLogger();

            try
            {
                Log.Information("Starting Path Finder API...");

                var builder = WebApplication.CreateBuilder(args);
                builder.Host.UseSerilog();

                // Add services to the container.
                builder.Services.AddControllers()
             .AddJsonOptions(options =>
             {
                 options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
             });
                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen(options =>
                {
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = "Bearer",
                        BearerFormat = "JWT",
                        In = ParameterLocation.Header,
                        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1Ni...\""
                    });

                    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new List<string>()
                        }
                    });
                });

                // Database
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

                // Identity
                builder.Services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<AppDbContext>()
                    .AddDefaultTokenProviders();

                builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

                builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IEmailService, EmailService>();
                builder.Services.AddScoped<ISkillService, SkillService>();
                builder.Services.AddScoped<ICvService, CvService>();
                builder.Services.AddScoped<IEducationService, EducationService>();
                builder.Services.AddScoped<IChatbotService, ChatbotService>();
                builder.Services.AddScoped<IUserProfileService, UserProfileService>();
                builder.Services.AddScoped<IUserExperienceService, UserExperienceService>();
                builder.Services.AddScoped<ICoursePlatformService, CoursePlatformService>();
                builder.Services.AddScoped<ICourseService, CourseService>();
                builder.Services.AddScoped<ICourseCategoryService, CourseCategoryService>();
                builder.Services.AddScoped<ICourseProgressService, CourseProgressService>();
                // JWT Configuration
                builder.Services.Configure<JWT>(builder.Configuration.GetSection("JWT"));

                // CORS
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowAllClients", policy =>
                    {
                        policy.AllowAnyOrigin()
                              .AllowAnyMethod()
                              .AllowAnyHeader();
                    });
                });

                // HttpClient for Gemini API
                builder.Services.AddHttpClient("GeminiClient", client =>
                {
                    client.Timeout = TimeSpan.FromSeconds(60);
                });

                // Authentication
                builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(o =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = false;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidIssuer = builder.Configuration["JWT:Issuer"],
                        ValidAudience = builder.Configuration["JWT:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Key"]))
                    };
                });

                var app = builder.Build();

                // Configure the HTTP request pipeline.

                // Global Exception Handler (must be first in pipeline)
                app.UseMiddleware<GlobalExceptionMiddleware>();

                if (app.Environment.IsDevelopment())
                {
                    app.MapOpenApi();
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }

                app.UseHttpsRedirection();
                app.UseStaticFiles();
                app.UseRouting();

                // Serilog request logging
                app.UseSerilogRequestLogging();

                app.UseCors("AllowAllClients");

                app.UseAuthentication();
                app.UseAuthorization();

                app.MapControllers();
                using (var scope = app.Services.CreateScope())
                {
                    var services = scope.ServiceProvider;
                    DbInitializer.SeedAsync(services).GetAwaiter().GetResult();
                }
                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application failed to start.");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}