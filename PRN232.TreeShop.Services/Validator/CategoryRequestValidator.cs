using FluentValidation;
using PRN232.LaptopShop.Services.Request;

namespace PRN232.LaptopShop.Services.Validator
{
    public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(x => x.CategoryName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Category name is required")
                .MinimumLength(3).WithMessage("Category name must be at least 3 characters long")
                .MaximumLength(100).WithMessage("Category name must not exceed 100 characters");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Description must not exceed 500 characters");
        }
    }
}
