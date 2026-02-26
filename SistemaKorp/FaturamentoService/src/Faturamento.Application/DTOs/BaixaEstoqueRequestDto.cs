namespace Faturamento.Application.DTOs;

public class BaixaEstoqueRequestDto
{
    public List<ItemBaixaDto> ItensBaixa { get; set; } = new();
}

