using AutoMapper;
using Faturamento.Application.DTOs;
using Faturamento.Domain.Entities;
using Faturamento.Domain.Enums;

namespace Faturamento.Application.Mappings;

public class NotaFiscalMappingProfile : Profile
{
    public NotaFiscalMappingProfile()
    {
        CreateMap<NotaFiscal, NotaFiscalResponse>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<ItemNotaFiscal, ItemNotaFiscalResponseDto>();
    }
}

