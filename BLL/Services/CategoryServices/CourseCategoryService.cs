using AutoMapper;
using BLL.Common;
using BLL.Dtos.CategoryDtos;
using DAL.Models;
using DAL.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace BLL.Services.CourseCategoryService
{
    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly IRepository<Category> _categoryRepo;
        private readonly IRepository<SubCategory> _subCategoryRepo;
        private readonly IMapper _mapper;
        private readonly ILogger<CourseCategoryService> _logger;

        public CourseCategoryService(
            IRepository<Category> categoryRepo,
            IRepository<SubCategory> subCategoryRepo,
            IMapper mapper,
            ILogger<CourseCategoryService> logger)
        {
            _categoryRepo = categoryRepo;
            _subCategoryRepo = subCategoryRepo;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<List<CategoryRS>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepo.Query()
                    .Include(c => c.SubCategories)
                    .ToListAsync();

                return ServiceResult<List<CategoryRS>>.Success(_mapper.Map<List<CategoryRS>>(categories));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories.");
                return ServiceResult<List<CategoryRS>>.Failure("An error occurred.", ServiceErrorCode.UpstreamServiceError);
            }
        }

        public async Task<ServiceResult<List<SubCategoryRS>>> GetSubCategoriesByCategoryIdAsync(int categoryId)
        {
            var subCats = await _subCategoryRepo.FindAsync(sc => sc.CategoryId == categoryId);
            return ServiceResult<List<SubCategoryRS>>.Success(_mapper.Map<List<SubCategoryRS>>(subCats));
        }

        public async Task<ServiceResult<string>> CreateCategoryAsync(CategoryRQ request)
        {
            var exists = await _categoryRepo.AnyAsync(c => c.Name.ToLower() == request.Name.ToLower());
            if (exists) return ServiceResult<string>.Failure("Category already exists.", ServiceErrorCode.ValidationError);

            await _categoryRepo.AddAsync(_mapper.Map<Category>(request));
            await _categoryRepo.SaveChangesAsync();
            return ServiceResult<string>.Success("Category created successfully.");
        }

        public async Task<ServiceResult<string>> UpdateCategoryAsync(int id, CategoryRQ request)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return ServiceResult<string>.Failure("Category not found.", ServiceErrorCode.NotFound);

            category.Name = request.Name;
            _categoryRepo.Update(category);
            await _categoryRepo.SaveChangesAsync();
            return ServiceResult<string>.Success("Category updated successfully.");
        }

        public async Task<ServiceResult<string>> DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepo.GetByIdAsync(id);
            if (category == null) return ServiceResult<string>.Failure("Category not found.", ServiceErrorCode.NotFound);

            _categoryRepo.Remove(category);
            await _categoryRepo.SaveChangesAsync();
            return ServiceResult<string>.Success("Category deleted successfully.");
        }

        public async Task<ServiceResult<string>> CreateSubCategoryAsync(SubCategoryRQ request)
        {
            var catExists = await _categoryRepo.AnyAsync(c => c.Id == request.CategoryId);
            if (!catExists) return ServiceResult<string>.Failure("Parent Category not found.", ServiceErrorCode.NotFound);

            await _subCategoryRepo.AddAsync(_mapper.Map<SubCategory>(request));
            await _subCategoryRepo.SaveChangesAsync();
            return ServiceResult<string>.Success("SubCategory created successfully.");
        }

        public async Task<ServiceResult<string>> UpdateSubCategoryAsync(int id, SubCategoryRQ request)
        {
            var subCat = await _subCategoryRepo.GetByIdAsync(id);
            if (subCat == null) return ServiceResult<string>.Failure("SubCategory not found.", ServiceErrorCode.NotFound);

            subCat.Name = request.Name;

            if (subCat.CategoryId != request.CategoryId)
            {
                var catExists = await _categoryRepo.AnyAsync(c => c.Id == request.CategoryId);
                if (!catExists) return ServiceResult<string>.Failure("Parent Category not found.", ServiceErrorCode.NotFound);
                subCat.CategoryId = request.CategoryId;
            }

            _subCategoryRepo.Update(subCat);
            await _subCategoryRepo.SaveChangesAsync();
            return ServiceResult<string>.Success("SubCategory updated successfully.");
        }

        public async Task<ServiceResult<string>> DeleteSubCategoryAsync(int id)
        {
            var subCat = await _subCategoryRepo.GetByIdAsync(id);
            if (subCat == null) return ServiceResult<string>.Failure("SubCategory not found.", ServiceErrorCode.NotFound);

            _subCategoryRepo.Remove(subCat);
            await _subCategoryRepo.SaveChangesAsync();
            return ServiceResult<string>.Success("SubCategory deleted successfully.");
        }
        public async Task<ServiceResult<string>> ImportCategoriesFromJsonAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return ServiceResult<string>.Failure("No file uploaded.", ServiceErrorCode.ValidationError);

            if (Path.GetExtension(file.FileName).ToLower() != ".json")
                return ServiceResult<string>.Failure("Only JSON files are allowed.", ServiceErrorCode.ValidationError);

            try
            {
                using var stream = new StreamReader(file.OpenReadStream());
                var content = await stream.ReadToEndAsync();

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var importData = JsonSerializer.Deserialize<List<ImportCategoryDto>>(content, options);

                if (importData == null || !importData.Any())
                    return ServiceResult<string>.Failure("Invalid or empty JSON file.", ServiceErrorCode.ValidationError);

                int categoriesAdded = 0;
                int subCategoriesAdded = 0;

                foreach (var catDto in importData)
                {
                    var category = await _categoryRepo.FirstOrDefaultAsync(c => c.Name.ToLower() == catDto.Name.ToLower());
                    if (category == null)
                    {
                        category = new Category { Name = catDto.Name };
                        await _categoryRepo.AddAsync(category);
                        await _categoryRepo.SaveChangesAsync(); 
                        categoriesAdded++;
                    }

                    if (catDto.SubCategories != null && catDto.SubCategories.Any())
                    {
                        foreach (var subName in catDto.SubCategories)
                        {
                            var subExists = await _subCategoryRepo.AnyAsync(sc =>
                                sc.Name.ToLower() == subName.ToLower() && sc.CategoryId == category.Id);

                            if (!subExists)
                            {
                                var subCategory = new SubCategory
                                {
                                    Name = subName,
                                    CategoryId = category.Id
                                };
                                await _subCategoryRepo.AddAsync(subCategory);
                                subCategoriesAdded++;
                            }
                        }
                    }
                }
                await _subCategoryRepo.SaveChangesAsync();

                return ServiceResult<string>.Success($"Import successful! Added {categoriesAdded} Categories and {subCategoriesAdded} SubCategories.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing categories from JSON");
                return ServiceResult<string>.Failure($"An error occurred while importing: {ex.Message}", ServiceErrorCode.UpstreamServiceError);
            }
        }
    }
}