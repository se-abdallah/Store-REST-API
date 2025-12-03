using System;
using AutoMapper;
using Store.Application.DTOs.Store;
using Store.Domain.Entities;

namespace Store.Application.Mapping;

public class AutoMapperProfiles : Profile
{
    public AutoMapperProfiles()
    {

        CreateMap<ProductTranslation, ProductTranslationDto>();


        CreateMap<Product, ProductAdminDetailDto>()
            .ForMember(dest => dest.Translations,
                opt => opt.MapFrom(src => src.Translations));


        CreateMap<Product, ProductPublicDetailDto>()
            .ForMember(dest => dest.Name, opt => opt.Ignore())
            .ForMember(dest => dest.Description, opt => opt.Ignore());


        CreateMap<Product, ProductListItemDto>()
            .ForMember(dest => dest.Name, opt => opt.Ignore());


        CreateMap<InvoiceDetail, InvoiceItemDto>();

        CreateMap<Invoice, InvoiceDto>()
            .ForMember(dest => dest.Items,
                opt => opt.MapFrom(src => src.Details))
            .ForMember(dest => dest.TotalProducts,
                opt => opt.MapFrom(src => src.Details.Count))
            .ForMember(dest => dest.TotalQuantity,
                opt => opt.MapFrom(src => src.Details.Sum(d => d.Quantity)));
    }
}