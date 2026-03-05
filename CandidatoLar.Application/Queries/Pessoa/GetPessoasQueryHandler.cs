using CandidatoLar.Application.Common;
using CandidatoLar.Application.Interfaces;
using CandidatoLar.Application.DTOs.Pessoas;
using CandidatoLar.Application.Mappings;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Queries.Pessoa;

public sealed class GetPessoasQueryHandler
    : IRequestHandler<GetPessoasQuery, PagedResult<PessoaResponse>>
{
    private readonly IPessoaRepository _repo;
    private readonly ILogger<GetPessoasQueryHandler> _logger;

    public GetPessoasQueryHandler(IPessoaRepository repo, ILogger<GetPessoasQueryHandler> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<PagedResult<PessoaResponse>> Handle(
        GetPessoasQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Listing pessoas. Filters: Nome={Nome} CPF={Cpf} Ativo={Ativo} Page={Page}/{PageSize}",
            request.Nome, request.Cpf, request.Ativo, request.Page, request.PageSize);

        var (items, total) = await _repo.ListAsync(
            request.Nome, request.Cpf, request.Ativo,
            request.Page, request.PageSize, cancellationToken);

        return new PagedResult<PessoaResponse>
        {
            Items = items.Select(p => p.ToResponse()),
            TotalCount = total,
            Page = request.Page,
            PageSize = request.PageSize
        };
    }
}
