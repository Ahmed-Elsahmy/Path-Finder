using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DAL.Models
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var listComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            var nullableListComparer = new ValueComparer<List<string>?>(
                (c1, c2) => (c1 == null && c2 == null)
                         || (c1 != null && c2 != null && c1.SequenceEqual(c2)),
                c => c == null ? 0 : c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c == null ? null : c.ToList()
            );

            modelBuilder.Entity<CV>()
                .Property(c => c.ExtractedSkills)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(nullableListComparer);

            modelBuilder.Entity<CV>()
                .Property(c => c.CVIssues)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(nullableListComparer);

            modelBuilder.Entity<CV>()
                .Property(c => c.SuggestedJobTitles)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(nullableListComparer);

            modelBuilder.Entity<CV>()
                .Property(c => c.RecommendedSkills)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(nullableListComparer);

            modelBuilder.Entity<UserExperience>()
               .Property(x => x.EmploymentType)
                   .HasConversion<string>();

            modelBuilder.Entity<CareerPath>()
                .Property(x => x.DifficultyLevel)
                    .HasConversion<string>();

            modelBuilder.Entity<UserCareerPath>()
                .Property(x => x.Status)
                    .HasConversion<string>();

            // ═══ Job Module Indexes ═══
            modelBuilder.Entity<JobSource>()
                .HasIndex(js => js.SourceName)
                .IsUnique();

            modelBuilder.Entity<JobApplication>()
                .HasIndex(ja => new { ja.UserId, ja.JobId })
                .IsUnique(); // Prevent duplicate applications

            modelBuilder.Entity<SavedJob>()
                .HasIndex(sj => new { sj.UserId, sj.JobId })
                .IsUnique(); // Prevent duplicate saves

            modelBuilder.Entity<Notification>()
                .HasIndex(n => new { n.UserId, n.IsRead });

            modelBuilder.Entity<Notification>()
                .Property(n => n.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            modelBuilder.Entity<Notification>()
                .Property(n => n.IsRead)
                .HasDefaultValue(false);
        }

        public DbSet<CV> CVs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<UserEducation> UserEducations { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<UserExperience> UserExperiences { get; set; }
        public DbSet<CoursePlatform> CoursePlatforms { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories { get; set; }
        public DbSet<CourseProgress> CourseProgresses { get; set; }
        public DbSet<CourseSkill> CourseSkills { get; set; }
        public DbSet<CareerPath> CareerPaths { get; set; }
        public DbSet<UserCareerPath> UserCareerPaths { get; set; }
        public DbSet<CareerPathCourse> CareerPathCourses { get; set; }

        // ═══ Job Module ═══
        public DbSet<JobSource> JobSources { get; set; }
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobSkillRequirement> JobSkillRequirements { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }

        public DbSet<Notification> Notifications { get; set; }

    }
}
