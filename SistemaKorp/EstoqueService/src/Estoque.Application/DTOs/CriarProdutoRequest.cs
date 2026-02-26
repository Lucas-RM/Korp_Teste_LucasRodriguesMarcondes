namespace Estoque.Application.DTOs;

public class CriarProdutoRequest
{
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
}

