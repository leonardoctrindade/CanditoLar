using CandidatoLar.Domain.Enums;

namespace CandidatoLar.Application.DTOs.Telefones;

public sealed record TelefoneResponse(
    Guid Id,
    Guid PessoaId,
    TipoTelefone Tipo,
    string Numero
);
