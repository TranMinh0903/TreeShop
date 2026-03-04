using PRN232.LaptopShop.Repo.Entities;

namespace PRN232.LaptopShop.Repo.Repository
{
    public class OrderRepo : GenericeRepository<Order>
    {
        public OrderRepo(ShopDBContext context) : base(context)
        {
        }
    }
}
