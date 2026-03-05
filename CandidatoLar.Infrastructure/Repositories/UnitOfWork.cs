using CandidatoLar.Application.Interfaces;
using CandidatoLar.Infrastructure.Persistence;

namespace CandidatoLar.Infrastructure.Repositories;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public UnitOfWork(AppDbContext db) => _db = db;

    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
