using Faturamento.Application.DTOs;

namespace Faturamento.Application.Interfaces;

public interface IEstoqueService
{
    Task<BaixaEstoqueResponseDto> RealizarBaixaAsync(BaixaEstoqueRequestDto request, CancellationToken cancellationToken = default);
}

