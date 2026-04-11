using BLL.Common;
using BLL.Dtos.CategoryDtos;
using BLL.Services.CourseCategoryService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Path_Finder.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseCategoryController : ControllerBase
    {
        private readonly ICourseCategoryService _categoryService;

        public CourseCategoryController(ICourseCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            if (result.IsSuccess) return Ok(new { Data = result.Data, Message = result.Data as string });

            return result.ErrorCode switch
            {
                ServiceErrorCode.NotFound => NotFound(new { Message = result.ErrorMessage }),
                ServiceErrorCode.UpstreamServiceError => StatusCode(503, new { Message = result.ErrorMessage }),
                _ => BadRequest(new { Message = result.ErrorMessage })
            };
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllCategories() => HandleResult(await _categoryService.GetAllCategoriesAsync());

        [HttpGet("{categoryId}/subcategories")]
        public async Task<IActionResult> GetSubCats(int categoryId) => HandleResult(await _categoryService.GetSubCategoriesByCategoryIdAsync(categoryId)); 
        [HttpPost("category/add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRQ req) => HandleResult(await _categoryService.CreateCategoryAsync(req)); 
        [HttpPut("category/update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryRQ req) => HandleResult(await _categoryService.UpdateCategoryAsync(id, req));

        [HttpDelete("category/delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id) => HandleResult(await _categoryService.DeleteCategoryAsync(id));

        [HttpPost("subcategory/add")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSubCategory([FromBody] SubCategoryRQ req) => HandleResult(await _categoryService.CreateSubCategoryAsync(req));

        [HttpPut("subcategory/update/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubCategory(int id, [FromBody] SubCategoryRQ req) => HandleResult(await _categoryService.UpdateSubCategoryAsync(id, req)); [HttpDelete("subcategory/delete/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubCategory(int id) => HandleResult(await _categoryService.DeleteSubCategoryAsync(id));
        [HttpPost("import-json")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ImportCategoriesFromJson(IFormFile file)
        {
            var result = await _categoryService.ImportCategoriesFromJsonAsync(file);
            return HandleResult(result);
        }
    }
}