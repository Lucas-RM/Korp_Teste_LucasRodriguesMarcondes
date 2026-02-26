using Faturamento.Application.DTOs;
using MediatR;

namespace Faturamento.Application.UseCases.NotasFiscais.Queries;

public class ListarNotasFiscaisQuery : IRequest<IEnumerable<NotaFiscalResponse>>
{
}

