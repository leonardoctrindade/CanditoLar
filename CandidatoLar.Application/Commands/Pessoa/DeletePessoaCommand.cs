using MediatR;

namespace CandidatoLar.Application.Commands.Pessoa;

public sealed record DeletePessoaCommand(Guid Id) : IRequest;
