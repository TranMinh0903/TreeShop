using PRN232.LaptopShop.Repo.Entities;

namespace PRN232.LaptopShop.Repo.Repository
{
    public class OrderDetailRepo : GenericeRepository<OrderDetail>
    {
        public OrderDetailRepo(ShopDBContext context) : base(context)
        {
        }
    }
}
