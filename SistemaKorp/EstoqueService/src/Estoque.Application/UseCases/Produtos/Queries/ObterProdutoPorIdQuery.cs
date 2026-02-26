using Estoque.Application.DTOs;
using MediatR;

namespace Estoque.Application.UseCases.Produtos.Queries;

public class ObterProdutoPorIdQuery : IRequest<ProdutoResponse>
{
    public Guid Id { get; set; }
}

