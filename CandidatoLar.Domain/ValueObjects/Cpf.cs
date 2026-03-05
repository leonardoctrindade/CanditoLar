using System.Text.RegularExpressions;
using CandidatoLar.Domain.Exceptions;

namespace CandidatoLar.Domain.ValueObjects;

public sealed class Cpf : IEquatable<Cpf>
{
    private static readonly Regex DigitsOnly = new(@"\D", RegexOptions.Compiled);

    public string Value { get; }

    public Cpf(string rawValue)
    {
        var digits = Normalize(rawValue);
        if (!IsValid(digits))
            throw new InvalidCpfException(rawValue);

        Value = digits;
    }

    private Cpf() => Value = string.Empty;


    private static string Normalize(string value) =>
        DigitsOnly.Replace(value ?? string.Empty, string.Empty);

    public static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf)) return false;

        var digits = DigitsOnly.Replace(cpf, string.Empty);

        if (digits.Length != 11) return false;
        if (digits.Distinct().Count() == 1) return false; // all same digit

        return CheckDigit(digits, 9) && CheckDigit(digits, 10);
    }

    private static bool CheckDigit(string digits, int position)
    {
        var sum = 0;
        for (var i = 0; i < position; i++)
            sum += (digits[i] - '0') * (position + 1 - i);

        var remainder = (sum * 10) % 11;
        if (remainder == 10) remainder = 0;

        return remainder == (digits[position] - '0');
    }


    public override string ToString() => Value;
    public override int GetHashCode() => Value.GetHashCode();
    public override bool Equals(object? obj) => obj is Cpf other && Equals(other);
    public bool Equals(Cpf? other) => other is not null && Value == other.Value;

    public static bool operator ==(Cpf? left, Cpf? right) => left?.Equals(right) ?? right is null;
    public static bool operator !=(Cpf? left, Cpf? right) => !(left == right);

    public string ToFormatted() =>
        $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}";
}
