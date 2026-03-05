using CandidatoLar.Application.DTOs.Pessoas;
using MediatR;

namespace CandidatoLar.Application.Commands.Pessoa;

public sealed record UpdatePessoaCommand(
    Guid Id,
    string Nome,
    DateTime DataNascimento
) : IRequest<PessoaResponse>;
