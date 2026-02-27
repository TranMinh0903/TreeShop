using FluentValidation;
using PRN232.LaptopShop.Services.Request;

namespace PRN232.LaptopShop.Services.Validator
{
    public class ProductRequestValidator : AbstractValidator<ProductRequest>
    {
        public ProductRequestValidator()
        {
            RuleFor(x => x.ProductName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Product name is required")
                .MinimumLength(3).WithMessage("Product name must be at least 3 characters long")
                .MaximumLength(200).WithMessage("Product name must not exceed 200 characters");

            RuleFor(x => x.Price)
                .NotEmpty().WithMessage("Price is required")
                .GreaterThan(0).WithMessage("Price must be greater than 0");

            RuleFor(x => x.StockQuantity)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Stock quantity is required")
                .GreaterThanOrEqualTo(0).WithMessage("Stock quantity must be greater than or equal to 0");

            RuleFor(x => x.CategoryId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Category ID is required")
                .GreaterThan(0).WithMessage("Category ID must be greater than 0");
        }
    }
}
