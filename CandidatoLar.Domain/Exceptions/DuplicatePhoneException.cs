namespace CandidatoLar.Domain.Exceptions;

public sealed class DuplicatePhoneException : DomainException
{
    public DuplicatePhoneException(string numero)
        : base($"The phone number '{numero}' already exists for this person.") { }
}
