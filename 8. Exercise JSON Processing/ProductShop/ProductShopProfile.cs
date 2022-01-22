using AutoMapper;
using ProductShop.DTO.Input;
using ProductShop.DTO.Output;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserInputDTO, User>();

            CreateMap<ProductInputDTO, Product>();

            CreateMap<CategoryInputDTO, Category>();

            CreateMap<CategoryProductInputDTO, CategoryProduct>();

            CreateMap<Product, ProductOutputDTO>()
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => $"{src.Seller.FirstName} {src.Seller.LastName}"));
        }
    }
}