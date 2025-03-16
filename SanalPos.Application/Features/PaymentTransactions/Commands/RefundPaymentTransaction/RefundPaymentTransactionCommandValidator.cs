using FluentValidation;

namespace SanalPos.Application.Features.PaymentTransactions.Commands.RefundPaymentTransaction
{
    public class RefundPaymentTransactionCommandValidator : AbstractValidator<RefundPaymentTransactionCommand>
    {
        public RefundPaymentTransactionCommandValidator()
        {
            RuleFor(p => p.TransactionId)
                .NotEmpty().WithMessage("İşlem ID'si boş olamaz.");

            RuleFor(p => p.RefundAmount)
                .NotEmpty().WithMessage("İade tutarı boş olamaz.")
                .GreaterThan(0).WithMessage("İade tutarı sıfırdan büyük olmalıdır.");

            RuleFor(p => p.RefundReason)
                .MaximumLength(250).WithMessage("İade nedeni 250 karakterden uzun olamaz.");
        }
    }
} 