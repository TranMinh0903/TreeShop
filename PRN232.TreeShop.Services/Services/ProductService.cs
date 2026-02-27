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
    public class ProductService
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly ILogger<ProductService> _logger;
        private readonly IMapper _mapper;

        public ProductService(UnitOfWork unitOfWork, ILogger<ProductService> logger, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<Result<ProductDetailResponse>> CreateProduct(ProductRequest request)
        {
            try
            {
                // Check if category exists
                var category = await _unitOfWork.CategoryRepo.FindAsync(c => c.Id == request.CategoryId && c.Status == true);
                if (category == null)
                {
                    return Result<ProductDetailResponse>.Failure(null, 404, "Category not found");
                }

                var newProduct = new Product
                {
                    ProductName = request.ProductName,
                    Price = request.Price,
                    StockQuantity = request.StockQuantity,
                    Status = true,
                    CategoryId = request.CategoryId,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.ProductRepo.Add(newProduct);
                await _unitOfWork.SaveChangesAsync();

                var response = new ProductDetailResponse
                {
                    Id = newProduct.Id,
                    ProductName = newProduct.ProductName,
                    Price = newProduct.Price,
                    StockQuantity = newProduct.StockQuantity,
                    CreatedAt = newProduct.CreatedAt,
                    CategoryId = newProduct.CategoryId,
                    CategoryName = category.CategoryName
                };

                return Result<ProductDetailResponse>.Success(response, 201);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while creating a product");
                return Result<ProductDetailResponse>.Failure(null, 500, "An error occurred while creating a product");
            }
        }

        public async Task<Result<ProductDetailResponse>> GetProductById(int id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepo.FindAsync(p => p.Id == id && p.Status == true);
                if (product == null)
                {
                    return Result<ProductDetailResponse>.Failure(null, 404, "Product not found");
                }

                var category = await _unitOfWork.CategoryRepo.FindAsync(c => c.Id == product.CategoryId);

                var response = new ProductDetailResponse
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CreatedAt = product.CreatedAt,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.CategoryName
                };

                return Result<ProductDetailResponse>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while retrieving the product");
                return Result<ProductDetailResponse>.Failure(null, 500, "An error occurred while retrieving the product");
            }
        }


        public async Task<Result<ProductDetailResponse>> UpdateProduct(int id, ProductRequest request)
        {
            try
            {
                var product = await _unitOfWork.ProductRepo.FindAsync(p => p.Id == id && p.Status == true);
                if (product == null)
                {
                    return Result<ProductDetailResponse>.Failure(null, 404, "Product not found");
                }

                // Check if category exists
                var category = await _unitOfWork.CategoryRepo.FindAsync(c => c.Id == request.CategoryId && c.Status == true);
                if (category == null)
                {
                    return Result<ProductDetailResponse>.Failure(null, 404, "Category not found");
                }

                product.ProductName = request.ProductName;
                product.Price = request.Price;
                product.StockQuantity = request.StockQuantity;
                product.CategoryId = request.CategoryId;

                _unitOfWork.ProductRepo.Update(product);
                await _unitOfWork.SaveChangesAsync();

                var response = new ProductDetailResponse
                {
                    Id = product.Id,
                    ProductName = product.ProductName,
                    Price = product.Price,
                    StockQuantity = product.StockQuantity,
                    CreatedAt = product.CreatedAt,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category.CategoryName
                };

                return Result<ProductDetailResponse>.Success(response);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while updating the product");
                return Result<ProductDetailResponse>.Failure(null, 500, "An error occurred while updating the product");
            }
        }

        public async Task<Result<ProductDetailResponse>> DeleteProduct(int id)
        {
            try
            {
                var product = await _unitOfWork.ProductRepo.FindAsync(p => p.Id == id && p.Status == true);
                if (product == null)
                {
                    return Result<ProductDetailResponse>.Failure(null, 404, "Product not found");
                }

                // Soft delete: change status to false
                product.Status = false;
                _unitOfWork.ProductRepo.Update(product);
                await _unitOfWork.SaveChangesAsync();

                return Result<ProductDetailResponse>.Success(null, 204);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred while deleting the product");
                return Result<ProductDetailResponse>.Failure(null, 500, "An error occurred while deleting the product");
            }
        }


        public async Task<Result<BasePaginatedList<object>>> GetAllProducts(
            int pageIndex,
            int pageSize,
            string? productName,
            decimal? minPrice,
            decimal? maxPrice,
            int? stockQuantity,
            string? categoryName,
            string? orderBy, 
            string? fields)
        {
            var query = _unitOfWork.ProductRepo.AsQueryable();
            query = query.Where(p => p.Status == true);

            // product name
            if (!string.IsNullOrEmpty(productName))
            {
                query = query.Where(p => p.ProductName.Contains(productName));
            }

            // stock quantity
            if (stockQuantity.HasValue)
            {
                query = query.Where(p => p.StockQuantity == stockQuantity.Value);
            }

            // category name
            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(p => p.Category.CategoryName.Contains(categoryName));
            }

            // price range
            if (minPrice.HasValue && maxPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value && p.Price <= maxPrice.Value);
            }
            else if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }
            else if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            var mapperConfig = _mapper.ConfigurationProvider;


            var pagingResult = await _unitOfWork.ProductRepo
                .GetAllWithPaggingSortSelectionFieldAsync<Product, ProductDetailResponse>(
                query,
                mapperConfig,
                orderBy,
                fields,
                pageIndex,
                pageSize);

            if (pagingResult == null || pagingResult.Items.Count == 0)
            {
                return Result<BasePaginatedList<object>>.Failure(null, 404, "No products found");
            }

            return Result<BasePaginatedList<object>>.Success(pagingResult);
        }
    }
}