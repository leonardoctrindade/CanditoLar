namespace CandidatoLar.Application.DTOs.Pessoas;

public sealed record UpdatePessoaRequest(
    string Nome,
    DateTime DataNascimento
);
