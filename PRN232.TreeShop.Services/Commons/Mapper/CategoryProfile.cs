using AutoMapper;
using PRN232.LaptopShop.Repo.Entities;
using PRN232.LaptopShop.Services.Response;

namespace PRN232.LaptopShop.Services.Commons.Mapper
{
    public class CategoryProfile : Profile
    {

        public CategoryProfile()
        {
            // Map Category -> CategoryResponse
            CreateMap<Category, CategoryResponse>();
            // .ForMember(...) nếu có các field tên khác nhau
        }
    }
}
