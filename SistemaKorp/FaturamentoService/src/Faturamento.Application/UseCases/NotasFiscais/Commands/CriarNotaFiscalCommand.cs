using Faturamento.Application.DTOs;
using MediatR;

namespace Faturamento.Application.UseCases.NotasFiscais.Commands;

public class CriarNotaFiscalCommand : IRequest<NotaFiscalResponse>
{
    public List<ItemNotaFiscalDto> Itens { get; set; } = new();
}

