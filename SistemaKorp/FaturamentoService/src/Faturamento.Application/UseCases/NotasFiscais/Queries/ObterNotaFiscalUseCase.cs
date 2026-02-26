using AutoMapper;
using Faturamento.Application.DTOs;
using Faturamento.Domain.Exceptions;
using Faturamento.Domain.Interfaces;
using MediatR;

namespace Faturamento.Application.UseCases.NotasFiscais.Queries;

public class ObterNotaFiscalUseCase : IRequestHandler<ObterNotaFiscalQuery, NotaFiscalResponse>
{
    private readonly INotaFiscalRepository _notaFiscalRepository;
    private readonly IMapper _mapper;

    public ObterNotaFiscalUseCase(INotaFiscalRepository notaFiscalRepository, IMapper mapper)
    {
        _notaFiscalRepository = notaFiscalRepository;
        _mapper = mapper;
    }

    public async Task<NotaFiscalResponse> Handle(ObterNotaFiscalQuery request, CancellationToken cancellationToken)
    {
        var nota = await _notaFiscalRepository.ObterPorIdComItensAsync(request.Id);
        if (nota == null)
            throw new DomainException($"Nota fiscal com ID {request.Id} não encontrada.");

        return _mapper.Map<NotaFiscalResponse>(nota);
    }
}

