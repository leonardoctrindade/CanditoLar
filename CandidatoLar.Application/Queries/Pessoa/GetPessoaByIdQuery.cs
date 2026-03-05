using CandidatoLar.Application.DTOs.Pessoas;
using MediatR;

namespace CandidatoLar.Application.Queries.Pessoa;

public sealed record GetPessoaByIdQuery(Guid Id) : IRequest<PessoaResponse>;
