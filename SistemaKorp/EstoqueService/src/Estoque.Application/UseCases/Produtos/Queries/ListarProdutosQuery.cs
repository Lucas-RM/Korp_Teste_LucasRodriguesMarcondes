using Estoque.Application.DTOs;
using MediatR;

namespace Estoque.Application.UseCases.Produtos.Queries;

public class ListarProdutosQuery : IRequest<IEnumerable<ProdutoResponse>>
{
}

