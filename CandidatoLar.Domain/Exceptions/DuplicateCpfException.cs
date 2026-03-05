namespace CandidatoLar.Domain.Exceptions;

public sealed class DuplicateCpfException : DomainException
{
    public DuplicateCpfException(string cpf)
        : base($"The CPF '{cpf}' is already registered.") { }
}
