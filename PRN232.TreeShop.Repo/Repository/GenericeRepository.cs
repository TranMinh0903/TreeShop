using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using PRN232.LaptopShop.Repo.Entities;
using PRN232.LaptopShop.Repo.Utils;
using Repository.Utils;

namespace PRN232.LaptopShop.Repo.Repository
{
    public class GenericeRepository<T> where T : class
    {
        private readonly ShopDBContext _context;
        private readonly DbSet<T> _dbSet;



        public GenericeRepository(ShopDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public Task UpdateAsync(T entity)
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }


        public async Task DeleteAsync(object id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
            }
        }
        public async Task<IList<T>> GetAllAsync(
        Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null)
                query = include(query);

            return await query.ToListAsync();
        }

        public Task<T?> GetByIdAsync(object id)
        {
            var entity = _dbSet.FindAsync(id);
            return entity.AsTask();
        }

        public IQueryable<T> Entity => _dbSet;

        public async Task<BasePaginatedList<T>> GetPagging(IQueryable<T> query, int index, int pageSize)
        {
            var count = await query.CountAsync();
            var items = await query.Skip((index - 1) * pageSize).Take(pageSize).ToListAsync();
            return new BasePaginatedList<T>(items, count, index, pageSize);
        }

        public async Task<T?> FindAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IList<T>> FindAllAsync(
            Expression<Func<T, bool>> predicate,
            Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            if (include != null)
                query = include(query);

            return await query.Where(predicate).ToListAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<BasePaginatedList<object>> GetAllWithPaggingSortSelectionFieldAsync<TEntity, TResponse>(
            IQueryable<TEntity> query,
            IConfigurationProvider mapperConfig, // Truyền _mapper.ConfigurationProvider
            string? orderBy = null,
            string? fields = null,
            int pageIndex = 1,
            int pageSize = 10)
        {
            // 1. Validation Fields dựa trên TResponse
            // Nói cách khác , chỉ những field nào tồn tại trong TResponse mới được phép chọn và sắp xếp
            // Việc này tránh select data không cần thiết từ database, đồng thời cũng giúp bảo mật khi có những field nhạy cảm tồn tại trong TEntity nhưng không tồn tại trong TResponse
            var validFields = QueryHelper.GetValidFields<TResponse>(fields);
            var validOrderBy = QueryHelper.GetValidOrderBy<TResponse>(orderBy);

            var count = await query.CountAsync();

            // 2. ProjectTo trước để chuyển từ Entity -> DTO
            // Việc này giúp giấu các field nhạy cảm ngay từ đầu
            // Thay vì query hết từ entity rồi mới chọn field, thì bây giờ chỉ query những field cần thiết đã được map sang DTO

            // Tại sao lại cấn projectTo này --> Nếu không có ProjectTo, thì query sẽ trả về TEntity, sau đó mới chọn field động trên TEntity. Điều này sẽ gây ra lỗi nếu có field nào đó tồn tại trong TEntity nhưng không tồn tại trong TResponse.
            var dtoQuery = query.ProjectTo<TResponse>(mapperConfig);

            // 3. Sorting trên DTO
            // orderBy sẽ được truyền vào dưới dạng string, ví dụ: "Name desc, Age asc"
            // lib: System.Linq.Dynamic.Core sẽ parse string này và áp dụng sorting động trên DTO
            if (!string.IsNullOrWhiteSpace(validOrderBy))
                dtoQuery = dtoQuery.OrderBy(validOrderBy);

            // 4. Paging
            var pagedQuery = dtoQuery.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            // 5. Select Field động trên DTO
            // Kết quả trả về List<object> (kiểu vô danh chứa các field của TResponse)
            // Cái này cũng có mặt hại là không thẻ strong type được nữa, nhưng bù lại có thể linh hoạt chọn field nào cần thiết để trả về, tránh trả về data không cần thiết
            // Và cũng khó khăn hơn trong việc sử dụng kết quả trả về, vì phải dùng dynamic để truy cập các field
            var items = await pagedQuery
                .Select($"new ({validFields})")
                .ToDynamicListAsync();

            return new BasePaginatedList<object>(items.Cast<object>().ToList(), count, pageIndex, pageSize);
        }

        public IQueryable<T> AsQueryable()
        {
            // Sử dụng AsNoTracking() để tăng hiệu năng nếu Read-only
            return _context.Set<T>().AsNoTracking().AsQueryable();
        }

    }
}
