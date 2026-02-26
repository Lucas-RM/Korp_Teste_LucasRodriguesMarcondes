using Estoque.Application.DTOs;
using MediatR;

namespace Estoque.Application.UseCases.Estoque.Commands;

public class RealizarBaixaEstoqueCommand : IRequest<BaixaEstoqueResponse>
{
    public List<ItemBaixaDto> ItensBaixa { get; set; } = new();
}

