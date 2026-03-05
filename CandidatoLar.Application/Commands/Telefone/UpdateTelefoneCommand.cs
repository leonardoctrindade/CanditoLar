using CandidatoLar.Application.DTOs.Telefones;
using CandidatoLar.Domain.Enums;
using MediatR;

namespace CandidatoLar.Application.Commands.Telefone;

public sealed record UpdateTelefoneCommand(
    Guid PessoaId,
    Guid TelefoneId,
    TipoTelefone Tipo,
    string Numero
) : IRequest<TelefoneResponse>;
