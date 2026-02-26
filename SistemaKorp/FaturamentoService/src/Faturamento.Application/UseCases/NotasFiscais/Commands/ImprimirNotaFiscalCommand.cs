using Faturamento.Application.DTOs;
using MediatR;

namespace Faturamento.Application.UseCases.NotasFiscais.Commands;

public class ImprimirNotaFiscalCommand : IRequest<ImprimirNotaFiscalResponse>
{
    public Guid Id { get; set; }
}

