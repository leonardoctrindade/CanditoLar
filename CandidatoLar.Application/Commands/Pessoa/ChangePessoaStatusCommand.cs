using MediatR;

namespace CandidatoLar.Application.Commands.Pessoa;

public sealed record ChangePessoaStatusCommand(Guid Id, bool Ativo) : IRequest;
