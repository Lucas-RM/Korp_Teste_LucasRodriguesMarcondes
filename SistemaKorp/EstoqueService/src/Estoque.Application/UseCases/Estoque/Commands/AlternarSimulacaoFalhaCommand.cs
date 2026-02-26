using Estoque.Application.DTOs;
using MediatR;

namespace Estoque.Application.UseCases.Estoque.Commands;

public class AlternarSimulacaoFalhaCommand : IRequest<ProdutoResponse>
{
    public Guid Id { get; set; }
}

