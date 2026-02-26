using AutoMapper;
using Faturamento.Application.DTOs;
using Faturamento.Application.Interfaces;
using Faturamento.Domain.Exceptions;
using Faturamento.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Faturamento.Application.UseCases.NotasFiscais.Commands;

public class ImprimirNotaFiscalUseCase : IRequestHandler<ImprimirNotaFiscalCommand, ImprimirNotaFiscalResponse>
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEstoqueService _estoqueService;
    private readonly IMapper _mapper;
    private readonly ILogger<ImprimirNotaFiscalUseCase> _logger;

    public ImprimirNotaFiscalUseCase(
        INotaFiscalRepository notaFiscalRepository,
        IUnitOfWork unitOfWork,
        IEstoqueService estoqueService,
        IMapper mapper,
        ILogger<ImprimirNotaFiscalUseCase> logger)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _unitOfWork = unitOfWork;
        _estoqueService = estoqueService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ImprimirNotaFiscalResponse> Handle(ImprimirNotaFiscalCommand request, CancellationToken cancellationToken)
    {
        // Passo 1: Buscar nota fiscal pelo ID com os itens
        var nota = await _notaFiscalRepository.ObterPorIdComItensAsync(request.Id);
        if (nota == null)
            throw new DomainException($"Nota fiscal com ID {request.Id} não encontrada.");

        // Passo 2: Verificar se pode ser impressa
        if (!nota.PodeSerImpressao())
        {
            return new ImprimirNotaFiscalResponse
            {
                Sucesso = false,
                Mensagem = "Nota já foi fechada ou não está em estado válido",
                EmProcessamento = false
            };
        }

        // Passo 3: Registrar log
        _logger.LogInformation($"Iniciando impressão da nota fiscal {nota.Numero}");

        // Passo 4: Montar BaixaEstoqueRequestDto
        var baixaRequest = new BaixaEstoqueRequestDto
        {
            ItensBaixa = nota.Itens.Select(i => new ItemBaixaDto
            {
                CodigoProduto = i.CodigoProduto,
                Quantidade = i.Quantidade
            }).ToList()
        };

        try
        {
            // Passo 6: Chamar IEstoqueService.RealizarBaixaAsync (com Polly Retry)
            var baixaResponse = await _estoqueService.RealizarBaixaAsync(baixaRequest, cancellationToken);

            // Passo 7: Se sucesso
            if (baixaResponse.Sucesso)
            {
                nota.Fechar();
                await _notaFiscalRepository.AtualizarAsync(nota);
                await _unitOfWork.CommitAsync(cancellationToken);

                _logger.LogInformation($"Nota fiscal {nota.Numero} fechada com sucesso após baixa de estoque");

                return new ImprimirNotaFiscalResponse
                {
                    Sucesso = true,
                    EmProcessamento = false,
                    Nota = _mapper.Map<NotaFiscalResponse>(nota)
                };
            }
            else
            {
                // Passo 9: Se estoque retornar Sucesso=false
                _logger.LogWarning($"Baixa de estoque recusada para nota {nota.Numero}: {baixaResponse.Mensagem}");
                return new ImprimirNotaFiscalResponse
                {
                    Sucesso = false,
                    Mensagem = baixaResponse.Mensagem,
                    EmProcessamento = false
                };
            }
        }
        catch (Exception ex)
        {
            // Passo 8: Se chamada ao estoque lançar exceção
            _logger.LogError(ex, $"Falha ao realizar baixa de estoque para nota {nota.Numero}. Nota mantida com Status=Aberta. Mensagem: {ex.Message}");

            return new ImprimirNotaFiscalResponse
            {
                Sucesso = false,
                Mensagem = "Não foi possível realizar a baixa de estoque. Tente novamente mais tarde.",
                EmProcessamento = false
            };
        }
    }
}

