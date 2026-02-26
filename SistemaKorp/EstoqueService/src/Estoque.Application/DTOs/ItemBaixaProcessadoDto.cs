namespace Estoque.Application.DTOs;

public class ItemBaixaProcessadoDto
{
    public string CodigoProduto { get; set; } = string.Empty;
    public decimal SaldoAnterior { get; set; }
    public decimal SaldoAtual { get; set; }
    public decimal QuantidadeBaixada { get; set; }
}

