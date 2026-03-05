using Asp.Versioning;
using CandidatoLar.Application.Common;
using CandidatoLar.Application.DTOs.Pessoas;
using CandidatoLar.Application.Commands.Pessoa;
using CandidatoLar.Application.Queries.Pessoa;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CandidatoLar.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/pessoas")]
[Produces("application/json")]
public sealed class PessoasController : ControllerBase
{
    private readonly IMediator _mediator;

    public PessoasController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Cria uma nova pessoa no sistema.
    /// </summary>
    /// <param name="request">Dados da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Pessoa criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create(
        [FromBody] CreatePessoaRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreatePessoaCommand(request.Nome, request.Cpf, request.DataNascimento);
        var result = await _mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Lista pessoas paginadas filtrando por nome, cpf e status ativo.
    /// </summary>
    /// <param name="nome">Nome para filtro opcional.</param>
    /// <param name="cpf">Cpf para filtro opcional.</param>
    /// <param name="ativo">Status para filtro opcional.</param>
    /// <param name="page">Número da página.</param>
    /// <param name="pageSize">Tamanho da página.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista paginada de pessoas.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PessoaResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(
        [FromQuery] string? nome,
        [FromQuery] string? cpf,
        [FromQuery] bool? ativo,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(
            new GetPessoasQuery(nome, cpf, ativo, page, pageSize), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Retorna os detalhes de uma pessoa pelo Id.
    /// </summary>
    /// <param name="id">Id da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Dados da pessoa.</returns>
    [HttpGet("{id:guid}", Name = nameof(GetById))]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetPessoaByIdQuery(id), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Atualiza dados de nome e data de nascimento de uma pessoa.
    /// </summary>
    /// <param name="id">Id da pessoa.</param>
    /// <param name="request">Dados de atualização.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Pessoa atualizada.</returns>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(PessoaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdatePessoaRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdatePessoaCommand(id, request.Nome, request.DataNascimento), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Altera o status (Ativo/Inativo) de uma pessoa no sistema.
    /// </summary>
    /// <param name="id">Id da pessoa.</param>
    /// <param name="request">Comando contento novo status.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Sucesso sem conteúdo adicional ou falha se não encontrada.</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(
        Guid id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new ChangePessoaStatusCommand(id, request.Ativo), cancellationToken);
        return Ok(new { message = $"Status atualizado com sucesso." });
    }

    /// <summary>
    /// Remove permanentemente uma pessoa do banco de dados (Hard delete).
    /// </summary>
    /// <param name="id">Id da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Sem resposta no sucesso.</returns>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeletePessoaCommand(id), cancellationToken);
        return NoContent();
    }
}

public sealed record UpdateStatusRequest(bool Ativo);
