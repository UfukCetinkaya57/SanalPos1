using FluentValidation;

namespace SanalPos.Application.Features.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerName)
                .NotEmpty().WithMessage("Müşteri adı zorunludur.")
                .MaximumLength(100).WithMessage("Müşteri adı en fazla 100 karakter olmalıdır.");

            RuleFor(x => x.CustomerEmail)
                .NotEmpty().WithMessage("Müşteri e-posta adresi zorunludur.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MaximumLength(100).WithMessage("E-posta adresi en fazla 100 karakter olmalıdır.");

            RuleFor(x => x.CustomerPhone)
                .MaximumLength(20).WithMessage("Telefon numarası en fazla 20 karakter olmalıdır.");

            RuleFor(x => x.ShippingAddress)
                .NotEmpty().WithMessage("Teslimat adresi zorunludur.")
                .MaximumLength(500).WithMessage("Teslimat adresi en fazla 500 karakter olmalıdır.");

            RuleFor(x => x.BillingAddress)
                .NotEmpty().WithMessage("Fatura adresi zorunludur.")
                .MaximumLength(500).WithMessage("Fatura adresi en fazla 500 karakter olmalıdır.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Notlar en fazla 500 karakter olmalıdır.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Sipariş en az bir ürün içermelidir.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.ProductId)
                    .NotEmpty().WithMessage("Ürün ID'si zorunludur.");

                item.RuleFor(x => x.Quantity)
                    .GreaterThan(0).WithMessage("Miktar sıfırdan büyük olmalıdır.");
            });
        }
    }
} 