using Faturamento.Application.DTOs;
using Faturamento.Application.UseCases.NotasFiscais.Commands;
using Faturamento.Application.UseCases.NotasFiscais.Queries;
using Faturamento.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Faturamento.API.Controllers;

[ApiController]
[Route("api/v1/notas-fiscais")]
public class NotasFiscaisController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<NotasFiscaisController> _logger;

    public NotasFiscaisController(IMediator mediator, ILogger<NotasFiscaisController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotaFiscalResponse>>> ListarTodos()
    {
        var query = new ListarNotasFiscaisQuery();
        var notas = await _mediator.Send(query);
        return Ok(notas);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<NotaFiscalResponse>> ObterPorId(Guid id)
    {
        var query = new ObterNotaFiscalQuery { Id = id };
        try
        {
            var nota = await _mediator.Send(query);
            return Ok(nota);
        }
        catch (DomainException ex)
        {
            return NotFound(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<NotaFiscalResponse>> Criar([FromBody] CriarNotaFiscalRequest request)
    {
        var command = new CriarNotaFiscalCommand
        {
            Itens = request.Itens
        };

        try
        {
            var nota = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterPorId), new { id = nota.Id }, nota);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpPost("{id:guid}/imprimir")]
    public async Task<ActionResult<ImprimirNotaFiscalResponse>> Imprimir(Guid id)
    {
        var command = new ImprimirNotaFiscalCommand { Id = id };

        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (DomainException ex)
        {
            return NotFound(new { sucesso = false, mensagem = ex.Message });
        }
    }
}

