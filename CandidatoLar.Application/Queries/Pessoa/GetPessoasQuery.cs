using CandidatoLar.Application.Common;
using CandidatoLar.Application.DTOs.Pessoas;
using MediatR;

namespace CandidatoLar.Application.Queries.Pessoa;

public sealed record GetPessoasQuery(
    string? Nome,
    string? Cpf,
    bool? Ativo,
    int Page = 1,
    int PageSize = 20
) : IRequest<PagedResult<PessoaResponse>>;
