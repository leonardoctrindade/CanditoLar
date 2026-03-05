namespace CandidatoLar.Application.DTOs.Pessoas;

public sealed record CreatePessoaRequest(
    string Nome,
    string Cpf,
    DateTime DataNascimento
);
