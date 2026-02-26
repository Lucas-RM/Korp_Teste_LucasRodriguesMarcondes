using System.Net.Http.Json;
using System.Text.Json;
using Faturamento.Application.DTOs;
using Faturamento.Application.Interfaces;
using Faturamento.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Faturamento.Infrastructure.ExternalServices;

public class EstoqueHttpService : IEstoqueService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<EstoqueHttpService> _logger;

    public EstoqueHttpService(HttpClient httpClient, ILogger<EstoqueHttpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<BaixaEstoqueResponseDto> RealizarBaixaAsync(BaixaEstoqueRequestDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/produtos/baixa-estoque", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<BaixaEstoqueResponseDto>(cancellationToken: cancellationToken);
                return result ?? new BaixaEstoqueResponseDto { Sucesso = false, Mensagem = "Resposta inválida do serviço de estoque" };
            }
            else
            {
                var errorBody = await response.Content.ReadFromJsonAsync<BaixaEstoqueResponseDto>(cancellationToken: cancellationToken);
                var errorMessage = errorBody?.Mensagem ?? $"Erro ao comunicar com serviço de estoque: {response.StatusCode}";

                _logger.LogWarning($"Falha ao realizar baixa de estoque. Status: {response.StatusCode}, Mensagem: {errorMessage}");

                return new BaixaEstoqueResponseDto
                {
                    Sucesso = false,
                    Mensagem = errorMessage
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro HTTP ao realizar baixa de estoque");
            throw new EstoqueIndisponivelException("Serviço de estoque indisponível", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao realizar baixa de estoque");
            throw new EstoqueIndisponivelException("Timeout ao comunicar com serviço de estoque", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao realizar baixa de estoque");
            throw new EstoqueIndisponivelException("Erro ao comunicar com serviço de estoque", ex);
        }
    }

    public async Task<VerificarProdutosResponseDto> VerificarExistenciaProdutosAsync(IEnumerable<string> codigos, CancellationToken cancellationToken = default)
    {
        var request = new VerificarProdutosRequestDto
        {
            Codigos = codigos
                .Where(c => !string.IsNullOrWhiteSpace(c))
                .Select(c => c.Trim())
                .Distinct()
                .ToList()
        };

        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/v1/produtos/verificar-existencia", request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<VerificarProdutosResponseDto>(cancellationToken: cancellationToken);
                return result ?? new VerificarProdutosResponseDto
                {
                    Sucesso = false,
                    Mensagem = "Resposta inválida do serviço de estoque"
                };
            }
            else
            {
                var errorBody = await response.Content.ReadFromJsonAsync<VerificarProdutosResponseDto>(cancellationToken: cancellationToken);
                var errorMessage = errorBody?.Mensagem ?? $"Erro ao comunicar com serviço de estoque: {response.StatusCode}";

                _logger.LogWarning($"Falha ao verificar existência de produtos. Status: {response.StatusCode}, Mensagem: {errorMessage}");

                return new VerificarProdutosResponseDto
                {
                    Sucesso = false,
                    Mensagem = errorMessage,
                    CodigosNaoEncontrados = errorBody?.CodigosNaoEncontrados ?? new List<string>()
                };
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erro HTTP ao verificar existência de produtos");
            throw new EstoqueIndisponivelException("Serviço de estoque indisponível", ex);
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Timeout ao verificar existência de produtos");
            throw new EstoqueIndisponivelException("Timeout ao comunicar com serviço de estoque", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao verificar existência de produtos");
            throw new EstoqueIndisponivelException("Erro ao comunicar com serviço de estoque", ex);
        }
    }
}

