namespace Faturamento.Application.DTOs;

public class ItemNotaFiscalDto
{
    public string CodigoProduto { get; set; } = string.Empty;
    public string DescricaoProduto { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
}

