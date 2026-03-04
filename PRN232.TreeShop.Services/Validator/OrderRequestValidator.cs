using FluentValidation;
using PRN232.LaptopShop.Services.Request;

namespace PRN232.LaptopShop.Services.Validator
{
    public class OrderRequestValidator : AbstractValidator<OrderRequest>
    {
        public OrderRequestValidator()
        {
            RuleFor(x => x.ReceiverName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Receiver name is required")
                .MaximumLength(200).WithMessage("Receiver name must not exceed 200 characters");

            RuleFor(x => x.ReceiverPhone)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Receiver phone is required")
                .Matches(@"^[0-9]{9,11}$").WithMessage("Phone number must be 9-11 digits");

            RuleFor(x => x.ShippingAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Shipping address is required")
                .MaximumLength(500).WithMessage("Shipping address must not exceed 500 characters");

            RuleFor(x => x.Items)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Order must have at least one item")
                .Must(items => items.Count > 0).WithMessage("Order must have at least one item");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId)
                    .GreaterThan(0).WithMessage("Product ID must be greater than 0");

                item.RuleFor(i => i.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0");
            });
        }
    }

    public class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
    {
        private static readonly string[] ValidStatuses = { "Confirmed", "Shipping", "Delivered", "Completed", "Cancelled" };

        public UpdateOrderStatusRequestValidator()
        {
            RuleFor(x => x.Status)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Status is required")
                .Must(s => ValidStatuses.Contains(s)).WithMessage($"Status must be one of: {string.Join(", ", ValidStatuses)}");

            RuleFor(x => x.DeliveryImageUrl)
                .NotEmpty().WithMessage("Delivery image is required when marking as Delivered")
                .When(x => x.Status == "Delivered");
        }
    }
}
