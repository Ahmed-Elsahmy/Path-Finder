using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

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

            // ✅ ValueComparer tells EF Core how to compare List<string> for change tracking
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

            // ✅ ExtractedSkills — List<string>?
            modelBuilder.Entity<CV>()
                .Property(c => c.ExtractedSkills)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(nullableListComparer);

            // ✅ CVIssues — List<string>?
            modelBuilder.Entity<CV>()
                .Property(c => c.CVIssues)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(nullableListComparer);

            // ✅ SuggestedJobTitles — List<string>?
            modelBuilder.Entity<CV>()
                .Property(c => c.SuggestedJobTitles)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(nullableListComparer);

            // ✅ RecommendedSkills — List<string>?
            modelBuilder.Entity<CV>()
                .Property(c => c.RecommendedSkills)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null)
                         ?? new List<string>()
                )
                .HasColumnType("nvarchar(max)")
                .Metadata.SetValueComparer(nullableListComparer);
        }

        public DbSet<CV> CVs { get; set; }
        public DbSet<Skill> Skills { get; set; }
        public DbSet<UserSkill> UserSkills { get; set; }
        public DbSet<UserEducation> UserEducations { get; set; }
    }
}