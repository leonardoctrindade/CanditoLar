namespace CandidatoLar.Domain.Exceptions;

public sealed class EntityNotFoundException : DomainException
{
    public EntityNotFoundException(string entity, object id)
        : base($"{entity} with id '{id}' was not found.") { }
}
