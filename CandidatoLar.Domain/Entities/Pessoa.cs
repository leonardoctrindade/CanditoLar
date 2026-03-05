using CandidatoLar.Domain.Enums;
using CandidatoLar.Domain.Exceptions;
using CandidatoLar.Domain.ValueObjects;

namespace CandidatoLar.Domain.Entities;

public sealed class Pessoa
{
    private readonly List<Telefone> _telefones = [];


    public Guid Id { get; private set; }


    public string Nome { get; private set; } = string.Empty;
    public Cpf Cpf { get; private set; } = null!;
    public DateTime DataNascimento { get; private set; }
    public bool Ativo { get; private set; }
    public IReadOnlyCollection<Telefone> Telefones => _telefones.AsReadOnly();


    private Pessoa() { } // EF Core

    public static Pessoa Create(string nome, Cpf cpf, DateTime dataNascimento)
    {
        var pessoa = new Pessoa
        {
            Id = Guid.NewGuid(),
            Ativo = true
        };

        pessoa.AtualizarNome(nome);
        pessoa.AtualizarDataNascimento(dataNascimento);
        pessoa.Cpf = cpf;

        return pessoa;
    }


    public void AtualizarNome(string nome)
    {
        if (string.IsNullOrWhiteSpace(nome) || nome.Length is < 2 or > 120)
            throw new DomainException("Nome must be between 2 and 120 characters.");
        Nome = nome.Trim();
    }

    public void AtualizarDataNascimento(DateTime data)
    {
        if (data.Date > DateTime.UtcNow.Date)
            throw new DomainException("DataNascimento cannot be a future date.");

        if (data.Year < 1900)
            throw new DomainException("DataNascimento is not plausible (before 1900).");

        DataNascimento = data.Date;
    }

    public void Ativar() => Ativo = true;
    public void Desativar() => Ativo = false;

    public Telefone AdicionarTelefone(TipoTelefone tipo, string numero)
    {
        var normalized = Telefone.Normalize(numero);

        var exists = _telefones.Any(t =>
            t.Tipo == tipo &&
            t.Numero == normalized);

        if (exists)
            throw new DuplicatePhoneException(normalized);

        var telefone = new Telefone(Id, tipo, numero);
        _telefones.Add(telefone);
        return telefone;
    }

    public void RemoverTelefone(Guid telefoneId)
    {
        var telefone = _telefones.FirstOrDefault(t => t.Id == telefoneId)
            ?? throw new EntityNotFoundException(nameof(Telefone), telefoneId);

        _telefones.Remove(telefone);
    }

    public Telefone AtualizarTelefone(Guid telefoneId, TipoTelefone tipo, string numero)
    {
        var telefone = _telefones.FirstOrDefault(t => t.Id == telefoneId)
            ?? throw new EntityNotFoundException(nameof(Telefone), telefoneId);

        var normalized = Telefone.Normalize(numero);

        var duplicate = _telefones.Any(t =>
            t.Id != telefoneId &&
            t.Tipo == tipo &&
            t.Numero == normalized);

        if (duplicate)
            throw new DuplicatePhoneException(normalized);

        telefone.Update(tipo, numero);
        return telefone;
    }
}
