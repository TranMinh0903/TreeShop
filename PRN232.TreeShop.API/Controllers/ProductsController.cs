using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PRN232.LaptopShop.Services.Commons.Results;
using PRN232.LaptopShop.Services.Request;
using PRN232.LaptopShop.Services.Response;
using PRN232.LaptopShop.Services.Services;
using Repository.Utils;

namespace PRN232.LaptopShop.API.Controllers
{
    [Route("api/v1/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] ProductRequest request)
        {
            var result = await _productService.CreateProduct(request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<ProductDetailResponse>.Ok(result.Value!, "Product created successfully"));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById([FromRoute] int id)
        {
            var result = await _productService.GetProductById(id);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<ProductDetailResponse>.Ok(result.Value!, "Product retrieved successfully"));
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProducts(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? orderBy = null,
            [FromQuery] string? fields = null,
            [FromQuery] string? productName = null,
            [FromQuery] decimal? minPrice = null,
            [FromQuery] decimal? maxPrice = null,
            [FromQuery] int? stockQuantity = null,
            [FromQuery] string? categoryName = null)
        {
            var result = await _productService.GetAllProducts(
                pageIndex, pageSize, productName, minPrice, maxPrice, stockQuantity, categoryName, orderBy, fields);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<BasePaginatedList<object>>.Ok(result.Value!, "Products retrieved successfully"));
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct([FromRoute] int id, [FromBody] ProductRequest request)
        {
            var result = await _productService.UpdateProduct(id, request);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<ProductDetailResponse>.Ok(result.Value!, "Product updated successfully"));
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            var result = await _productService.DeleteProduct(id);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode, ApiResponse<string>.Fail(message: result.Errors!));
            }
            return StatusCode(result.StatusCode, ApiResponse<string>.Ok("", "Product deleted successfully"));
        }
    }
}
