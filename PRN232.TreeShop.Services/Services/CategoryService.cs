using AutoMapper;
using Microsoft.Extensions.Logging;
using PRN232.LaptopShop.Repo.Entities;
using PRN232.LaptopShop.Repo.Repository;
using PRN232.LaptopShop.Services.Commons.Results;
using PRN232.LaptopShop.Services.Request;
using PRN232.LaptopShop.Services.Response;
using Repository.Utils;

namespace PRN232.LaptopShop.Services.Services
{
    public class CategoryService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger<CategoryService> _logger;
        private readonly IMapper _mapper;

        public CategoryService(UnitOfWork unitOfWork, ILogger<CategoryService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<CategoryResponse>> CreateCategory(CategoryRequest request)
        {
            try
            {
                var newCategory = new Category
                {
                    CategoryName = request.CategoryName,
                    Description = request.Description,
                    Status = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.CategoryRepo.Add(newCategory);
                await _unitOfWork.SaveChangesAsync();

                var response = new CategoryResponse
                {
                    Id = newCategory.Id,
                    CategoryName = newCategory.CategoryName,
                    Description = newCategory.Description,
                    //Status = newCategory.Status,
                    CreatedAt = newCategory.CreatedAt
                };

                return Result<CategoryResponse>.Success(response, 201);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating a category");
                return Result<CategoryResponse>.Failure(null, 500, "An error occurred while creating a category");
            }
        }

        public async Task<Result<CategoryResponse>> GetCategoryById(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepo.FindAsync(c => c.Id == id && c.Status == true);
                if (category == null)
                {
                    return Result<CategoryResponse>.Failure(null, 404, "Category not found");
                }

                var response = new CategoryResponse
                {
                    Id = category.Id,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    //Status = category.Status,
                    CreatedAt = category.CreatedAt
                };

                return Result<CategoryResponse>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving the category");
                return Result<CategoryResponse>.Failure(null, 500, "An error occurred while retrieving the category");
            }
        }


        public async Task<Result<CategoryResponse>> UpdateCategory(int id, CategoryRequest request)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepo.FindAsync(c => c.Id == id && c.Status == true);
                if (category == null)
                {
                    return Result<CategoryResponse>.Failure(null, 404, "Category not found");
                }

                category.CategoryName = request.CategoryName;
                category.Description = request.Description;

                _unitOfWork.CategoryRepo.Update(category);
                await _unitOfWork.SaveChangesAsync();

                var response = new CategoryResponse
                {
                    Id = category.Id,
                    CategoryName = category.CategoryName,
                    Description = category.Description,
                    //Status = category.Status,
                    CreatedAt = category.CreatedAt
                };

                return Result<CategoryResponse>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating the category");
                return Result<CategoryResponse>.Failure(null, 500, "An error occurred while updating the category");
            }
        }

        public async Task<Result<CategoryResponse>> DeleteCategory(int id)
        {
            try
            {
                var category = await _unitOfWork.CategoryRepo.FindAsync(c => c.Id == id && c.Status == true);
                if (category == null)
                {
                    return Result<CategoryResponse>.Failure(null, 404, "Category not found");
                }

                // Soft delete: change status to false
                category.Status = false;
                _unitOfWork.CategoryRepo.Update(category);
                await _unitOfWork.SaveChangesAsync();

                return Result<CategoryResponse>.Success(null, 200);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while deleting the category");
                return Result<CategoryResponse>.Failure(null, 500, "An error occurred while deleting the category");
            }
        }

        public async Task<Result<BasePaginatedList<object>>> GetAllCategories(
            int pageIndex,
            int pageSize,
            string? categoryName,
            string? orderBy,
            string? fields)
        {
            var query = _unitOfWork.CategoryRepo.AsQueryable();
            query = query.Where(c => c.Status == true);

            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(c => c.CategoryName.Contains(categoryName));
            }

            // phải bat91 buộc có mapper từ Category sang CategoryResponse để có thể sử dụng
            var mapperConfig = _mapper.ConfigurationProvider;

            var pagingResult = await _unitOfWork.CategoryRepo
                .GetAllWithPaggingSortSelectionFieldAsync<Category, CategoryResponse>(
                query, 
                mapperConfig, 
                orderBy, 
                fields, 
                pageIndex, 
                pageSize);

            if (pagingResult == null || pagingResult.Items.Count == 0)
            {
                return Result<BasePaginatedList<object>>.Failure(null, 404, "No categories found");
            }

            return Result<BasePaginatedList<object>>.Success(pagingResult, 200);

        }
    }
}
