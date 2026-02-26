using AutoMapper;
using Faturamento.Application.DTOs;
using Faturamento.Domain.Interfaces;
using MediatR;

namespace Faturamento.Application.UseCases.NotasFiscais.Queries;

public class ListarNotasFiscaisUseCase : IRequestHandler<ListarNotasFiscaisQuery, IEnumerable<NotaFiscalResponse>>
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IMapper _mapper;

    public ListarNotasFiscaisUseCase(INotaFiscalRepository notaFiscalRepository, IMapper mapper)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<NotaFiscalResponse>> Handle(ListarNotasFiscaisQuery request, CancellationToken cancellationToken)
    {
        var notas = await _notaFiscalRepository.ListarTodosAsync();
        return _mapper.Map<IEnumerable<NotaFiscalResponse>>(notas);
    }
}

