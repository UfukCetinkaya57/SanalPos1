using AutoMapper;
using SanalPos.Application.DTOs;
using SanalPos.Domain.Entities;
using SanalPos.Domain.Enums;

namespace SanalPos.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Invoice, InvoiceDto>();
            CreateMap<InvoiceDto, Invoice>();

            CreateMap<PaymentTransaction, PaymentTransactionDto>()
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.LastModifiedAt, opt => opt.MapFrom(src => src.LastModifiedAt));
                
            CreateMap<PaymentTransactionDto, PaymentTransaction>();
        }
    }
} 