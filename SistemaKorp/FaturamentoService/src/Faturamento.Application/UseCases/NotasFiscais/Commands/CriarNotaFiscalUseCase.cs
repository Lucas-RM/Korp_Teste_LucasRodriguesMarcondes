using AutoMapper;
using Faturamento.Application.DTOs;
using Faturamento.Application.Interfaces;
using Faturamento.Domain.Entities;
using Faturamento.Domain.Exceptions;
using Faturamento.Domain.Interfaces;
using MediatR;

namespace Faturamento.Application.UseCases.NotasFiscais.Commands;

public class CriarNotaFiscalUseCase : IRequestHandler<CriarNotaFiscalCommand, NotaFiscalResponse>
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEstoqueService _estoqueService;
    private readonly IMapper _mapper;

    public CriarNotaFiscalUseCase(
        INotaFiscalRepository notaFiscalRepository,
        IUnitOfWork unitOfWork,
        IEstoqueService estoqueService,
        IMapper mapper)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _unitOfWork = unitOfWork;
        _estoqueService = estoqueService;
        _mapper = mapper;
    }

    public async Task<NotaFiscalResponse> Handle(CriarNotaFiscalCommand request, CancellationToken cancellationToken)
    {
        var codigos = request.Itens
            .Select(i => i.CodigoProduto)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Select(c => c.Trim())
            .Distinct()
            .ToList();

        if (codigos.Count == 0)
            throw new DomainException("A nota fiscal deve conter ao menos um item com código de produto válido.");

        var verificacao = await _estoqueService.VerificarExistenciaProdutosAsync(codigos, cancellationToken);

        if (!verificacao.Sucesso)
        {
            if (verificacao.CodigosNaoEncontrados.Any())
            {
                var lista = string.Join(", ", verificacao.CodigosNaoEncontrados);
                throw new DomainException($"Os seguintes produtos não foram encontrados no estoque: {lista}.");
            }

            throw new DomainException(verificacao.Mensagem ?? "Não foi possível validar os produtos no estoque.");
        }

        var nota = NotaFiscal.Create();

        foreach (var itemDto in request.Itens)
        {
            nota.AdicionarItem(itemDto.CodigoProduto, itemDto.DescricaoProduto, itemDto.Quantidade);
        }

        await _notaFiscalRepository.AdicionarAsync(nota);
        await _unitOfWork.CommitAsync(cancellationToken);

        return _mapper.Map<NotaFiscalResponse>(nota);
    }
}

