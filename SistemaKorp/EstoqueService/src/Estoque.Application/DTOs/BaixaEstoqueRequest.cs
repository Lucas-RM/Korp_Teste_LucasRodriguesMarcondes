namespace Estoque.Application.DTOs;

public class BaixaEstoqueRequest
{
    public List<ItemBaixaDto> ItensBaixa { get; set; } = new();
}

