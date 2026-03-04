using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LaptopShop.Services.Commons.Results;
using PRN232.LaptopShop.Services.Request;
using PRN232.LaptopShop.Services.Response;
using PRN232.LaptopShop.Services.Services;
using Repository.Utils;

namespace PRN232.LaptopShop.API.Controllers
{
    [Route("api/v1/categories")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _categoryService;

        public CategoriesController(CategoryService categoryService)
        {
            _categoryService = categoryService;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryRequest request)
        {
            var result = await _categoryService.CreateCategory(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<CategoryResponse>.Ok(result.Value!, "Category created successfully"));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            var result = await _categoryService.GetCategoryById(id);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<CategoryResponse>.Ok(result.Value!, "Category retrieved successfully"));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllCategories(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? orderBy = null,
            [FromQuery] string? fields = null,
            [FromQuery] string? categoryName = null)
        {
            var result = await _categoryService.GetAllCategories(pageIndex, pageSize, categoryName, orderBy, fields);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<BasePaginatedList<object>>.Ok(result.Value!, "Categories retrieved successfully"));
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] CategoryRequest request)
        {
            var result = await _categoryService.UpdateCategory(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<CategoryResponse>.Ok(result.Value!, "Category updated successfully"));
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory([FromRoute] int id)
        {
            var result = await _categoryService.DeleteCategory(id);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<string>.Ok("", "Category deleted successfully"));
        }
    }
}
