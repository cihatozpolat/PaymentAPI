using FluentValidation;
using PaymentAPI.Entities;
using PaymentAPI.Requests;

namespace PaymentAPI.Validators
{
    public class paymentValidation : AbstractValidator<PaymentRequest>
    {
        public paymentValidation()
        {
            CascadeMode = CascadeMode.Continue;

            RuleFor(x => x.Name)
            .Cascade(CascadeMode.Continue)
            .NotEmpty().WithMessage("Name boş olamaz!")
            .MaximumLength(20).WithMessage("Name 20 karakterden uzun olamaz!");

            RuleFor(x => x.Balance)
            .NotEqual(0).WithMessage("Balance boş olamaz!")
            .GreaterThan(0).WithMessage("Balance 0'dan küçük olamaz");

            RuleFor(x => x.Currency)
            .NotEmpty().WithMessage("Currency boş olamaz!")
            .MaximumLength(10).WithMessage("Currency 10 karakterden uzun olamaz!");
        }
    }
}