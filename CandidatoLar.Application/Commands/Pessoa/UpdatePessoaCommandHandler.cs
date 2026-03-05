using CandidatoLar.Application.Interfaces;
using CandidatoLar.Application.DTOs.Pessoas;
using CandidatoLar.Application.Mappings;
using CandidatoLar.Domain.Exceptions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CandidatoLar.Application.Commands.Pessoa;

public sealed class UpdatePessoaCommandHandler : IRequestHandler<UpdatePessoaCommand, PessoaResponse>
{
    private readonly IPessoaRepository _repo;
    private readonly IUnitOfWork _uow;
    private readonly ILogger<UpdatePessoaCommandHandler> _logger;

    public UpdatePessoaCommandHandler(IPessoaRepository repo, IUnitOfWork uow, ILogger<UpdatePessoaCommandHandler> logger)
    {
        _repo = repo;
        _uow = uow;
        _logger = logger;
    }

    public async Task<PessoaResponse> Handle(UpdatePessoaCommand request, CancellationToken cancellationToken)
    {
        var pessoa = await _repo.GetByIdWithTelesAsync(request.Id, cancellationToken)
            ?? throw new EntityNotFoundException(nameof(Domain.Entities.Pessoa), request.Id);

        pessoa.AtualizarNome(request.Nome);
        pessoa.AtualizarDataNascimento(request.DataNascimento);

        _repo.Update(pessoa);
        await _uow.CommitAsync(cancellationToken);

        _logger.LogInformation("Pessoa {Id} updated", request.Id);

        return pessoa.ToResponse();
    }
}
