using CandidatoLar.Application.DTOs.Pessoas;
using MediatR;

namespace CandidatoLar.Application.Commands.Pessoa;

public sealed record CreatePessoaCommand(
    string Nome,
    string Cpf,
    DateTime DataNascimento
) : IRequest<PessoaResponse>;
