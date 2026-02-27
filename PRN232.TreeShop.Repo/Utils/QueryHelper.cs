namespace PRN232.LaptopShop.Repo.Utils
{
    public static class QueryHelper
    {
        // Utils 1: Lọc chuỗi Fields để chỉ lấy các field có trong DTO và luôn ép có Id
        public static string GetValidFields<TDto>(string? fields)
        {
            var allowedFields = typeof(TDto).GetProperties()
                .Select(p => p.Name)
                .ToList();

            if (string.IsNullOrWhiteSpace(fields))
                return string.Join(", ", allowedFields); // Trả về tất cả field của DTO nếu ko truyền

            var requestedFields = fields.Split(',')
                .Select(f => f.Trim())
                .Where(f => allowedFields.Any(af => string.Equals(af, f, StringComparison.OrdinalIgnoreCase)))
                .ToList();

            // Luôn đảm bảo có Id (nếu DTO có Id)
            if (allowedFields.Contains("Id") && !requestedFields.Any(f => f.Equals("Id", StringComparison.OrdinalIgnoreCase)))
                requestedFields.Insert(0, "Id");

            return requestedFields.Any() ? string.Join(", ", requestedFields) : string.Join(", ", allowedFields);
        }

        // Utils 2: Kiểm tra chuỗi OrderBy có hợp lệ không
        public static string? GetValidOrderBy<TDto>(string? orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy)) return null;

            var allowedFields = typeof(TDto).GetProperties().Select(p => p.Name).ToList();

            // Tách chuỗi "Name DESC, Age ASC" -> "Name", "Age"
            var parts = orderBy.Split(',')
                .Select(p => p.Trim().Split(' ')[0])
                .ToList();

            // Nếu có bất kỳ field nào không nằm trong DTO -> Trả về null hoặc mặc định (để tránh lỗi crash)
            bool isValid = parts.All(p => allowedFields.Any(af => string.Equals(af, p, StringComparison.OrdinalIgnoreCase)));

            return isValid ? orderBy : null;
        }
    }
}
