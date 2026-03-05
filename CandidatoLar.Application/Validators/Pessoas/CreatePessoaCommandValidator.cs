using CandidatoLar.Application.Commands.Pessoa;
using CandidatoLar.Domain.ValueObjects;
using FluentValidation;

namespace CandidatoLar.Application.Validators.Pessoas;

public sealed class CreatePessoaCommandValidator : AbstractValidator<CreatePessoaCommand>
{
    public CreatePessoaCommandValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty().WithMessage("Nome is required.")
            .Length(2, 120).WithMessage("Nome must be between 2 and 120 characters.");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("CPF is required.")
            .Must(cpf => Cpf.IsValid(cpf)).WithMessage("CPF is invalid.");

        RuleFor(x => x.DataNascimento)
            .LessThanOrEqualTo(_ => DateTime.UtcNow.Date)
                .WithMessage("DataNascimento cannot be a future date.")
            .GreaterThanOrEqualTo(new DateTime(1900, 1, 1))
                .WithMessage("DataNascimento is not plausible (before 1900).");
    }
}
