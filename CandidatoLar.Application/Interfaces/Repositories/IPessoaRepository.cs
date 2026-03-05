using CandidatoLar.Domain.Entities;

namespace CandidatoLar.Application.Interfaces;

public interface IPessoaRepository
{
    Task<Pessoa?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Pessoa?> GetByIdWithTelesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCpfAsync(string cpf, Guid? excludeId = null, CancellationToken cancellationToken = default);
    Task<(IEnumerable<Pessoa> Items, int TotalCount)> ListAsync(
        string? nome, string? cpf, bool? ativo, int page, int pageSize,
        CancellationToken cancellationToken = default);
    Task AddAsync(Pessoa pessoa, CancellationToken cancellationToken = default);
    void Update(Pessoa pessoa);
    void Remove(Pessoa pessoa);
}
