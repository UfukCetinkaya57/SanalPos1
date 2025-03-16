using FluentValidation;
using System;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Features.PaymentTransactions.Commands.CreatePaymentTransaction
{
    public class CreatePaymentTransactionCommandValidator : AbstractValidator<CreatePaymentTransactionCommand>
    {
        public CreatePaymentTransactionCommandValidator()
        {
            RuleFor(p => p.OrderId)
                .NotEmpty().WithMessage("Sipariş ID'si boş olamaz.");

            RuleFor(p => p.Amount)
                .NotEmpty().WithMessage("Tutar boş olamaz.")
                .GreaterThan(0).WithMessage("Tutar sıfırdan büyük olmalıdır.");

            RuleFor(p => p.PaymentMethod)
                .NotEmpty().WithMessage("Ödeme yöntemi boş olamaz.")
                .IsInEnum().WithMessage("Geçerli bir ödeme yöntemi seçiniz.");

            RuleFor(p => p.CardNumber)
                .NotEmpty().WithMessage("Kart numarası boş olamaz.")
                .CreditCard().WithMessage("Geçerli bir kredi kartı numarası giriniz.")
                .When(p => p.PaymentMethod == PaymentMethod.CreditCard || p.PaymentMethod == PaymentMethod.DebitCard);

            RuleFor(p => p.CardHolderName)
                .NotEmpty().WithMessage("Kart sahibi adı boş olamaz.")
                .MaximumLength(100).WithMessage("Kart sahibi adı 100 karakterden uzun olamaz.")
                .When(p => p.PaymentMethod == PaymentMethod.CreditCard || p.PaymentMethod == PaymentMethod.DebitCard);

            RuleFor(p => p.CardExpiryMonth)
                .NotEmpty().WithMessage("Kart son kullanma ayı boş olamaz.")
                .Must(BeValidMonth).WithMessage("Geçerli bir ay giriniz (01-12).")
                .When(p => p.PaymentMethod == PaymentMethod.CreditCard || p.PaymentMethod == PaymentMethod.DebitCard);

            RuleFor(p => p.CardExpiryYear)
                .NotEmpty().WithMessage("Kart son kullanma yılı boş olamaz.")
                .Must(BeValidYear).WithMessage("Geçerli bir yıl giriniz.")
                .When(p => p.PaymentMethod == PaymentMethod.CreditCard || p.PaymentMethod == PaymentMethod.DebitCard);

            RuleFor(p => p.CardCvv)
                .NotEmpty().WithMessage("CVV boş olamaz.")
                .Length(3, 4).WithMessage("CVV 3 veya 4 karakter olmalıdır.")
                .Matches("^[0-9]*$").WithMessage("CVV sadece rakamlardan oluşmalıdır.")
                .When(p => p.PaymentMethod == PaymentMethod.CreditCard || p.PaymentMethod == PaymentMethod.DebitCard);
        }

        private bool BeValidMonth(string month)
        {
            if (int.TryParse(month, out int monthValue))
            {
                return monthValue >= 1 && monthValue <= 12;
            }
            return false;
        }

        private bool BeValidYear(string year)
        {
            if (int.TryParse(year, out int yearValue))
            {
                int currentYear = DateTime.Now.Year % 100;
                return yearValue >= currentYear && yearValue <= currentYear + 20;
            }
            return false;
        }
    }
} 