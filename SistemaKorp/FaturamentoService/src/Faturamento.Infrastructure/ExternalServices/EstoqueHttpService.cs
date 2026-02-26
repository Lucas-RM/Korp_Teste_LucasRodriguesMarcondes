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
}

