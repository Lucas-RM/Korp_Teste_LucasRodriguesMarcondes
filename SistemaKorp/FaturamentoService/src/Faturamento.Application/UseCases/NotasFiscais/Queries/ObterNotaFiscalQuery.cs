using Faturamento.Application.DTOs;
using MediatR;

namespace Faturamento.Application.UseCases.NotasFiscais.Queries;

public class ObterNotaFiscalQuery : IRequest<NotaFiscalResponse>
{
    public Guid Id { get; set; }
}

