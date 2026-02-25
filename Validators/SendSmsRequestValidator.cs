using FluentValidation;
using SimpleApi.Models;

namespace SimpleApi.Validators;

public class SendSmsRequestValidator : AbstractValidator<SendSmsRequest>
{
    public SendSmsRequestValidator()
    {
        RuleFor(x => x.Phone)
            .NotEmpty().WithMessage("Telefon numarası zorunludur.")
            .MinimumLength(10).WithMessage("Telefon en az 10 karakter olmalı.");

        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Mesaj zorunludur.")
            .MinimumLength(3).WithMessage("Mesaj en az 3 karakter olmalı.");
    }
}
