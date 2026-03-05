using CandidatoLar.Application.Interfaces;
using CandidatoLar.Application.DTOs.Pessoas;
using CandidatoLar.Application.Mappings;
using CandidatoLar.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Queries.Pessoa;

public sealed class GetPessoaByIdQueryHandler : IRequestHandler<GetPessoaByIdQuery, PessoaResponse>
{
    private readonly IPessoaRepository _repo;
    private readonly ILogger<GetPessoaByIdQueryHandler> _logger;

    public GetPessoaByIdQueryHandler(IPessoaRepository repo, ILogger<GetPessoaByIdQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<PessoaResponse> Handle(GetPessoaByIdQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching Pessoa {Id}", request.Id);

        var pessoa = await _repo.GetByIdWithTelesAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Pessoa), request.Id);

        return pessoa.ToResponse();
    }
}
