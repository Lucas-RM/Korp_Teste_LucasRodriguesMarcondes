namespace Faturamento.Application.DTOs;

public class ItemNotaFiscalResponseDto
{
    public Guid Id { get; set; }
    public string CodigoProduto { get; set; } = string.Empty;
    public string DescricaoProduto { get; set; } = string.Empty;
    public decimal Quantidade { get; set; }
}

