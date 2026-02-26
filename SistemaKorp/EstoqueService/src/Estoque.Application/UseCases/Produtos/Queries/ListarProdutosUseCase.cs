using AutoMapper;
using Estoque.Application.DTOs;
using Estoque.Domain.Interfaces;
using MediatR;

namespace Estoque.Application.UseCases.Produtos.Queries;

public class ListarProdutosUseCase : IRequestHandler<ListarProdutosQuery, IEnumerable<ProdutoResponse>>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;

    public ListarProdutosUseCase(IProdutoRepository produtoRepository, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProdutoResponse>> Handle(ListarProdutosQuery request, CancellationToken cancellationToken)
    {
        var produtos = await _produtoRepository.ListarTodosAsync();
        return _mapper.Map<IEnumerable<ProdutoResponse>>(produtos);
    }
}

