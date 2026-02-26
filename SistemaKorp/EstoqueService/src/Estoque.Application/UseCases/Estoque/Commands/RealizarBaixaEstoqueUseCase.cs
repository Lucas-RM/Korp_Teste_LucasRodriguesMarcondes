using AutoMapper;
using Estoque.Application.DTOs;
using Estoque.Domain.Entities;
using Estoque.Domain.Exceptions;
using Estoque.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Estoque.Application.UseCases.Estoque.Commands;

public class RealizarBaixaEstoqueUseCase : IRequestHandler<RealizarBaixaEstoqueCommand, BaixaEstoqueResponse>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<RealizarBaixaEstoqueUseCase> _logger;

    public RealizarBaixaEstoqueUseCase(
        IProdutoRepository produtoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<RealizarBaixaEstoqueUseCase> logger)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<BaixaEstoqueResponse> Handle(RealizarBaixaEstoqueCommand request, CancellationToken cancellationToken)
    {
        var itensProcessados = new List<ItemBaixaProcessadoDto>();

        try
        {
            foreach (var item in request.ItensBaixa)
            {
                var produto = await _produtoRepository.ObterPorCodigoAsync(item.CodigoProduto);
                if (produto == null)
                    throw new DomainException($"Produto com código {item.CodigoProduto} não encontrado.");

                var saldoAnterior = produto.Saldo;
                produto.BaixarSaldo(item.Quantidade);
                await _produtoRepository.AtualizarAsync(produto);

                itensProcessados.Add(new ItemBaixaProcessadoDto
                {
                    CodigoProduto = item.CodigoProduto,
                    SaldoAnterior = saldoAnterior,
                    SaldoAtual = produto.Saldo,
                    QuantidadeBaixada = item.Quantidade
                });
            }

            await _unitOfWork.CommitAsync(cancellationToken);

            _logger.LogInformation($"Baixa de estoque realizada com sucesso. {itensProcessados.Count} itens processados.");

            return new BaixaEstoqueResponse
            {
                Sucesso = true,
                Mensagem = "Baixa de estoque realizada com sucesso.",
                ItensProcessados = itensProcessados
            };
        }
        catch (FalhaSimuladaException ex)
        {
            _logger.LogWarning($"Falha simulada detectada durante baixa de estoque: {ex.Message}");
            throw; 
        }
        catch (DomainException ex)
        {
            _logger.LogError(ex, $"Erro de domínio durante baixa de estoque: {ex.Message}");
            throw; 
        }
    }
}

