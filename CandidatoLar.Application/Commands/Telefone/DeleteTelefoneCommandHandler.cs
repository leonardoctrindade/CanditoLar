using CandidatoLar.Application.Interfaces;
using CandidatoLar.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Commands.Telefone;

public sealed class DeleteTelefoneCommandHandler : IRequestHandler<DeleteTelefoneCommand>
{
    private readonly IPessoaRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<DeleteTelefoneCommandHandler> _logger;

    public DeleteTelefoneCommandHandler(IPessoaRepository repo, IUnitOfWork uow, ILogger<DeleteTelefoneCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(DeleteTelefoneCommand request, CancellationToken cancellationToken)
    {
        var pessoa = await _repo.GetByIdWithTelesAsync(request.PessoaId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Pessoa), request.PessoaId);

        pessoa.RemoverTelefone(request.TelefoneId);

        _repo.Update(pessoa);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Telefone {TelId} removed from Pessoa {PessoaId}", request.TelefoneId, request.PessoaId);
    }
}
