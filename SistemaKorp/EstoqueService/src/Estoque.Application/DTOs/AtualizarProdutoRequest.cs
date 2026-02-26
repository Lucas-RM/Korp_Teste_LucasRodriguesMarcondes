namespace Estoque.Application.DTOs;

public class AtualizarProdutoRequest
{
    public string Descricao { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
}

