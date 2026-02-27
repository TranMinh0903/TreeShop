using PRN232.LaptopShop.Repo.Entities;

namespace PRN232.LaptopShop.Repo.Repository
{
    public class ProductRepo : GenericeRepository<Product>
    {
        public ProductRepo(ShopDBContext context) : base(context)
        {
        }
    }
}
