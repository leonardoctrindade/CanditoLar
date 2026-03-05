namespace CandidatoLar.Domain.Exceptions;

public sealed class InvalidCpfException : DomainException
{
    public InvalidCpfException(string cpf)
        : base($"The CPF '{cpf}' is invalid.") { }
}
