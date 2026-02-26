using AutoMapper;
using Estoque.Application.DTOs;
using Estoque.Domain.Interfaces;
using MediatR;

namespace Estoque.Application.UseCases.Produtos.Queries;

public class ObterProdutoPorIdUseCase : IRequestHandler<ObterProdutoPorIdQuery, ProdutoResponse>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IMapper _mapper;

    public ObterProdutoPorIdUseCase(IProdutoRepository produtoRepository, IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _mapper = mapper;
    }

    public async Task<ProdutoResponse> Handle(ObterProdutoPorIdQuery request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.Id);
        if (produto == null)
            throw new Domain.Exceptions.DomainException($"Produto com ID {request.Id} não encontrado.");

        return _mapper.Map<ProdutoResponse>(produto);
    }
}

