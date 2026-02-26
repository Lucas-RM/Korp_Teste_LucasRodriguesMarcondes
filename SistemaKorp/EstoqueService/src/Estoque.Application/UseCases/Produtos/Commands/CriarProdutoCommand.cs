using Estoque.Application.DTOs;
using MediatR;

namespace Estoque.Application.UseCases.Produtos.Commands;

public class CriarProdutoCommand : IRequest<ProdutoResponse>
{
    public string Codigo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public decimal Saldo { get; set; }
}

