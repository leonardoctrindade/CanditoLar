using System.Text.RegularExpressions;
using CandidatoLar.Application.Commands.Telefone;
using FluentValidation;

namespace CandidatoLar.Application.Validators.Telefones;

public sealed class UpdateTelefoneCommandValidator : AbstractValidator<UpdateTelefoneCommand>
{
    private static readonly Regex DigitsOnly = new(@"^\d{8,13}$", RegexOptions.Compiled);

    public UpdateTelefoneCommandValidator()
    {
        RuleFor(x => x.Tipo)
            .IsInEnum().WithMessage("Tipo de telefone inválido.");

        RuleFor(x => x.Numero)
            .NotEmpty().WithMessage("Numero is required.")
            .Must(n => DigitsOnly.IsMatch(Regex.Replace(n ?? string.Empty, @"\D", string.Empty)))
                .WithMessage("Numero must contain only digits and have between 8 and 13 digits.");
    }
}
