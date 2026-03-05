using CandidatoLar.Application.Interfaces;
using CandidatoLar.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Commands.Pessoa;

public sealed class DeletePessoaCommandHandler : IRequestHandler<DeletePessoaCommand>
{
    private readonly IPessoaRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<DeletePessoaCommandHandler> _logger;

    public DeletePessoaCommandHandler(IPessoaRepository repo, IUnitOfWork uow, ILogger<DeletePessoaCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(DeletePessoaCommand request, CancellationToken cancellationToken)
    {
        var pessoa = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Pessoa), request.Id);

        _repo.Remove(pessoa);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Pessoa {Id} deleted (hard delete)", request.Id);
    }
}
