namespace Estoque.Application.DTOs;

public class BaixaEstoqueResponse
{
    public bool Sucesso { get; set; }
    public string Mensagem { get; set; } = string.Empty;
    public List<ItemBaixaProcessadoDto> ItensProcessados { get; set; } = new();
}

