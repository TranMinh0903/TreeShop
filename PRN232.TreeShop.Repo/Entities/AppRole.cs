namespace PRN232.LaptopShop.Repo.Entities
{
    public static class AppRole
    {
        public const string User = "User";
        public const string Admin = "Admin";
        public const string Shipper = "Shipper";

        public static readonly string[] AllRoles = { User, Admin, Shipper };

        public static bool IsValid(string? role)
        {
            return role == null || AllRoles.Contains(role);
        }
    }
}
