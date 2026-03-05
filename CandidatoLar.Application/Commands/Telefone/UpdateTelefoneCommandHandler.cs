using CandidatoLar.Application.Interfaces;
using CandidatoLar.Application.DTOs.Telefones;
using CandidatoLar.Application.Mappings;
using CandidatoLar.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Commands.Telefone;

public sealed class UpdateTelefoneCommandHandler : IRequestHandler<UpdateTelefoneCommand, TelefoneResponse>
{
    private readonly IPessoaRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UpdateTelefoneCommandHandler> _logger;

    public UpdateTelefoneCommandHandler(IPessoaRepository repo, IUnitOfWork uow, ILogger<UpdateTelefoneCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<TelefoneResponse> Handle(UpdateTelefoneCommand request, CancellationToken cancellationToken)
    {
        var pessoa = await _repo.GetByIdWithTelesAsync(request.PessoaId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Pessoa), request.PessoaId);

        var telefone = pessoa.AtualizarTelefone(request.TelefoneId, request.Tipo, request.Numero);

        _repo.Update(pessoa);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Telefone {TelId} updated on Pessoa {PessoaId}", request.TelefoneId, request.PessoaId);

        return telefone.ToResponse();
    }
}
