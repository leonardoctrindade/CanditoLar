using CandidatoLar.Application.Interfaces;
using CandidatoLar.Application.DTOs.Telefones;
using CandidatoLar.Application.Mappings;
using CandidatoLar.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Commands.Telefone;

public sealed class AddTelefoneCommandHandler : IRequestHandler<AddTelefoneCommand, TelefoneResponse>
{
    private readonly IPessoaRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<AddTelefoneCommandHandler> _logger;

    public AddTelefoneCommandHandler(IPessoaRepository repo, IUnitOfWork uow, ILogger<AddTelefoneCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<TelefoneResponse> Handle(AddTelefoneCommand request, CancellationToken cancellationToken)
    {
        var pessoa = await _repo.GetByIdWithTelesAsync(request.PessoaId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Pessoa), request.PessoaId);

        var telefone = pessoa.AdicionarTelefone(request.Tipo, request.Numero);

        _repo.Update(pessoa);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Telefone {TelId} added to Pessoa {PessoaId}", telefone.Id, request.PessoaId);

        return telefone.ToResponse();
    }
}
