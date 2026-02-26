namespace Faturamento.Application.DTOs;

public class CriarNotaFiscalRequest
{
    public List<ItemNotaFiscalDto> Itens { get; set; } = new();
}

