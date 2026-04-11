using BLL.Common;
using BLL.Dtos.CategoryDtos;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BLL.Services.CourseCategoryService
{
    public interface ICourseCategoryService
    {
        Task<ServiceResult<List<CategoryRS>>> GetAllCategoriesAsync();
        Task<ServiceResult<List<SubCategoryRS>>> GetSubCategoriesByCategoryIdAsync(int categoryId);

        Task<ServiceResult<string>> CreateCategoryAsync(CategoryRQ request);
        Task<ServiceResult<string>> UpdateCategoryAsync(int id, CategoryRQ request);
        Task<ServiceResult<string>> DeleteCategoryAsync(int id);

        Task<ServiceResult<string>> CreateSubCategoryAsync(SubCategoryRQ request);
        Task<ServiceResult<string>> UpdateSubCategoryAsync(int id, SubCategoryRQ request);
        Task<ServiceResult<string>> DeleteSubCategoryAsync(int id);
        Task<ServiceResult<string>> ImportCategoriesFromJsonAsync(IFormFile file);

    }
}