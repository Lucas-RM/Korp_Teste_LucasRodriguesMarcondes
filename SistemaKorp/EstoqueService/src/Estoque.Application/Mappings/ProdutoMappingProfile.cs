using AutoMapper;
using Estoque.Application.DTOs;
using Estoque.Domain.Entities;

namespace Estoque.Application.Mappings;

public class ProdutoMappingProfile : Profile
{
    public ProdutoMappingProfile()
    {
        CreateMap<Produto, ProdutoResponse>();
    }
}

