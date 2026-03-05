using CandidatoLar.Application.Interfaces;
using CandidatoLar.Application.DTOs.Telefones;
using CandidatoLar.Application.Mappings;
using CandidatoLar.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Queries.Telefone;

public sealed class GetTelefonesByPessoaQueryHandler
    : IRequestHandler<GetTelefonesByPessoaQuery, IReadOnlyList<TelefoneResponse>>
{
    private readonly IPessoaRepository _repo;
    private readonly ILogger<GetTelefonesByPessoaQueryHandler> _logger;

    public GetTelefonesByPessoaQueryHandler(IPessoaRepository repo, ILogger<GetTelefonesByPessoaQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<IReadOnlyList<TelefoneResponse>> Handle(
        GetTelefonesByPessoaQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching telefones for Pessoa {PessoaId}", request.PessoaId);

        var pessoa = await _repo.GetByIdWithTelesAsync(request.PessoaId, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Pessoa), request.PessoaId);

        return pessoa.Telefones.Select(t => t.ToResponse()).ToList();
    }
}
