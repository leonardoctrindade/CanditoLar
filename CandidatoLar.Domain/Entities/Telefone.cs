using System.Text.RegularExpressions;
using CandidatoLar.Domain.Enums;
using CandidatoLar.Domain.Exceptions;

namespace CandidatoLar.Domain.Entities;

public sealed class Telefone
{
    private static readonly Regex DigitsOnly = new(@"\D", RegexOptions.Compiled);
    private const int MinLength = 8;
    private const int MaxLength = 13;

    public Guid Id { get; private set; }
    public Guid PessoaId { get; private set; }
    public TipoTelefone Tipo { get; private set; }
    public string Numero { get; private set; } = string.Empty;


    private Telefone() { } // EF Core

    internal Telefone(Guid pessoaId, TipoTelefone tipo, string numero)
    {
        Id = Guid.NewGuid();
        PessoaId = pessoaId;
        Tipo = tipo;
        Numero = Normalize(numero);
        ValidateNumero(Numero);
    }


    internal void Update(TipoTelefone tipo, string numero)
    {
        var normalized = Normalize(numero);
        ValidateNumero(normalized);
        Tipo = tipo;
        Numero = normalized;
    }


    public static string Normalize(string numero) =>
        DigitsOnly.Replace(numero ?? string.Empty, string.Empty);

    private static void ValidateNumero(string digits)
    {
        if (digits.Length is < MinLength or > MaxLength)
            throw new DomainException(
                $"Phone number must have between {MinLength} and {MaxLength} digits. Got '{digits}' ({digits.Length} digits).");
    }
}
