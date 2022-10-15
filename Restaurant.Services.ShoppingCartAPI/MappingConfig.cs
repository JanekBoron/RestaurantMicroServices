using AutoMapper;
using Restaurant.Services.ShoppingCartAPI.Models;
using Restaurant.Services.ShoppingCartAPI.Models.Dto;

namespace Restaurant.Services.ShoppingCartAPI
{
    public class MappingConfig
    {
        public static MapperConfiguration RegisterMaps()
        {
            var mappingConfig = new MapperConfiguration(config =>
            {
                config.CreateMap<ProductDto, Product>().ReverseMap();
                config.CreateMap<CartHeaderDto, CartHeader>().ReverseMap();
                config.CreateMap<CartDto, Cart>().ReverseMap();
                config.CreateMap<CartDetailDto, CartDetails>().ReverseMap();

            });
            return mappingConfig;
        }
    }
}
