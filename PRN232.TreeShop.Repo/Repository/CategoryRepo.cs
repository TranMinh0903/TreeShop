using PRN232.LaptopShop.Repo.Entities;

namespace PRN232.LaptopShop.Repo.Repository
{
    public class CategoryRepo : GenericeRepository<Category>
    {
        public CategoryRepo(ShopDBContext context) : base(context)
        {
        }
    }
}
