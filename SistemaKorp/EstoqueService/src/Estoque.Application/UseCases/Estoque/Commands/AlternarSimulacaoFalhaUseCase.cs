using AutoMapper;
using Estoque.Application.DTOs;
using Estoque.Domain.Interfaces;
using MediatR;

namespace Estoque.Application.UseCases.Estoque.Commands;

public class AlternarSimulacaoFalhaUseCase : IRequestHandler<AlternarSimulacaoFalhaCommand, ProdutoResponse>
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AlternarSimulacaoFalhaUseCase(
        IProdutoRepository produtoRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _produtoRepository = produtoRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProdutoResponse> Handle(AlternarSimulacaoFalhaCommand request, CancellationToken cancellationToken)
    {
        var produto = await _produtoRepository.ObterPorIdAsync(request.Id);
        if (produto == null)
            throw new Domain.Exceptions.DomainException($"Produto com ID {request.Id} não encontrado.");

        if (produto.SimularFalha)
            produto.DesativarSimulacaoFalha();
        else
            produto.AtivarSimulacaoFalha();

        await _produtoRepository.AtualizarAsync(produto);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<ProdutoResponse>(produto);
    }
}

