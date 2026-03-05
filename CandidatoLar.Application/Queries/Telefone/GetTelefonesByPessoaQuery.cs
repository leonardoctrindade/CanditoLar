using CandidatoLar.Application.DTOs.Telefones;
using MediatR;

namespace CandidatoLar.Application.Queries.Telefone;

public sealed record GetTelefonesByPessoaQuery(Guid PessoaId) : IRequest<IReadOnlyList<TelefoneResponse>>;
