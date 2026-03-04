using AutoMapper;
using PRN232.LaptopShop.Repo.Entities;
using PRN232.LaptopShop.Services.Response;

namespace PRN232.LaptopShop.Services.Commons.Mapper
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderResponse>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Username))
                .ForMember(dest => dest.ShipperName, opt => opt.MapFrom(src => src.Shipper != null ? src.Shipper.Username : null))
                .ForMember(dest => dest.OrderDetails, opt => opt.MapFrom(src => src.OrderDetails));

            CreateMap<OrderDetail, OrderDetailResponse>();
        }
    }
}
