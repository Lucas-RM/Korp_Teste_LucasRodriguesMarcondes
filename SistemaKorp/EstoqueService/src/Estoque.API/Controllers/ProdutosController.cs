using Estoque.Application.DTOs;
using Estoque.Application.UseCases.Estoque.Commands;
using Estoque.Application.UseCases.Produtos.Commands;
using Estoque.Application.UseCases.Produtos.Queries;
using Estoque.Domain.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Estoque.API.Controllers;

[ApiController]
[Route("api/v1/produtos")]
public class ProdutosController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProdutosController> _logger;

    public ProdutosController(IMediator mediator, ILogger<ProdutosController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProdutoResponse>>> ListarTodos()
    {
        var query = new ListarProdutosQuery();
        var produtos = await _mediator.Send(query);
        return Ok(produtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProdutoResponse>> ObterPorId(Guid id)
    {
        var query = new ObterProdutoPorIdQuery { Id = id };
        try
        {
            var produto = await _mediator.Send(query);
            return Ok(produto);
        }
        catch (DomainException ex)
        {
            return NotFound(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpPost]
    public async Task<ActionResult<ProdutoResponse>> Criar([FromBody] CriarProdutoRequest request)
    {
        var command = new CriarProdutoCommand
        {
            Codigo = request.Codigo,
            Descricao = request.Descricao,
            Saldo = request.Saldo
        };

        try
        {
            var produto = await _mediator.Send(command);
            return CreatedAtAction(nameof(ObterPorId), new { id = produto.Id }, produto);
        }
        catch (DomainException ex)
        {
            return BadRequest(new { sucesso = false, mensagem = ex.Message });
        }
    }

    [HttpPost("verificar-existencia")]
    public async Task<ActionResult<VerificarProdutosResponse>> VerificarExistencia([FromBody] VerificarProdutosRequest request)
    {
        var query = new VerificarProdutosQuery
        {
            Codigos = request.Codigos
        };

        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpPut("{id:guid}")]
    public Task<ActionResult<ProdutoResponse>> Atualizar(Guid id, [FromBody] AtualizarProdutoRequest request)
    {
        // Implementar se necessário
        return Task.FromResult<ActionResult<ProdutoResponse>>(NotFound());
    }

    [HttpDelete("{id:guid}")]
    public Task<IActionResult> Remover(Guid id)
    {
        // Implementar se necessário
        return Task.FromResult<IActionResult>(NotFound());
    }

    [HttpPost("baixa-estoque")]
    public async Task<ActionResult<BaixaEstoqueResponse>> BaixaEstoque([FromBody] BaixaEstoqueRequest request)
    {
        var command = new RealizarBaixaEstoqueCommand
        {
            ItensBaixa = request.ItensBaixa
        };

        try
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (FalhaSimuladaException)
        {
            return StatusCode(503, new BaixaEstoqueResponse
            {
                Sucesso = false,
                Mensagem = "Serviço temporariamente indisponível. Tente novamente mais tarde."
            });
        }
        catch (DomainException ex)
        {
            return BadRequest(new BaixaEstoqueResponse
            {
                Sucesso = false,
                Mensagem = ex.Message
            });
        }
    }

    [HttpPatch("{id:guid}/simular-falha")]
    public async Task<ActionResult<ProdutoResponse>> AlternarSimulacaoFalha(Guid id)
    {
        var command = new AlternarSimulacaoFalhaCommand { Id = id };

        try
        {
            var produto = await _mediator.Send(command);
            return Ok(produto);
        }
        catch (DomainException ex)
        {
            return NotFound(new { sucesso = false, mensagem = ex.Message });
        }
    }
}

