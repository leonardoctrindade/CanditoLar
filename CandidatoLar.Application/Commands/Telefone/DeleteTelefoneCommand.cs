using MediatR;

namespace CandidatoLar.Application.Commands.Telefone;

public sealed record DeleteTelefoneCommand(Guid PessoaId, Guid TelefoneId) : IRequest;
