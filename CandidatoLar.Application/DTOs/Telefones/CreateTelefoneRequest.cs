using CandidatoLar.Domain.Enums;

namespace CandidatoLar.Application.DTOs.Telefones;

public sealed record CreateTelefoneRequest(
    TipoTelefone Tipo,
    string Numero
);
