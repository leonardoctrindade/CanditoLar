using CandidatoLar.Application.Interfaces;
using CandidatoLar.Application.DTOs.Pessoas;
using CandidatoLar.Application.Mappings;
using CandidatoLar.Domain.Entities;
using CandidatoLar.Domain.Exceptions;
using CandidatoLar.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Commands.Pessoa;

public sealed class CreatePessoaCommandHandler : IRequestHandler<CreatePessoaCommand, PessoaResponse>
{
    private readonly IPessoaRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<CreatePessoaCommandHandler> _logger;

    public CreatePessoaCommandHandler(
        IPessoaRepository repo,
        IUnitOfWork uow,
        ILogger<CreatePessoaCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<PessoaResponse> Handle(CreatePessoaCommand request, CancellationToken cancellationToken)
    {
        var cpf = new Cpf(request.Cpf); // validates and normalises

        if (await _repo.ExistsByCpfAsync(cpf.Value, cancellationToken: cancellationToken))
            throw new DuplicateCpfException(cpf.Value);

        var pessoa = Domain.Entities.Pessoa.Create(request.Nome, cpf, request.DataNascimento);

        await _repo.AddAsync(pessoa, cancellationToken);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Pessoa {Id} created with CPF {Cpf}", pessoa.Id, cpf.Value);

        return pessoa.ToResponse();
    }
}
