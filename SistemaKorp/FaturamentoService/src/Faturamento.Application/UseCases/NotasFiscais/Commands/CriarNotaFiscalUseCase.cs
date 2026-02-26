using AutoMapper;
using Faturamento.Application.DTOs;
using Faturamento.Domain.Entities;
using Faturamento.Domain.Interfaces;
using MediatR;

namespace Faturamento.Application.UseCases.NotasFiscais.Commands;

public class CriarNotaFiscalUseCase : IRequestHandler<CriarNotaFiscalCommand, NotaFiscalResponse>
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CriarNotaFiscalUseCase(
        INotaFiscalRepository notaFiscalRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<NotaFiscalResponse> Handle(CriarNotaFiscalCommand request, CancellationToken cancellationToken)
    {
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

