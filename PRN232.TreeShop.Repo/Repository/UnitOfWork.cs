using PRN232.LaptopShop.Repo.Entities;

namespace PRN232.LaptopShop.Repo.Repository
{
    public class UnitOfWork
    {
        private readonly ShopDBContext _context;
        public AccountRepo AccountRepo { get; private set; }
        public CategoryRepo CategoryRepo { get; private set; }
        public ProductRepo ProductRepo { get; private set; }
        public OrderRepo OrderRepo { get; private set; }
        public OrderDetailRepo OrderDetailRepo { get; private set; }

        public UnitOfWork(ShopDBContext context, AccountRepo accountRepo, CategoryRepo categoryRepo, ProductRepo productRepo, OrderRepo orderRepo, OrderDetailRepo orderDetailRepo)
        {
            _context = context;
            AccountRepo = accountRepo;
            CategoryRepo = categoryRepo;
            ProductRepo = productRepo;
            OrderRepo = orderRepo;
            OrderDetailRepo = orderDetailRepo;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
