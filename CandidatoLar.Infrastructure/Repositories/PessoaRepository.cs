using CandidatoLar.Application.Interfaces;
using CandidatoLar.Domain.Entities;
using CandidatoLar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CandidatoLar.Infrastructure.Repositories;

public sealed class PessoaRepository : IPessoaRepository
{
    private readonly AppDbContext _db;

    public PessoaRepository(AppDbContext db) => _db = db;

    public async Task<Pessoa?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _db.Pessoas.FindAsync([id], cancellationToken);

    public async Task<Pessoa?> GetByIdWithTelesAsync(Guid id, CancellationToken cancellationToken = default) =>
        await _db.Pessoas
            .Include(p => p.Telefones)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public async Task<bool> ExistsByCpfAsync(
        string cpf, Guid? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _db.Pessoas.Where(p => p.Cpf.Value == cpf);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);
        return await query.AnyAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Pessoa> Items, int TotalCount)> ListAsync(
        string? nome, string? cpf, bool? ativo, int page, int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Pessoas.Include(p => p.Telefones).AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
            query = query.Where(p => p.Nome.Contains(nome));

        if (!string.IsNullOrWhiteSpace(cpf))
        {
            var normalised = System.Text.RegularExpressions.Regex.Replace(cpf, @"\D", string.Empty);
            query = query.Where(p => p.Cpf.Value == normalised);
        }

        if (ativo.HasValue)
            query = query.Where(p => p.Ativo == ativo.Value);

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(p => p.Nome)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public async Task AddAsync(Pessoa pessoa, CancellationToken cancellationToken = default) =>
        await _db.Pessoas.AddAsync(pessoa, cancellationToken);

    public void Update(Pessoa pessoa) => _db.Pessoas.Update(pessoa);

    public void Remove(Pessoa pessoa) => _db.Pessoas.Remove(pessoa);
}
