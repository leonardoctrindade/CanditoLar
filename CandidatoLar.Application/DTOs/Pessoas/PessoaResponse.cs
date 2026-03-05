using CandidatoLar.Application.DTOs.Telefones;

namespace CandidatoLar.Application.DTOs.Pessoas;

public sealed record PessoaResponse(
    Guid Id,
    string Nome,
    string Cpf,
    string CpfFormatado,
    DateTime DataNascimento,
    bool Ativo,
    IReadOnlyList<TelefoneResponse> Telefones
);
