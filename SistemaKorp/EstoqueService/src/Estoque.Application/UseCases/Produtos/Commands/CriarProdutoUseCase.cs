using AutoMapper;
using Estoque.Application.DTOs;
using Estoque.Domain.Entities;
using Estoque.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Estoque.Application.UseCases.Produtos.Commands;

public class CriarProdutoUseCase : IRequestHandler<CriarProdutoCommand, ProdutoResponse>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CriarProdutoUseCase> _logger;

    public CriarProdutoUseCase(
        IProdutoRepository produtoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        ILogger<CriarProdutoUseCase> logger)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ProdutoResponse> Handle(CriarProdutoCommand request, CancellationToken cancellationToken)
    {
        var produtoExistente = await _produtoRepository.ObterPorCodigoAsync(request.Codigo);
        if (produtoExistente != null)
            throw new Domain.Exceptions.DomainException($"Produto com código {request.Codigo} já existe.");

        var produto = Produto.Create(request.Codigo, request.Descricao, request.Saldo);
        await _produtoRepository.AdicionarAsync(produto);
        await _unitOfWork.CommitAsync(cancellationToken);

        _logger.LogInformation($"Produto criado com sucesso. Id: {produto.Id}, Código: {produto.Codigo}");

        return _mapper.Map<ProdutoResponse>(produto);
    }
}

