using AutoMapper;
using BLL.Dtos.CategoryDtos;
using BLL.Dtos.CourseDtos;
using BLL.Dtos.CoursePlatformDtos;
using BLL.Dtos.CourseProgressDtos;
using BLL.Dtos.CvDtos;
using BLL.Dtos.EducationDtos;
using BLL.Dtos.SkillDtos;
using BLL.Dtos.UserExperienceDtos;
using BLL.Dtos.UserProfileDtos;
using DAL.Models;

namespace BLL.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // CV -> CvRS
            CreateMap<CV, CvRS>()
                .ForMember(dest => dest.ExtractedSkills, opt => opt.MapFrom(src => src.ExtractedSkills ?? new List<string>()))
                .ForMember(dest => dest.CVIssues, opt => opt.MapFrom(src => src.CVIssues ?? new List<string>()))
                .ForMember(dest => dest.SuggestedJobTitles, opt => opt.MapFrom(src => src.SuggestedJobTitles ?? new List<string>()))
                .ForMember(dest => dest.RecommendedSkills, opt => opt.MapFrom(src => src.RecommendedSkills ?? new List<string>()));

            // UserSkill -> UserSkillRS (includes Skill navigation)
            CreateMap<UserSkill, UserSkillRS>()
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill.SkillName))
                .ForMember(dest => dest.IsTechnical, opt => opt.MapFrom(src => src.Skill.IsTechnical))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Skill.Category ?? ""))
                .ForMember(dest => dest.ProficiencyLevel, opt => opt.MapFrom(src => src.ProficiencyLevel ?? "Not Specified"))
                .ForMember(dest => dest.Source, opt => opt.MapFrom(src => src.Source ?? "Manual"));

            // UserEducation -> UserEducationRS
            CreateMap<UserEducation, UserEducationRS>()
                .ForMember(dest => dest.CertificateUrls, opt => opt.MapFrom(src => src.CertificatePaths ?? new List<string>()));



            // UserProfile -> UserProfileRS
            CreateMap<UserProfile, UserProfileRS>();
            // UserProfileRQ -> UserProfile
            CreateMap<UserProfileRQ, UserProfile>();
            // UpdateUserProfileRQ -> UserProfile (ignore ProfilePictureUrl and only map non-null properties)
            CreateMap<UpdateUserProfileRQ, UserProfile>()
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.Ignore())
                .ForAllMembers(opts =>
                    opts.Condition((src, dest, srcMember) => srcMember != null));




            // UserExperience -> UserExperienceRS
            CreateMap<UserExperience, UserExperienceRS>();
            // UserExperienceRQ -> UserExperience
            CreateMap<UserExperienceRQ, UserExperience>();
            // UpdateUserExperienceRQ -> UserExperience (only map non-null properties)
            CreateMap<UpdateUserExperienceRQ, UserExperience>()  .ForAllMembers(opts =>  opts.Condition((src, dest, srcMember) => srcMember != null));


            // CoursePlatform -> CoursePlatformRS
            CreateMap<CoursePlatform, CoursePlatformRS>();
            // CoursePlatformRQ -> CoursePlatform
            CreateMap<CoursePlatformRQ, CoursePlatform>();
            // UpdateCoursePlatformRQ -> CoursePlatform (only map non-null properties)
            CreateMap<UpdateCoursePlatformRQ, CoursePlatform>()  .ForAllMembers(opts =>  opts.Condition((src, dest, srcMember) => srcMember != null));



            // Course -> CourseRS
            CreateMap<Course, CourseRS>()
                .ForMember(dest => dest.PlatformName, opt => opt.MapFrom(src => src.Platform != null ? src.Platform.Name : ""))
                .ForMember(dest => dest.PlatformLogo, opt => opt.MapFrom(src => src.Platform != null ? src.Platform.LogoUrl : ""))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : ""))
                .ForMember(dest => dest.SubCategoryName, opt => opt.MapFrom(src => src.SubCategory != null ? src.SubCategory.Name : ""))

                // 🟢 السطر الإضافي: توجيه AutoMapper لسحب المهارات
                .ForMember(dest => dest.CourseSkills, opt => opt.MapFrom(src => src.CourseSkills));
            // CourseRQ -> Course
            CreateMap<CourseRQ, Course>();



            //category -> categoryRS
            CreateMap<Category, CategoryRS>();
            CreateMap<CategoryRQ, Category>();
            CreateMap<SubCategory, SubCategoryRS>();
             CreateMap<SubCategoryRQ, SubCategory>();


            CreateMap<CourseProgress, CourseProgressRS>()
                .ForMember(dest => dest.CourseName, opt => opt.MapFrom(src => src.Course != null ? src.Course.Name : ""))
                .ForMember(dest => dest.CourseThumbnailUrl, opt => opt.MapFrom(src => src.Course != null ? src.Course.ThumbnailUrl : ""))
                .ForMember(dest => dest.TotalLessons, opt => opt.MapFrom(src => src.Course != null ? src.Course.TotalLessons : 1));

            CreateMap<UpdateProgressRQ, CourseProgress>()
                    .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));


            CreateMap<CourseSkill, BLL.Dtos.CourseSkillDtos.CourseSkillRS>()
                .ForMember(dest => dest.SkillName, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.SkillName : ""))
                .ForMember(dest => dest.SkillCategory, opt => opt.MapFrom(src => src.Skill != null ? src.Skill.Category : ""))
                .ForMember(dest => dest.IsTechnical, opt => opt.MapFrom(src => src.Skill != null && src.Skill.IsTechnical));
        }
    }
}
