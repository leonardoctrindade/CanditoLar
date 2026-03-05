using Asp.Versioning;
using CandidatoLar.Application.DTOs.Telefones;
using CandidatoLar.Application.Commands.Telefone;
using CandidatoLar.Application.Queries.Telefone;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CandidatoLar.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/pessoas/{pessoaId:guid}/telefones")]
[Produces("application/json")]
public sealed class TelefonesController : ControllerBase
{
    private readonly IMediator _mediator;

    public TelefonesController(IMediator mediator) => _mediator = mediator;

    /// <summary>
    /// Lista todos os telefones de uma determinada pessoa.
    /// </summary>
    /// <param name="pessoaId">Id da pessoa.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Lista de telefones da pessoa.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<TelefoneResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAll(Guid pessoaId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetTelefonesByPessoaQuery(pessoaId), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Adiciona um novo telefone a uma pessoa existente.
    /// </summary>
    /// <param name="pessoaId">Id da pessoa.</param>
    /// <param name="request">Dados do telefone.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Telefone criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TelefoneResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Add(
        Guid pessoaId,
        [FromBody] CreateTelefoneRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AddTelefoneCommand(pessoaId, request.Tipo, request.Numero), cancellationToken);

        return CreatedAtAction(nameof(GetAll), new { pessoaId }, result);
    }

    /// <summary>
    /// Atualiza os dados de um telefone pertencente a uma pessoa.
    /// </summary>
    /// <param name="pessoaId">Id da pessoa.</param>
    /// <param name="telefoneId">Id do telefone a ser atualizado.</param>
    /// <param name="request">Novos dados do telefone.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Telefone atualizado.</returns>
    [HttpPut("{telefoneId:guid}")]
    [ProducesResponseType(typeof(TelefoneResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Update(
        Guid pessoaId,
        Guid telefoneId,
        [FromBody] CreateTelefoneRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new UpdateTelefoneCommand(pessoaId, telefoneId, request.Tipo, request.Numero), cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Remove permanentemente um telefone de uma pessoa.
    /// </summary>
    /// <param name="pessoaId">Id da pessoa.</param>
    /// <param name="telefoneId">Id do telefone.</param>
    /// <param name="cancellationToken">Token de cancelamento.</param>
    /// <returns>Sem resposta no sucesso.</returns>
    [HttpDelete("{telefoneId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(
        Guid pessoaId,
        Guid telefoneId,
        CancellationToken cancellationToken)
    {
        await _mediator.Send(new DeleteTelefoneCommand(pessoaId, telefoneId), cancellationToken);
        return NoContent();
    }
}
