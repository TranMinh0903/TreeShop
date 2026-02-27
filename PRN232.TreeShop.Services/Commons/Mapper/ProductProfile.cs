using AutoMapper;
using PRN232.LaptopShop.Repo.Entities;
using PRN232.LaptopShop.Services.Response;

namespace PRN232.LaptopShop.Services.Commons.Mapper
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {

            CreateMap<Product, ProductDetailResponse>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName));
        }
    }
}
