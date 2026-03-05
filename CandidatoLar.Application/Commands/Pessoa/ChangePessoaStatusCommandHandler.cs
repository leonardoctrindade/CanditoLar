using CandidatoLar.Application.Interfaces;
using CandidatoLar.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Commands.Pessoa;

public sealed class ChangePessoaStatusCommandHandler : IRequestHandler<ChangePessoaStatusCommand>
{
    private readonly IPessoaRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<ChangePessoaStatusCommandHandler> _logger;

    public ChangePessoaStatusCommandHandler(IPessoaRepository repo, IUnitOfWork uow, ILogger<ChangePessoaStatusCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task Handle(ChangePessoaStatusCommand request, CancellationToken cancellationToken)
    {
        var pessoa = await _repo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Pessoa), request.Id);

        if (request.Ativo)
            pessoa.Ativar();
        else
            pessoa.Desativar();

        _repo.Update(pessoa);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Pessoa {Id} status changed to Ativo={Ativo}", request.Id, request.Ativo);
    }
}
