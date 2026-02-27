using PRN232.LaptopShop.Repo.Entities;

namespace PRN232.LaptopShop.Repo.Repository
{
    public class AccountRepo : GenericeRepository<Account>
    {
        public AccountRepo(ShopDBContext context) : base(context)
        {
        }
    }
}
