using CandidatoLar.Application.DTOs.Pessoas;
using CandidatoLar.Application.DTOs.Telefones;
using CandidatoLar.Domain.Entities;

namespace CandidatoLar.Application.Mappings;

public static class PessoaMappings
{
    public static PessoaResponse ToResponse(this Pessoa p) =>
        new(
            p.Id,
            p.Nome,
            p.Cpf.Value,
            p.Cpf.ToFormatted(),
            p.DataNascimento,
            p.Ativo,
            p.Telefones.Select(t => t.ToResponse()).ToList()
        );

    public static TelefoneResponse ToResponse(this Telefone t) =>
        new(t.Id, t.PessoaId, t.Tipo, t.Numero);
}
